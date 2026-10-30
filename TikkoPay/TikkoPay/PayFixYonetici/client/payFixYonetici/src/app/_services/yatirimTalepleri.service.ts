import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { Durum } from '../_enums/durum.enum';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { ToplamYatirimTutariDto } from '../_models/yatirimTalepleri/toplamYatirimTutari.dto';
import { ToplamCekimTutariDto } from '../_models/yatirimTalepleri/toplamCekimTutari.dto';
import { YatirimCreateDto } from '../_models/yatirimTalepleri/yatirimCreate.dto';
import { CekimCreateDto } from '../_models/yatirimTalepleri/cekimCreate.dto';

@Injectable({
    providedIn: 'root'
})
export class YatirimTalepleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<YatirimTalepleriListDto>> {
        return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto>>(`${this.baseApiUrl}/YatirimTalepleri/${id}`);
    }

    getParams(filterModel: YatirimTalepleriFilter): HttpParams {
        let params = new HttpParams();
        
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

    getBekleyenYatirimTalepleri(filterModel: YatirimTalepleriFilter): Observable<ResponseViewModel<YatirimTalepleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/YatirimTalepleri/bekleyen-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto[]>>(url, { params });
    }

    getBekleyenYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/YatirimTalepleri/bekleyen-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    exportToExcel(filterModel: YatirimTalepleriFilter, islemTipi: IslemTip, durum: Durum): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleri/exportToExcel`;
        let params = this.getParams(filterModel);

        params = params.set('islemTipi', islemTipi);
        params = params.set('durum', durum);

        return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
    }

    getOnaylananYatirimTalepleri(filterModel: YatirimTalepleriFilter): Observable<ResponseViewModel<YatirimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleri/onaylanan-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto[]>>(url, { params });
    }

    getOnaylananYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleri/onaylanan-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    getRedYatirimTalepleri(filterModel: YatirimTalepleriFilter): Observable<ResponseViewModel<YatirimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleri/red-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto[]>>(url, { params });
    }

    getRedYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/YatirimTalepleri/red-yatirim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
    }

    getToplamYatirimTutari(filterModel: YatirimTalepleriFilter){
        const url: string = `${this.baseApiUrl}/YatirimTalepleri/get-toplam-yatirim-tutari`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<ToplamYatirimTutariDto[]>>(url, { params });
    }
}