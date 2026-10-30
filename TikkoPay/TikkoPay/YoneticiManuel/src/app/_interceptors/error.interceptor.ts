import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private alertify: AlertifyService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {

        let errorMsg = 'Bilinmeyen bir hata oluştu.';

        if (error.error && error.error.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }

        this.alertify.error(errorMsg);
        return throwError(() => error);
      })
    );
  }
}
