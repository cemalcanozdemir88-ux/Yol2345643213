import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Directive({
    selector: '[appHasAuthorization]'
})
export class HasAuthorizationDirective {
    constructor(
        private authService: AuthService,
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) { }

    @Input() set appHasAuthorization(authorizationId: string) {
        if (this.authService.userHasAuthorization(authorizationId)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        } else {
            this.viewContainer.clear();
        }
    }
}