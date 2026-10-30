import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { WebSiteTedarikcileriCreateDto } from "../_models/websitetedarikcileri/webSiteTedarikcileriCreate.dto";
import { Observable } from "rxjs";
import { WebSiteTedarikcileriListDto } from "../_models/websitetedarikcileri/webSiteTedarikcileriList.dto";


@Injectable({
    providedIn: 'root'
})
export class WebSiteTedarikcileriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getByWebSiteId(webSiteId: string): Observable<ResponseViewModel<WebSiteTedarikcileriListDto[]>> {
        return this.httpClient.get<ResponseViewModel<WebSiteTedarikcileriListDto[]>>(`${this.baseApiUrl}/websitetedarikcileri/${webSiteId}`);
    }

    create(item: WebSiteTedarikcileriCreateDto) {
        return this.httpClient.post<ResponseViewModel<WebSiteTedarikcileriCreateDto>>(`${this.baseApiUrl}/websitetedarikcileri/create`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/websitetedarikcileri/${id}`);
    }
}