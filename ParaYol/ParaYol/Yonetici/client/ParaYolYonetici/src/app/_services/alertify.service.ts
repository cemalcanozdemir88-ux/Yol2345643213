import { Injectable } from "@angular/core";

declare let alertify: any;

@Injectable({
    providedIn: "root"
})
export class AlertifyService {
    success(message: string) {
        alertify.success(message, 15);
        alertify.set('notifier', 'position', 'top-center');
    }

    error(message: string) {
        alertify.error(message, 15);
        alertify.set('notifier', 'position', 'top-center');
    }

    warning(message: string) {
        alertify.warning(message, 15);
        alertify.set('notifier', 'position', 'top-center');
    }

    message(message: string) {
        alertify.message(message, 15);
        alertify.set('notifier', 'position', 'top-center');
    }
}