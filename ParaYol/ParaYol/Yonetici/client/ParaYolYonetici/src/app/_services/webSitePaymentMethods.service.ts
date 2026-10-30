import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSitePaymentMethodsDto } from "../_models/webSitePaymentMethods/webSitePaymentMethods.dto";

@Injectable({
    providedIn: 'root'
})
export class WebSitePaymentMethodsService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getByWebSiteId(webSiteId: string): Observable<ResponseViewModel<WebSitePaymentMethodsDto[]>> {
        const url: string = `${this.baseApiUrl}/WebSitePaymentMethods/get-by-websiteid`;

        let params = new HttpParams();
        params = params.set('webSiteId', webSiteId);

        return this.httpClient.get<ResponseViewModel<WebSitePaymentMethodsDto[]>>(url, { params });
    }
}