import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { KullanicilarListDto } from '../_models/kullanicilar/kullanicilarList.dto';
import { KullanicilarCreateDto } from '../_models/kullanicilar/kullanicilarCreate.dto';
import { KullanicilarUpdateDto } from '../_models/kullanicilar/kullanicilarUpdate.dto';
import { KullanicilarFilter } from '../_models/kullanicilar/kullanicilarFilter';

@Injectable({
    providedIn: 'root'
})
export class KullanicilarService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<KullanicilarListDto>> {
        return this.httpClient.get<ResponseViewModel<KullanicilarListDto>>(`${this.baseApiUrl}/kullanicilar/${id}`);
    }

    getAll(filterModel: KullanicilarFilter): Observable<ResponseViewModel<KullanicilarListDto[]>> {
        const url: string = `${this.baseApiUrl}/kullanicilar`;
        let params = new HttpParams();

        if (filterModel.kullaniciTipi) {
            params = params.set('kullaniciTipi', filterModel.kullaniciTipi.toString());
        }

        return this.httpClient.get<ResponseViewModel<KullanicilarListDto[]>>(url, { params });
    }

    create(item: KullanicilarCreateDto) {
        return this.httpClient.post<ResponseViewModel<KullanicilarCreateDto>>(`${this.baseApiUrl}/kullanicilar`, item);
    }

    update(item: KullanicilarUpdateDto) {
        return this.httpClient.put<ResponseViewModel<KullanicilarUpdateDto>>(`${this.baseApiUrl}/kullanicilar`, item);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/kullanicilar/${id}`);
    }
}