import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, interval } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { YatirimTalepleriListDto } from '../_models/yatirimTalepleri/yatirimTaleperliList.dto';
import { YatirimTalepleriFilter, YatirimTalepleriPagingFilter } from '../_models/yatirimTalepleri/yatirimTalepleriFilter';
import { ExcelExportDto } from '../_models/excelExport.dto';
import { Durum } from '../_enums/durum.enum';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { IslemTip } from '../_enums/islemTip.Enum';
import { ToplamYatirimTutariDto } from '../_models/yatirimTalepleri/toplamYatirimTutari.dto';

@Injectable({
  providedIn: 'root',
})
export class YatirimTalepleriService {

  baseApiUrl: string = environment.baseApiUrl;
  private dataSubject = new Subject<number>();

  constructor(private httpClient: HttpClient) {}

  startPolling() {
    var url = `${this.baseApiUrl}/YatirimTalepleri/get-pending-count`;

    interval(60000).subscribe(() => {
      this.httpClient.get<number>(url).subscribe({
        next: (data) => {
          this.dataSubject.next(data); 
        },
        error: (error) => {
          console.error('API error:', error);
        }
      });
    });
  }

  getDataObservable() {
    return this.dataSubject.asObservable();
  }

  getById(id: string): Observable<ResponseViewModel<YatirimTalepleriListDto>> {
    return this.httpClient.get<ResponseViewModel<YatirimTalepleriListDto>>(
      `${this.baseApiUrl}/YatirimTalepleri/${id}`
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

  getBekleyenYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/bekleyen-yatirim-talepleri`;
    
    let params = this.getParams(filterModel);
    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  exportToExcel(filterModel: YatirimTalepleriFilter, islemTipi: IslemTip, durum: Durum): Observable<ResponseViewModel<ExcelExportDto>> {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/exportToExcel`;
    
    let params = this.getParams(filterModel);
    params = params.set('islemTipi', islemTipi);
    params = params.set('durum', durum);

    return this.httpClient.get<ResponseViewModel<ExcelExportDto>>(url, { params });
  }

  getOnaylananYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/onaylanan-yatirim-talepleri`;
    
    let params = this.getParams(filterModel);
    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  getRedYatirimTalepleriWithPaging(filterModel: YatirimTalepleriPagingFilter): Observable<ResponseViewModelPaging<YatirimTalepleriListDto[]>> {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/red-yatirim-talepleri`;
    
    let params = this.getParams(filterModel);
    params = params.set('pageNumber', filterModel.pageNumber);
    params = params.set('recordsPerPage', filterModel.recordsPerPage);

    return this.httpClient.get<ResponseViewModelPaging<YatirimTalepleriListDto[]>>(url, { params });
  }

  getToplamYatirimTutari(){
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/get-toplam-yatirim-tutari`;
    return this.httpClient.get<ResponseViewModel<ToplamYatirimTutariDto[]>>(url);
  }

  getToplamYatirimTutariWithFilter(filterModel: YatirimTalepleriFilter){
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/get-toplam-yatirim-tutari`;
    let params = this.getParams(filterModel);

    return this.httpClient.get<ResponseViewModel<ToplamYatirimTutariDto[]>>(url, { params });
  }

  approveItem(id: string) {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/approve?id=${id}`;
    return this.httpClient.put<ResponseViewModel<YatirimTalepleriListDto>>(url, null);
  }

  cancelItem(id: string, iptalAciklama: string) {
    const url: string = `${this.baseApiUrl}/YatirimTalepleri/cancel?id=${id}`;
    const body = { id: id, iptalAciklama: iptalAciklama };
    return this.httpClient.put<ResponseViewModel<YatirimTalepleriListDto>>(url, body);
  }
}
