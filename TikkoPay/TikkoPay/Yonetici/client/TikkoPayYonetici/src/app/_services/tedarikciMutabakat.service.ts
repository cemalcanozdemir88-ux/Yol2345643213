import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { TedarikciMutabakatlariListDto } from '../_models/tedarikciMutabakat/tedarikciMutabakatList.dto';
import { TedarikciMutabakatFilter } from '../_models/tedarikciMutabakat/tedarikciMutabakatFilter';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { TedarikciMutabakatCalculateDto } from '../_models/tedarikciMutabakat/tedarikciMutabakatCalculate.dto';

@Injectable({
    providedIn: 'root'
})
export class TedarikciMutabakatService {

    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    getAll(filterModel: TedarikciMutabakatFilter): Observable<ResponseViewModel<TedarikciMutabakatlariListDto[]>> {

        const url: string = `${this.baseApiUrl}/tedarikcimutabakat`;
        let params = new HttpParams();

        if (filterModel.tedarikciId) {
            params = params.set('tedarikciId', filterModel.tedarikciId.toString());
        }

        params = params.set('mutabakatTarihi', filterModel.mutabakatTarihi.toString());

        return this.httpClient.get<ResponseViewModel<TedarikciMutabakatlariListDto[]>>(url, { params });
    }

    calculate(tedarikciMutabakatCalculateModel : TedarikciMutabakatCalculateDto){
        return this.httpClient.post<ResponseViewModel<TedarikciMutabakatCalculateDto>>(`${this.baseApiUrl}/tedarikcimutabakat`, tedarikciMutabakatCalculateModel);
    }
}