import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { AffiliateDto } from "../_models/affiliate/affiliate.dto";
import { AffiliateCreateDto } from "../_models/affiliate/affiliateCreate.dto";
import { AffiliateBakiyeListDto } from "../_models/affiliate/affiliateBakiyeList.dto";

@Injectable({
    providedIn: 'root'
})
export class AffiliateService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<AffiliateDto>> {
        return this.httpClient.get<ResponseViewModel<AffiliateDto>>(`${this.baseApiUrl}/affiliates/${id}`);
    }

    getAll(): Observable<ResponseViewModel<AffiliateDto[]>> {
        return this.httpClient.get<ResponseViewModel<AffiliateDto[]>>(`${this.baseApiUrl}/affiliates/get-all`);
    }

    getBakiyeList(affiliateId: string, websiteId: string): Observable<ResponseViewModel<AffiliateBakiyeListDto[]>> {
        const url: string = `${this.baseApiUrl}/affiliates/get-bakiyelist`;
        let params = new HttpParams();

        if (affiliateId) {
            params = params.set('affiliateId', affiliateId);
        }

        if (websiteId) {
            params = params.set('websiteId', websiteId);
        }

        return this.httpClient.get<ResponseViewModel<AffiliateBakiyeListDto[]>>(url, { params });
    }

    create(affiliate: AffiliateCreateDto) {
        return this.httpClient.post<ResponseViewModel<AffiliateCreateDto>>(`${this.baseApiUrl}/affiliates/create`, affiliate);
    }

    update(affiliate: AffiliateCreateDto) {
        return this.httpClient.put<ResponseViewModel<AffiliateCreateDto>>(`${this.baseApiUrl}/affiliates/update`, affiliate);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/affiliates/${id}`);
    }
}