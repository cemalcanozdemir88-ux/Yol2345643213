import { Component } from '@angular/core';
import { AffiliateDto } from '../_models/affiliate/affiliate.dto';
import { AffiliateBakiyeListDto } from '../_models/affiliate/affiliateBakiyeList.dto';
import { AffiliateService } from '../_services/affiliate.service';
import { AlertifyService } from '../_services/alertify.service';
import { WebSiteleriListDto } from '../_models/websiteleri/websiteleriList.dto';
import { WebSiteleriService } from '../_services/websiteleri.service';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-affiliate-bakiye',
  templateUrl: './affiliate-bakiye.component.html',
  styleUrls: ['./affiliate-bakiye.component.css']
})
export class AffiliateBakiyeComponent {

  affiliateDropDownModel: AffiliateDto[] = [];
  affiliateBakiyeList : AffiliateBakiyeListDto[] = [];
  webSiteDropDownModel: WebSiteleriListDto[] = [];

  selectedAffiliate: string = "";
  selectedWebsiteId: string = "";

  isAffiliateKullanicisi: boolean = false;

  constructor(
    private webSiteService: WebSiteleriService,
    private affiliateService: AffiliateService,
    private authServcie: AuthService,
    private alertify: AlertifyService) {

  }

  ngOnInit(): void {
    this.getAffiliates();
    this.getAll();
  }

  filterChanged() {
    this.getAll();
  }

  getAffiliates() {
    this.affiliateService.getAll().subscribe((reponse) => {
      this.affiliateDropDownModel = reponse.result;

      if(this.authServcie.isAffiliateKullanicisi()){
        this.isAffiliateKullanicisi = true;
        this.selectedAffiliate = this.authServcie.getAffiliateId();
        this.getAffiliateWebsites();
      }

    });
  }

  getAffiliateWebsites(){
    if(this.selectedAffiliate){
      this.webSiteService.getSelectListByAffiliate(this.selectedAffiliate).subscribe((reponse) => {
        this.webSiteDropDownModel = reponse.result;
      });
    }
  }

  getAll() {
    this.affiliateService.getBakiyeList(this.selectedAffiliate, this.selectedWebsiteId).subscribe((response) => {
      if (response.success) {
        this.affiliateBakiyeList = response.result;
      }
      else {
        this.alertify.error(response.message);
      }
    })
  }
}
