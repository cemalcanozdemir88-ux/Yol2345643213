import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteKullanicilariCreateDto } from "../_models/webSiteKullanicilari/webSiteKullanicilariCreate.dto";
import { WebSiteKullanicilariListDto } from "../_models/webSiteKullanicilari/webSiteKullanicilariList.dto";
import { WebSiteKullanicilariUpdateDto } from "../_models/webSiteKullanicilari/webSiteKullanicilariUpdate.dto";


@Injectable({
    providedIn: 'root'
})
export class WebSiteKullanicilariService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<WebSiteKullanicilariListDto>> {
        return this.httpClient.get<ResponseViewModel<WebSiteKullanicilariListDto>>(`${this.baseApiUrl}/websitekullanicilari/${id}`);
    }

    getAll(): Observable<ResponseViewModel<WebSiteKullanicilariListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteKullanicilariListDto[]>>(`${this.baseApiUrl}/websitekullanicilari`);
    }

    create(item: WebSiteKullanicilariCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteKullanicilariCreateDto>>(`${this.baseApiUrl}/websitekullanicilari`, item);
    }

    update(item: WebSiteKullanicilariUpdateDto) {
        return this.httpClient.put<ResponseViewModel<WebSiteKullanicilariUpdateDto>>(`${this.baseApiUrl}/websitekullanicilari`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websitekullanicilari/${id}`);
    }
}