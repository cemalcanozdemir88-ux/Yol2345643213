import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { BankaListDto } from '../_models/banka/bankaList.dto';
import { BankaCreateDto } from '../_models/banka/bankaCreate.dto';
import { BankaUpdateDto } from '../_models/banka/bankaUpdate.dto';

@Injectable({
    providedIn: 'root'
})
export class BankaService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getById(id: string): Observable<ResponseViewModel<BankaListDto>> {
        return this.httpClient.get<ResponseViewModel<BankaListDto>>(`${this.baseApiUrl}/banka/${id}`);
    }

    getAll(): Observable<ResponseViewModel<BankaListDto[]>> {
        return this.httpClient.get<ResponseViewModel<BankaListDto[]>>(`${this.baseApiUrl}/banka`);
    }

    create(banka: BankaCreateDto) {
        return this.httpClient.post<ResponseViewModel<BankaCreateDto>>(`${this.baseApiUrl}/banka`, banka);
    }

    update(banka: BankaUpdateDto) {
        return this.httpClient.put<ResponseViewModel<BankaUpdateDto>>(`${this.baseApiUrl}/banka`, banka);
    }

    delete(id: string): Observable<any> {
        return this.httpClient.delete(`${this.baseApiUrl}/banka/${id}`);
    }
}