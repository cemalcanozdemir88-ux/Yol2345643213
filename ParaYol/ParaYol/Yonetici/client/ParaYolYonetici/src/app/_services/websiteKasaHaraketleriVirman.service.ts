import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteKasaHaraketleriVirmanCreateDto } from "../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriVirmanCreate.dto";


@Injectable({
    providedIn: 'root'
})
export class WebSiteKasaHaraketleriVirmanService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    create(item: WebSiteKasaHaraketleriVirmanCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteKasaHaraketleriVirmanCreateDto>>(`${this.baseApiUrl}/websitekasaharaketlerivirman`, item);
    }

}