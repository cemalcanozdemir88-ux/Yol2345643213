import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteKasaHaraketleriListDto } from "../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriList.dto";
import { KasaHaraketleriListDto } from "../_models/kasaHaraketleri/kasaHaraketleriList.dto";
import { WebSiteKasaHaraketleriFilter, WebSiteKasaHaraketleriPagingFilter } from "../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriFilter";
import { WebSiteKasaHaraketleriCreateDto } from "../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriCreate.dto";
import { WebSiteKasaHaraketleriUpdateDto } from "../_models/webSiteKasaHaraketleri/webSiteKasaHaraketleriUpdate.dto";
import { ResponseViewModelPaging } from "../_models/responseViewModelPaging";
import { ExcelExportDto } from "../_models/excelExport.dto";


@Injectable({
    providedIn: 'root'
})
export class WebSiteKasaHaraketleriService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<WebSiteKasaHaraketleriListDto>> {
        return this.httpClient.get<ResponseViewModel<WebSiteKasaHaraketleriListDto>>(`${this.baseApiUrl}/websitekasaharaketleri/${id}`);
    }

    getAllWithPaging(filterModel: WebSiteKasaHaraketleriPagingFilter): Observable<ResponseViewModelPaging<WebSiteKasaHaraketleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/websitekasaharaketleri`;
        let params = new HttpParams();

        if (filterModel.webSiteId) {
            params = params.set('webSiteId', filterModel.webSiteId.toString());
        }

        if (filterModel.islemTipi) {
            params = params.set('islemTipi', filterModel.islemTipi);
        }

        if (filterModel.durum) {
            params = params.set('durum', parseInt(filterModel.durum));
        }

        if (filterModel.createdDateStart) {
            params = params.set('createdDateStart', (filterModel.createdDateStart));
        }

        if (filterModel.createdDateEnd) {
            params = params.set('createdDateEnd', (filterModel.createdDateEnd));
        }

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<KasaHaraketleriListDto[]>>(url, { params });
    }

    exportToExcel(filterModel: WebSiteKasaHaraketleriPagingFilter): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/websitekasaharaketleri/exportToExcel`;
        let params = new HttpParams();

        if (filterModel.webSiteId) {
            params = params.set('webSiteId', filterModel.webSiteId.toString());
        }

        if (filterModel.durum) {
            params = params.set('durum', parseInt(filterModel.durum));
        }

        if (filterModel.createdDateStart) {
            params = params.set('createdDateStart', (filterModel.createdDateStart));
        }

        if (filterModel.createdDateEnd) {
            params = params.set('createdDateEnd', (filterModel.createdDateEnd));
        }

        return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
    }

    create(item: WebSiteKasaHaraketleriCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteKasaHaraketleriCreateDto>>(`${this.baseApiUrl}/websitekasaharaketleri`, item);
    }

    update(item: WebSiteKasaHaraketleriUpdateDto) {
        return this.httpClient.put<ResponseViewModel<WebSiteKasaHaraketleriUpdateDto>>(`${this.baseApiUrl}/websitekasaharaketleri`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websitekasaharaketleri/${id}`);
    }
}