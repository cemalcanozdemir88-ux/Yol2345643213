import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { KullaniciIpAdresleriFilter } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriFilter';
import { KullaniciIpAdresleriListDto } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriList.dto';
import { KullaniciIpAdresleriCreateDto } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriCreate.dto';
import { KullaniciIpAdresleriUpdateDto } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriUpdate.dto';
import { KullaniciIpAdresleriWithAvailableCount } from '../_models/kullaniciIpAdresleri/kullaniciIpAdresleriWithAvailableCount.dto';
import { UpdateIpListDto } from '../_models/kullaniciIpAdresleri/updateIpList.dto';

@Injectable({
    providedIn: 'root'
})
export class KullaniciIpAdresleriService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<KullaniciIpAdresleriListDto>> {
        return this.httpClient.get<ResponseViewModel<KullaniciIpAdresleriListDto>>(`${this.baseApiUrl}/kullaniciipadresleri/${id}`);
    }

    getAll(filterModel: KullaniciIpAdresleriFilter): Observable<ResponseViewModel<KullaniciIpAdresleriListDto[]>> {
        const url: string = `${this.baseApiUrl}/kullaniciipadresleri`;
        let params = new HttpParams();

        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
        }
        
        params = params.set('searchText', filterModel.searchText);

        return this.httpClient.get<ResponseViewModel<KullaniciIpAdresleriListDto[]>>(url, { params });
    }

    getByKullaniciId(kullaniciId: string): Observable<ResponseViewModel<KullaniciIpAdresleriWithAvailableCount>> {
        return this.httpClient.get<ResponseViewModel<KullaniciIpAdresleriWithAvailableCount>>(`${this.baseApiUrl}/kullaniciipadresleri/get-by-kullaniciId?kullaniciId=${kullaniciId}`);
    }

    create(kullaniciIpAdresleri: KullaniciIpAdresleriCreateDto) {
        return this.httpClient.post<ResponseViewModel<KullaniciIpAdresleriCreateDto>>(`${this.baseApiUrl}/kullaniciipadresleri`, kullaniciIpAdresleri);
    }

    update(kullaniciIpAdresleri: KullaniciIpAdresleriUpdateDto) {
        return this.httpClient.put<ResponseViewModel<KullaniciIpAdresleriUpdateDto>>(`${this.baseApiUrl}/kullaniciipadresleri`, kullaniciIpAdresleri);
    }

    updateIpList(updateIpListDto: UpdateIpListDto) {
        return this.httpClient.put<ResponseViewModel<KullaniciIpAdresleriUpdateDto>>(`${this.baseApiUrl}/kullaniciipadresleri/update-ip-list`, updateIpListDto);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/kullaniciipadresleri/${id}`);
    }
}