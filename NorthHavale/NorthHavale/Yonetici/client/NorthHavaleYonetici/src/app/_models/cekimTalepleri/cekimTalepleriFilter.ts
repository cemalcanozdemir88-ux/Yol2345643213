export interface CekimTalepleriFilter {
    tedarikciId: string;
    bankaId: string;
    webSiteId: string;
    searchText: string;
    approveDateStart: string,
    approveDateEnd: string
    createdDateStart: string,
    createdDateEnd: string
}

export interface CekimTalepleriPagingFilter extends CekimTalepleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}
