import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ExcelExportDto } from "../_models/excelExport.dto";
import { BakiyeListDto } from "../_models/kasaHaraketleri/bakiyeList.dto";
import { KasaHaraketleriCreateDto } from "../_models/kasaHaraketleri/kasaHaraketleriCreate.dto";
import { KasaHaraketleriFilter, KasaHaraketleriPagingFilter } from "../_models/kasaHaraketleri/kasaHaraketleriFilter";
import { KasaHaraketleriListDto } from "../_models/kasaHaraketleri/kasaHaraketleriList.dto";
import { KasaHaraketleriUpdateDto } from "../_models/kasaHaraketleri/kasaHaraketleriUpdate.dto";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { ResponseViewModelPaging } from "../_models/responseViewModelPaging";


@Injectable({
    providedIn: 'root'
})
export class KasaHaraketleriService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<KasaHaraketleriListDto>> {
        return this.httpClient.get<ResponseViewModel<KasaHaraketleriListDto>>(`${this.baseApiUrl}/kasaharaketleri/${id}`);
    }

    getAll(filterModel: KasaHaraketleriFilter): Observable<ResponseViewModel<KasaHaraketleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/kasaharaketleri`;
        let params = new HttpParams();

        if (filterModel.durum) {
            params = params.set('durum', parseInt(filterModel.durum));
        }

        if (filterModel.createdDateStart) {
            params = params.set('createdDateStart', (filterModel.createdDateStart));
        }

        if (filterModel.createdDateEnd) {
            params = params.set('createdDateEnd', (filterModel.createdDateEnd));
        }

        return this.httpClient.get<ResponseViewModelPaging<KasaHaraketleriListDto[]>>(url, { params });
    }

    getAllWithPaging(filterModel: KasaHaraketleriPagingFilter): Observable<ResponseViewModelPaging<KasaHaraketleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/kasaharaketleri`;
        let params = new HttpParams();

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

    exportToExcel(filterModel: KasaHaraketleriFilter): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/kasaharaketleri/exportToExcel`;
        let params = new HttpParams();

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

    getBakiyeList(): Observable<ResponseViewModel<BakiyeListDto[]>> {
        const url: string = `${this.baseApiUrl}/kasaharaketleri/get-bakiyelist`;
        let params = new HttpParams();

        return this.httpClient.get<ResponseViewModel<BakiyeListDto[]>>(url, { params });
    }

    create(item: KasaHaraketleriCreateDto) {
        return this.httpClient.post<ResponseViewModel<KasaHaraketleriCreateDto>>(`${this.baseApiUrl}/kasaharaketleri`, item);
    }

    update(item: KasaHaraketleriUpdateDto) {
        return this.httpClient.put<ResponseViewModel<KasaHaraketleriUpdateDto>>(`${this.baseApiUrl}/kasaharaketleri`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/kasaharaketleri/${id}`);
    }

}