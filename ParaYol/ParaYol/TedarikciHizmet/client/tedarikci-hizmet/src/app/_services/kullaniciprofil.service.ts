import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environment";
import { ChangePasswordDto } from "../_models/kullanicilar/changePassword.dto";
import { ResponseViewModel } from "../_models/responseviewmodel";

@Injectable({
    providedIn: 'root'
})
export class KullaniciProfileService {
    baseApiUrl: string = environment.baseApiUrl;

    constructor(private httpClient: HttpClient) {

    }

    changePassword(item: ChangePasswordDto) {
        return this.httpClient.post<ResponseViewModel<ChangePasswordDto>>(`${this.baseApiUrl}/kullaniciProfil/change-password`, item);
    }
}