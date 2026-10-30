import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environment';
import { ResponseViewModel } from '../_models/responseviewmodel';
import { LoginRequest } from '../_models/kullanicilar/loginRequest';
import { JwtHelperService } from '@auth0/angular-jwt';
import { map } from 'rxjs/operators';
import { LoginOtpResponseDto } from '../_models/auth/loginOtpResponse.dto';
import { TwoFactorAuthSetupResponseDto } from '../_models/auth/twoFactorAuthSetupResponse.dto';
import { ValidateOtpCodeRequest } from '../_models/auth/validateOtpCodeRequest';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    baseApiUrl: string = environment.baseApiUrl;
    jwtHelper = new JwtHelperService();
    decodedToken: any;

    constructor(private httpClient: HttpClient) {

    }

    login(item: LoginRequest): Observable<any> {
        return this.httpClient.post(`${this.baseApiUrl}/auth/login`, item).pipe(
            map((response: any) => {
                localStorage.setItem("token", response.token);
                this.decodedToken = this.jwtHelper.decodeToken(response.token);
            })
        );
    }

    loginOtp(item: LoginRequest): Observable<ResponseViewModel<LoginOtpResponseDto>> {
        return this.httpClient.post<ResponseViewModel<LoginOtpResponseDto>>(`${this.baseApiUrl}/auth/login-otp`, item);
    }

    getOtpBarcode(id: string): Observable<ResponseViewModel<TwoFactorAuthSetupResponseDto>> {
        return this.httpClient.get<ResponseViewModel<TwoFactorAuthSetupResponseDto>>(`${this.baseApiUrl}/auth/get-otp-barcode/${id}`);
    }

    sendOtpCode(item: ValidateOtpCodeRequest): Observable<any>{

        return this.httpClient.post(`${this.baseApiUrl}/auth/validate-code`, item).pipe(
            map((response: any) => {
                localStorage.setItem("token", response.token);
                this.decodedToken = this.jwtHelper.decodeToken(response.token);
            })
        );
    }

    loggedIn() {
        const token = localStorage.getItem("token");
        return !this.jwtHelper.isTokenExpired(token);
    }

    logout() {
        localStorage.removeItem("token");
    }

    setDecodedToken() {
        const token = localStorage.getItem("token");
        if (token) {
            this.decodedToken = this.jwtHelper.decodeToken(token);
        }
        else {
            console.error("Token not found or null");
        }
    }

    getName(): string {
        return this.decodedToken.name;
    }

    getUseriId(): string {
        return this.decodedToken.nameidentifier;
    }

    getTedarikciId(): string {
        return this.decodedToken.tedarikciId;
    }

    getWebsiteId(): string {
        return this.decodedToken.websiteId;
    }

    getRole(): string {
        return this.decodedToken.role;
    }

    isUserInRoles(requiredRoles: string[]): boolean {
        const userRole = this.getRole();
        return requiredRoles.includes(userRole);
    }

    userHasAuthorization(authorizationId: string): boolean {
        const userRoles: string[] = this.decodedToken.roles || [];
        return userRoles.includes(authorizationId);
    }


}