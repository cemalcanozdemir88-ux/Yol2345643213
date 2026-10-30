export interface TumCekimTalepleriFilter {
    tedarikciId: string;
    bankaId: string;
    webSiteId: string;
    searchText: string;
    durum: string;
    dekontKontrolDurum: string;
    createdDateStart: string;
    createdDateEnd: string;
    pageNumber: number;
    recordsPerPage: number;
}