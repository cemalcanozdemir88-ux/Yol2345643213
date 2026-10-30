import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { Observable } from "rxjs";
import { WebSiteTedarikcileriManualCreateDto } from "../_models/websitetedarikcileriManual/webSiteTedarikcileriManualCreate.dto";
import { WebSiteTedarikcileriManualListDto } from "../_models/websitetedarikcileriManual/webSiteTedarikcileriManualList.dto";


@Injectable({
    providedIn: 'root'
})
export class WebSiteTedarikcileriManualService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getByWebSiteId(webSiteId: string): Observable<ResponseViewModel<WebSiteTedarikcileriManualListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteTedarikcileriManualListDto[]>>(`${this.baseApiUrl}/websitetedarikcilerimanual/${webSiteId}`);
    }

    create(item: WebSiteTedarikcileriManualCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteTedarikcileriManualCreateDto>>(`${this.baseApiUrl}/websitetedarikcilerimanual/create`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websitetedarikcilerimanual/${id}`);
    }
}