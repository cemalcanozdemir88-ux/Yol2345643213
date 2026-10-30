import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { Durum } from '../_enums/durum.enum';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { ToplamCekimTutariDto } from '../_models/yatirimTalepleri/toplamCekimTutari.dto';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTalepleriList.dto';
import { CekimTalepleriFilter, CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';

@Injectable({
    providedIn: 'root'
})
export class CekimTalepleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<CekimTalepleriListDto>> {
        return this.httpClient.get<ResponseViewModel<CekimTalepleriListDto>>(`${this.baseApiUrl}/CekimTalepleri/${id}`);
    }

    getParams(filterModel: CekimTalepleriFilter): HttpParams {
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

    exportToExcel(filterModel: CekimTalepleriFilter, durum: boolean): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/exportToExcel`;
        let params = this.getParams(filterModel);

        params = params.set('durum', durum.toString());

        return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
    }

    getOnaylananCekimTalepleri(filterModel: CekimTalepleriFilter): Observable<ResponseViewModel<CekimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/onaylanan-cekim-talepleri`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<CekimTalepleriListDto[]>>(url, { params });
    }

    getOnaylananCekimTalepleriWithPaging(filterModel: CekimTalepleriPagingFilter): Observable<ResponseViewModelPaging<CekimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/onaylanan-cekim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<CekimTalepleriListDto[]>>(url, { params });
    }

    getRedCekimTalepleri(filterModel: CekimTalepleriFilter): Observable<ResponseViewModel<CekimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/red-cekim-talepleri`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<CekimTalepleriListDto[]>>(url, { params });
    }

    getRedCekimTalepleriWithPaging(filterModel: CekimTalepleriPagingFilter): Observable<ResponseViewModelPaging<CekimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/red-cekim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<CekimTalepleriListDto[]>>(url, { params });
    }

    getToplamCekimTutari(filterModel: CekimTalepleriFilter){
        const url: string = `${this.baseApiUrl}/CekimTalepleri/get-toplam-cekim-tutari`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<ToplamCekimTutariDto[]>>(url, { params });
    }
}