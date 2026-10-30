import { Injectable } from "@angular/core";
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { BehaviorSubject, Observable } from "rxjs";
import { environment } from "src/environment";

@Injectable({
    providedIn: 'root',
})
export class SignalrService {
    private hubConnection!: HubConnection;

    // 📩 Genel mesajlar
    private messageSubject = new BehaviorSubject<string | null>(null);
    public messages$: Observable<string | null> = this.messageSubject.asObservable();

    private updateHavaleBankaListSubject = new BehaviorSubject<void>(undefined);
    public updateHavaleBankaList$: Observable<void> = this.updateHavaleBankaListSubject.asObservable();

    constructor() { }


    public startConnection(): void {
        const hubUrl = environment.baseApiUrl.replace(/\/api\/?$/i, '') + '/hubs/notify';

        const forceLongPolling = environment.forceSignalRLongPolling === true;

        const attachHandlers = (conn: HubConnection) => {

            // 🔔 Genel mesaj
            conn.on('ReceiveMessage', (message: string) => {
                console.log('ReceiveMessage:', message);
                this.messageSubject.next(message);
            });

            // 🛒 Havale bankası pasif edildiğinde çalışacak method
            conn.on('UpdateHavaleBankaList', () => {
                this.updateHavaleBankaListSubject.next();
            });

            conn.onreconnecting((error) => {
                console.warn('SignalR reconnecting', error);
            });

            conn.onreconnected((connectionId) => {
                console.info('SignalR reconnected:', connectionId);
            });

            conn.onclose((error) => {
                console.error('SignalR closed', error);
            });
        };

        const buildConnection = (transport: HttpTransportType) =>
            new HubConnectionBuilder()
                .withUrl(hubUrl, {
                    // Read token dynamically so latest token (set after login) is used
                    accessTokenFactory: () => localStorage.getItem('token') ?? '',
                    transport,
                })
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Information)
                .build();

        const startWithTransport = async (transport: HttpTransportType) => {
            if (this.hubConnection) {
                try {
                    await this.hubConnection.stop();
                } catch { }
            }

            this.hubConnection = buildConnection(transport);
            attachHandlers(this.hubConnection);
            await this.hubConnection.start();
        };

        const tryStart = async () => {
            try {
                if (forceLongPolling) {
                    await startWithTransport(HttpTransportType.LongPolling);
                } else {
                    try {
                        await startWithTransport(HttpTransportType.WebSockets);
                    } catch {
                        await startWithTransport(HttpTransportType.LongPolling);
                    }
                }

                console.info('✅ SignalR connected');
            } catch (err) {
                console.error('❌ SignalR start error', err);
            }
        };

        tryStart();
    }


    public stopConnection(): void {
        if (this.hubConnection) {
            this.hubConnection.stop().catch((err) =>
                console.error('SignalR stop error', err)
            );
        }
    }
}