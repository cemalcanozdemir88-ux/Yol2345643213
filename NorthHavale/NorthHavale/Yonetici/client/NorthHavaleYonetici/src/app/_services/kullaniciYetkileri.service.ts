import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environment";
import { ResponseViewModel } from "../_models/responseviewmodel";
import { KullaniciYetkileriListDto } from "../_models/kullaniciYetkileri/kullaniciYetkileriList.dto";
import { KullaniciYetkileriCreateDto } from "../_models/kullaniciYetkileri/kullaniciYetkileriCreate.dto";
import { KullaniciYetkileriDeleteDto } from "../_models/kullaniciYetkileri/kullaniciYetkileriDeletedto";


@Injectable({
    providedIn: 'root'
})
export class KullaniciYetkileriService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getByKullaniciId(kullaniciId: string): Observable<ResponseViewModel<KullaniciYetkileriListDto>> {
        return this.httpClient.get<ResponseViewModel<KullaniciYetkileriListDto>>(`${this.baseApiUrl}/kullaniciyetkileri/get-by-kullaniciId?kullaniciId=${kullaniciId}`);
    }

    create(item: KullaniciYetkileriCreateDto) {
        return this.httpClient.post<ResponseViewModel<KullaniciYetkileriCreateDto>>(`${this.baseApiUrl}/kullaniciyetkileri`, item);
    }

    delete(item: KullaniciYetkileriDeleteDto) {
        return this.httpClient.post<ResponseViewModel<KullaniciYetkileriDeleteDto>>(`${this.baseApiUrl}/kullaniciyetkileri/delete`, item);
    }
}