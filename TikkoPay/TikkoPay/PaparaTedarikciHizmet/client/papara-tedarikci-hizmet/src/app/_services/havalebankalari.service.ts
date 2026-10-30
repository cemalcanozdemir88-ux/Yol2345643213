import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { HavaleBankalariListDto } from '../_models/havalebankalari/havalebankalariList.dto';
import { HavaleBankalariCreateDto } from '../_models/havalebankalari/havalebankalariCreate.dto';
import { HavaleBankalariUpdateDto } from '../_models/havalebankalari/havalebankalariUpdate.dto';
import { HavaleBankalariFilter, HavaleBankalariPagingFilter } from '../_models/havalebankalari/havalebankalariFilter';
import { ResponseViewModelPaging } from '../_models/responseViewModelPaging';

@Injectable({
    providedIn: 'root'
})
export class HavaleBankalariService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<HavaleBankalariListDto>> {
        return this.httpClient.get<ResponseViewModel<HavaleBankalariListDto>>(`${this.baseApiUrl}/havalebankalari/${id}`);
    }

    getAllWithPaging(filterModel: HavaleBankalariPagingFilter): Observable<ResponseViewModelPaging<HavaleBankalariListDto[]>> {
        const url: string = `${this.baseApiUrl}/havalebankalari`;
        let params = new HttpParams();

        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
        }
        if (filterModel.bankaId) {
            params = params.set('bankaId', filterModel.bankaId.toString());
        }

        params = params.set('searchText', filterModel.searchText);
        params = params.set('havaleBankalariDurumFilter', filterModel.havaleBankalariDurumFilter);

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<HavaleBankalariListDto[]>>(url, { params });
    }

    create(banka: HavaleBankalariCreateDto) {
        return this.httpClient.post<ResponseViewModel<HavaleBankalariCreateDto>>(`${this.baseApiUrl}/havalebankalari`, banka);
    }

    update(banka: HavaleBankalariUpdateDto) {
        return this.httpClient.put<ResponseViewModel<HavaleBankalariUpdateDto>>(`${this.baseApiUrl}/havalebankalari`, banka);
    }
}