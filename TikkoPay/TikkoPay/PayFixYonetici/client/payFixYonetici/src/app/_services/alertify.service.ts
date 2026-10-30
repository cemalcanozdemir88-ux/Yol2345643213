import { Injectable } from "@angular/core";

declare let alertify: any;

@Injectable({
    providedIn: "root"
})
export class AlertifyService {
    success(message: string) {
        alertify.success(message, 5);
        alertify.set('notifier', 'position', 'top-right');
    }

    error(message: string) {
        alertify.error(message, 5);
        alertify.set('notifier', 'position', 'top-right');
    }

    warning(message: string) {
        alertify.warning(message, 5);
        alertify.set('notifier', 'position', 'top-right');
    }

    message(message: string) {
        alertify.message(message, 5);
        alertify.set('notifier', 'position', 'top-right');
    }
}