import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteHavaleBankalariCreateDto, WebSiteHavaleBankalariListDto } from "../_models/websitehavalebankalari/webSiteHavaleBankalari.dto";

@Injectable({
    providedIn: 'root'
})
export class WebSiteHavaleBankalariService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {
    }

    getByWebSiteId(webSiteId: string): Observable<ResponseViewModel<WebSiteHavaleBankalariListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteHavaleBankalariListDto[]>>(`${this.baseApiUrl}/websitehavalebankalari/${webSiteId}`);
    }

    create(item: WebSiteHavaleBankalariCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteHavaleBankalariCreateDto>>(`${this.baseApiUrl}/websitehavalebankalari/create`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websitehavalebankalari/${id}`);
    }
}
