import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { IslemTip } from '../_models/yatirimTalepleri/islemTip.Enum';
import { Durum } from '../_enums/durum.enum';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { ToplamCekimTutariDto } from '../_models/cekimTalepleri/toplamCekimTutari.dto';
import { CekimCreateDto } from '../_models/cekimTalepleri/cekimCreate.dto';
import { MultiTedarikciYonlendirmeDto } from '../_models/cekimTalepleri/multi-tedarikci-yonlendirme.dto';
import { CekimTalepleriListDto } from '../_models/cekimTalepleri/cekimTaleperliList.dto';
import { CekimTalepleriFilter, CekimTalepleriPagingFilter } from '../_models/cekimTalepleri/cekimTalepleriFilter';
import { GetDekontDto } from '../_models/cekimTalepleri/getDekont.dto';
import { OrtalamaOnaySuresiDto } from '../_models/yatirimTalepleri/ortalamaOnaySuresi.dto';

@Injectable({
    providedIn: 'root'
})
export class CekimTalepleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getParams(filterModel: CekimTalepleriFilter): HttpParams {
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

    getByTopId(topId: string): Observable<ResponseViewModel<CekimTalepleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/CekimTalepleri/get-by-topid/${topId}`;
        
        return this.httpClient.get<ResponseViewModel<CekimTalepleriListDto[]>>(url);
    }

    getDekont(id: string): Observable<ResponseViewModel<GetDekontDto>> {
        const url: string = `${this.baseApiUrl}/CekimTalepleri/get-dekont/${id}`;
        
        return this.httpClient.get<ResponseViewModel<GetDekontDto>>(url);
    }

    exportToExcel(filterModel: CekimTalepleriFilter, durum: Durum): Observable<ResponseViewModel<ExcelExportDto>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/exportToExcel`;
        let params = this.getParams(filterModel);

        params = params.set('durum', durum);

        return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
    }

    getBekleyenCekimTalepleriWithPaging(filterModel: CekimTalepleriPagingFilter): Observable<ResponseViewModelPaging<CekimTalepleriListDto[]>> {

        const url: string = `${this.baseApiUrl}/CekimTalepleri/bekleyen-cekim-talepleri`;
        let params = this.getParams(filterModel);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<CekimTalepleriListDto[]>>(url, { params });
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

    onaylaVeSiteyeBildir(id: string) {
        const url: string = `${this.baseApiUrl}/CekimTalepleri/onayla-siteye-bildir?id=${id}`;
        return this.httpClient.put<ResponseViewModel<CekimTalepleriListDto>>(url, null);
    }

    iptalEtVeSiteyeBildir(id: string) {
        const url: string = `${this.baseApiUrl}/CekimTalepleri/iptalet-siteye-bildir?id=${id}`;
        return this.httpClient.put<ResponseViewModel<CekimTalepleriListDto>>(url, null);
    }

    multiTedarikciYonlendir(request: MultiTedarikciYonlendirmeDto): Observable<any> {
        const url = `${this.baseApiUrl}/CekimTalepleri/multi-tedarikciye-yonlendir`;
        return this.httpClient.put<ResponseViewModel<CekimTalepleriListDto>>(url, request);
    }

    getToplamCekimTutari(filterModel: CekimTalepleriFilter){
        const url: string = `${this.baseApiUrl}/CekimTalepleri/get-toplam-cekim-tutari`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<ToplamCekimTutariDto[]>>(url, { params });
    }

    getOrtalamaOnaySuresi(filterModel: CekimTalepleriFilter){
        const url: string = `${this.baseApiUrl}/CekimTalepleri/get-ortalama-onay-suresi`;
        let params = this.getParams(filterModel);

        return this.httpClient.get<ResponseViewModel<OrtalamaOnaySuresiDto[]>>(url, { params });
    }

    createCekim(item: CekimCreateDto){
        return this.httpClient.post<ResponseViewModel<CekimCreateDto>>(`${this.baseApiUrl}/CekimTalepleri`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/CekimTalepleri/${id}`);
    }
}