import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { BankaListDto } from '../_models/banka/bankaList.dto';

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
}