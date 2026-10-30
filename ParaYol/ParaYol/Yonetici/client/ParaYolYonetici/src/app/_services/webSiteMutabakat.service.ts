import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { WebSiteMutabakatFilter } from '../_models/webSiteMutabakat/webSiteMutabakatFilter';
import { WebSiteMutabakatlariListDto } from '../_models/webSiteMutabakat/webSiteMutabakatList.dto';
import { WebsiteMutabakatCalculateDto } from '../_models/webSiteMutabakat/webSiteMutabakatCalculate.dto';

@Injectable({
    providedIn: 'root'
})
export class WebSiteMutabakatService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getAll(filterModel: WebSiteMutabakatFilter): Observable<ResponseViewModel<WebSiteMutabakatlariListDto[]>> {

        const url: string = `${this.baseApiUrl}/websitemutabakat`;
        let params = new HttpParams();

        if (filterModel.webSiteId) {
            params = params.set('webSiteId', filterModel.webSiteId.toString());
        }

        params = params.set('mutabakatTarihi', filterModel.mutabakatTarihi.toString());

        return this.httpClient.get<ResponseViewModel<WebSiteMutabakatlariListDto[]>>(url, { params });
    }

    calculate(webSiteMutabakatCalculateModel: WebsiteMutabakatCalculateDto) {
        return this.httpClient.post<ResponseViewModel<WebsiteMutabakatCalculateDto>>(`${this.baseApiUrl}/websitemutabakat`, webSiteMutabakatCalculateModel);
    }
}