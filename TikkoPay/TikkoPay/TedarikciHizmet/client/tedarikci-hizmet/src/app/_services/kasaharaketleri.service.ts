import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { KasaHaraketleriListDto } from '../_models/kasaHaraketleri/kasaHaraketleriList.dto';
import { KasaHaraketleriFilter, KasaHaraketleriPagingFilter } from '../_models/kasaHaraketleri/kasaHaraketleriFilter';
import { KasaHaraketleriCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriCreate.dto';
import { KasaHaraketleriUpdateDto } from '../_models/kasaHaraketleri/kasaHaraketleriUpdate.dto';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { ToplamKasaHaraketiGirisDto } from '../_models/kasaHaraketleri/toplamKasaHaraketiGiris.dto';
import { ToplamKasaHaraketiCikisDto } from '../_models/kasaHaraketleri/toplamKasaHaraketiCikis.dto';

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

    getToplamKasaHaraketiGiris(){
        const url: string = `${this.baseApiUrl}/kasaharaketleri/get-toplam-kasa-haraketi-giris`;
        return this.httpClient.get<ResponseViewModel<ToplamKasaHaraketiGirisDto[]>>(url);
    }

    getToplamKasaHaraketiCikis(){
        const url: string = `${this.baseApiUrl}/kasaharaketleri/get-toplam-kasa-haraketi-cikis`;
        return this.httpClient.get<ResponseViewModel<ToplamKasaHaraketiCikisDto[]>>(url);
    }

    getAllWithPaging(filterModel: KasaHaraketleriPagingFilter): Observable<ResponseViewModelPaging<KasaHaraketleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/kasaharaketleri`;
        let params = new HttpParams();

        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
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

    exportToExcel(filterModel: KasaHaraketleriFilter): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/kasaharaketleri/exportToExcel`;
        let params = new HttpParams();

        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
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

    create(item: KasaHaraketleriCreateDto) {
        return this.httpClient.post<ResponseViewModel<KasaHaraketleriCreateDto>>(`${this.baseApiUrl}/kasaharaketleri`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/kasaharaketleri/${id}`);
    }

}