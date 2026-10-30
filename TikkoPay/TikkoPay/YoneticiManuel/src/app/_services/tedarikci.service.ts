import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { TedarikcilerListDto } from '../_models/tedarikci/tedarikcilerList.dto';
import { TedarikciCreateDto } from '../_models/tedarikci/tedarikciCreate.dto';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { TedarikciItemDto } from '../_models/tedarikci/tedarikciItem.dto';
import { TedarikciBakiyeListDto } from '../_models/tedarikci/tedarikciBakiyetList.dto';
import { TedarikcilerSelectListWithBakiyeDto } from '../_models/tedarikci/tedarikcilerSelectListWithBakiye.dto';
import { TedarikcilerPagingFilter } from '../_models/tedarikci/tedarikcilerFilter';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';
import { TedarikcilerSelectList } from '../_models/tedarikci/tedarikcilerSelectList';

@Injectable({
    providedIn: 'root'
})
export class TedarikciService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<TedarikciItemDto>> {
        return this.httpClient.get<ResponseViewModel<TedarikciItemDto>>(`${this.baseApiUrl}/tedarikciler/${id}`);
    }

    getAllWithPaging(filterModel: TedarikcilerPagingFilter): Observable<ResponseViewModelPaging<TedarikcilerListDto[]>> {
        const url: string = `${this.baseApiUrl}/tedarikciler/get-all-with-paging`;
        let params = new HttpParams();

        if(filterModel.bankaHavale)
            params = params.set('bankaHavale', filterModel.bankaHavale.toString());

        if(filterModel.otoPuan)
            params = params.set('otoPuan', filterModel.otoPuan.toString());


        if(filterModel.durum)
            params = params.set('durum', filterModel.durum.toString());

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<TedarikcilerListDto[]>>(url, { params });
    }

    getAll(): Observable<ResponseViewModel<TedarikcilerListDto[]>> {
        return this.httpClient.get<ResponseViewModel<TedarikcilerListDto[]>>(`${this.baseApiUrl}/tedarikciler/get-all`);
    }

    getSelectList(): Observable<ResponseViewModel<TedarikcilerSelectList[]>> {
        return this.httpClient.get<ResponseViewModel<TedarikcilerSelectList[]>>(`${this.baseApiUrl}/tedarikciler/get-select-list`);
    }

    getSelectListWithBakiye(): Observable<ResponseViewModel<TedarikcilerSelectListWithBakiyeDto[]>> {
        return this.httpClient.get<ResponseViewModel<TedarikcilerSelectListWithBakiyeDto[]>>(`${this.baseApiUrl}/tedarikciler/get-select-list-with-bakiye`);
    }

    getBakiyeList(tedarikciId: string, tedarikciDurum: number): Observable<ResponseViewModel<TedarikciBakiyeListDto[]>> {
        const url: string = `${this.baseApiUrl}/tedarikciler/get-bakiyelist`;
        let params = new HttpParams();

        if (tedarikciId) {
            params = params.set('tedarikciId', tedarikciId);
        }

        params = params.set('tedarikciDurum', tedarikciDurum);

        return this.httpClient.get<ResponseViewModel<TedarikciBakiyeListDto[]>>(url, { params });
    }

    create(tedarikci: TedarikciCreateDto) {
        return this.httpClient.post<ResponseViewModel<TedarikciCreateDto>>(`${this.baseApiUrl}/tedarikciler/create`, tedarikci);
    }

    update(tedarikci: TedarikciCreateDto) {
        return this.httpClient.put<ResponseViewModel<TedarikciCreateDto>>(`${this.baseApiUrl}/tedarikciler/update`, tedarikci);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/tedarikciler/${id}`);
    }
}