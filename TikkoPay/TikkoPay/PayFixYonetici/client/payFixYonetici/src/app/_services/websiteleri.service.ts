import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteleriItemDto } from "../_models/websiteleri/websiteleriItem.dto";
import { WebSiteleriListDto } from "../_models/websiteleri/websiteleriList.dto";
import { WebSiteleriCreateDto } from "../_models/websiteleri/websiteleriCreate.dto";
import { WebSiteBakiyeListDto } from "../_models/websiteleri/webSiteBakiyeList.dto";

@Injectable({
    providedIn: 'root'
})
export class WebSiteleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<WebSiteleriItemDto>> {
        return this.httpClient.get<ResponseViewModel<WebSiteleriItemDto>>(`${this.baseApiUrl}/websiteleri/${id}`);
    }

    getSelectList(): Observable<ResponseViewModel<WebSiteleriListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteleriListDto[]>>(`${this.baseApiUrl}/websiteleri/get-select-list`);
    }

    getSelectListByAffiliate(affiliateId: string): Observable<ResponseViewModel<WebSiteleriListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteleriListDto[]>>(`${this.baseApiUrl}/websiteleri/get-select-list-by-affiliate?affiliateId=${affiliateId}`);
    }

    getAll(): Observable<ResponseViewModel<WebSiteleriListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteleriListDto[]>>(`${this.baseApiUrl}/websiteleri/get-all`);
    }

    getBakiyeList(webSiteId: string): Observable<ResponseViewModel<WebSiteBakiyeListDto[]>> {
        const url: string = `${this.baseApiUrl}/websiteleri/get-bakiyelist`;
        let params = new HttpParams();

        if (webSiteId) {
            params = params.set('webSiteId', webSiteId);
        }

        return this.httpClient.get<ResponseViewModel<WebSiteBakiyeListDto[]>>(url, { params });
    }

    create(webSite: WebSiteleriCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteleriCreateDto>>(`${this.baseApiUrl}/websiteleri/create`, webSite);
    }

    update(webSite: WebSiteleriCreateDto) {
        return this.httpClient.put<ResponseViewModel<WebSiteleriCreateDto>>(`${this.baseApiUrl}/websiteleri/update`, webSite);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websiteleri/${id}`);
    }
}