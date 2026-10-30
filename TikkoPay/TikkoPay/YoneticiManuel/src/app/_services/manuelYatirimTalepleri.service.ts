import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { YatirimManuelCreateDto } from '../_models/yatirimTalepleri/yatirimManuelCreate.dto';
import { Observable } from 'rxjs';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';

@Injectable({
    providedIn: 'root'
})
export class ManuelYatirimTalepleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getBekleyenYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/YatirimTalepleriManuel/bekleyen-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    getOnaylananYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/YatirimTalepleriManuel/onaylanan-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    getRedYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleriManuel/red-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    createYatirim(item: YatirimManuelCreateDto) {
        return this.httpClient.post<ResponseViewModel<YatirimManuelCreateDto>>(`${this.baseApiUrl}/YatirimTalepleriManuel`, item);
    }

    updateIsSendToWebSiteValue(id: string): Observable<any>  {
        const url: string = `${this.baseApiUrl}/YatirimTalepleriManuel/update-issendtowebsite-value?id=${id}`;
        return this.httpClient.put<ResponseViewModel<YatirimTalepleriListDto>>(url, null);
    }

    getParams(filterModel: YatirimTalepleriFilter): HttpParams {
        let params = new HttpParams();
        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
        }
        if (filterModel.bankaId) {
            params = params.set('bankaId', filterModel.bankaId.toString());
        }
        if (filterModel.webSiteId) {
            params = params.set('webSiteId', filterModel.webSiteId.toString());
        }
        if (filterModel.approveDateStart) {
            params = params.set('approveDateStart', (filterModel.approveDateStart));
        }
        if (filterModel.approveDateEnd) {
            params = params.set('approveDateEnd', (filterModel.approveDateEnd));
        }
        if (filterModel.createdDateStart) {
            params = params.set('createdDateStart', (filterModel.createdDateStart));
        }
        if (filterModel.createdDateEnd) {
            params = params.set('createdDateEnd', (filterModel.createdDateEnd));
        }

        params = params.set('searchText', filterModel.searchText);

        return params;
    }

}