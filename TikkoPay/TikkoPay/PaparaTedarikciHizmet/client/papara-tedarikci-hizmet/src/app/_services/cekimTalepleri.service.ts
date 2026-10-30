import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { Durum } from '../_enums/durum.enum';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { IslemTip } from '../_enums/islemTip.Enum';
import { ToplamCekimTutariDto } from '../_models/yatirimTalepleri/toplamCekimTutari.dto';

@Injectable({
  providedIn: 'root',
})
export class CekimTalepleriService {
  baseApiUrl: string = environment.baseApiUrl;

  constructor(private httpClient: HttpClient) {}

  getById(id: string): Observable<ResponseViewModel<YatirimTalepleriListDto>> {
    return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto>>(
      `${this.baseApiUrl}/CekimTalepleri/${id}`
    );
  }

  getParams(filterModel: YatirimTalepleriFilter): HttpParams {
    let params = new HttpParams();
    
    if (filterModel.bankaId) {
      params = params.set('bankaId', filterModel.bankaId.toString());
    }
    if (filterModel.approveDateStart) {
      params = params.set('approveDateStart', filterModel.approveDateStart);
    }
    if (filterModel.approveDateEnd) {
      params = params.set('approveDateEnd', filterModel.approveDateEnd);
    }
    if (filterModel.createdDateStart) {
      params = params.set('createdDateStart', filterModel.createdDateStart);
    }
    if (filterModel.createdDateEnd) {
      params = params.set('createdDateEnd', filterModel.createdDateEnd);
    }

    params = params.set('searchText', filterModel.searchText);

    return params;
  }

  exportToExcel(filterModel: YatirimTalepleriFilter, islemTipi: IslemTip, durum: Durum): Observable<ResponseViewModel<ExcelExportDto>> {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/exportToExcel`;
    let params = this.getParams(filterModel);

    params = params.set('islemTipi', islemTipi);
    params = params.set('durum', durum);

    return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
  }

  getBekleyenCekimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/bekleyen-cekim-talepleri`;
    let params = this.getParams(filterModel);

    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  getOnaylananCekimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/onaylanan-cekim-talepleri`;
    let params = this.getParams(filterModel);

    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  getRedCekimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/red-cekim-talepleri`;
    let params = this.getParams(filterModel);

    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  getToplamCekimTutari(){
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/get-toplam-cekim-tutari`;
    return this.httpClient.get<ResponseViewModel<ToplamCekimTutariDto[]>>(url);
  }

  getToplamCekimTutariWithFilter(filterModel: YatirimTalepleriFilter){
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/get-toplam-cekim-tutari`;
    let params = this.getParams(filterModel);

    return this.httpClient.get<ResponseViewModel<ToplamCekimTutariDto[]>>(url, { params });
  }

  approveItem(id: string, onayAciklama: string) {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/approve?id=${id}`;
    const body = { id: id, onayAciklama: onayAciklama };
    return this.httpClient.put<ResponseViewModel<YatirimTalepleriListDto>>(url, body);
  }

  cancelItem(id: string, iptalAciklama: string) {
    const url: string = `${this.baseApiUrl}/CekimTalepleri/cancel?id=${id}`;
    const body = { id: id, iptalAciklama: iptalAciklama };
    return this.httpClient.put<ResponseViewModel<YatirimTalepleriListDto>>(url, body);
  }
}
