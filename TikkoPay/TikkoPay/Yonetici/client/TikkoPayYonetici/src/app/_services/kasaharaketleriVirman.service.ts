import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environment';
import { ResponseViewModel } from "../_models/responseviewmodel";
import { KasaHaraketleriVirmanCreateDto } from '../_models/kasaHaraketleri/kasaHaraketleriVirmanCreate.dto';

@Injectable({
    providedIn: 'root'
})
export class KasaHaraketleriVirmanService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    create(item: KasaHaraketleriVirmanCreateDto) {
        return this.httpClient.post<ResponseViewModel<KasaHaraketleriVirmanCreateDto>>(`${this.baseApiUrl}/kasaharaketlerivirman`, item);
    }
}