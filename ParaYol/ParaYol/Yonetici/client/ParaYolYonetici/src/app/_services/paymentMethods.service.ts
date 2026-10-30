import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { PaymentMethodsDto } from "../_models/paymentMethods/paymentMethods.dto";
import { ResponseViewModel } from "../_models/responseviewmodel";

@Injectable({
    providedIn: 'root'
})
export class PaymentMethodsService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getAll(): Observable<ResponseViewModel<PaymentMethodsDto[]>> {
        const url: string = `${this.baseApiUrl}/PaymentMethods/get-all`;

        let params = new HttpParams();

        return this.httpClient.get<ResponseViewModel<PaymentMethodsDto[]>>(url, { params });
    }
}