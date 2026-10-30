export class ResponseViewModel<T> {
    success: boolean;
    message: string;
    result: any;

    constructor() {
        this.success = false;
        this.message = '';
        this.result;
    }
}
