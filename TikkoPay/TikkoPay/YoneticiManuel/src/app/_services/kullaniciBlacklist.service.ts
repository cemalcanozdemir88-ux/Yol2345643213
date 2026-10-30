import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment";
import { KullaniciBlacklistFilter } from "../_models/kullaniciBlacklist/kullaniciBlacklistFilter.dto";
import { KullaniciBlacklistDto } from "../_models/kullaniciBlacklist/kullaniciBlacklist.dto";
import { Observable } from "rxjs";
import { ResponseViewModelPaging } from "../_models/responseViewModelPaging";
import { KullaniciBlacklistCreateDto } from "../_models/kullaniciBlacklist/kullaniciBlacklistCreate.dto";
import { ResponseViewModel } from "../_models/responseviewmodel";

@Injectable({
    providedIn: 'root'
})
export class KullaniciBlacklistService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getAllWithPaging(filterModel: KullaniciBlacklistFilter): Observable<ResponseViewModelPaging<KullaniciBlacklistDto[]>> {
        const url: string = `${this.baseApiUrl}/kullaniciblacklist`;
        let params = new HttpParams();

        if (filterModel.websiteId) {
            params = params.set('websiteId', filterModel.websiteId.toString());
        }

        if (filterModel.userId) {
            params = params.set('userId', filterModel.userId.toString());
        }

        params = params.set('pageNumber', (filterModel.pageNumber));
        params = params.set('recordsPerPage', (filterModel.recordsPerPage));

        return this.httpClient.get<ResponseViewModelPaging<KullaniciBlacklistDto[]>>(url, { params });
    }

    create(item: KullaniciBlacklistCreateDto) {
        return this.httpClient.post<ResponseViewModel<KullaniciBlacklistCreateDto>>(`${this.baseApiUrl}/kullaniciblacklist`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/kullaniciblacklist/${id}`);
    }
}