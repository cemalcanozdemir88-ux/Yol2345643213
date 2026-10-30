export interface CekimTalepleriFilter {
    tedarikciId: string;
    bankaId: string;
    webSiteId: string;
    searchText: string;
    dekontKontrolDurum: string;
    approveDateStart: string;
    approveDateEnd: string;
    createdDateStart: string;
    createdDateEnd: string;
}

export interface CekimTalepleriPagingFilter extends CekimTalepleriFilter {
    pageNumber: number;
    recordsPerPage: number;
    sortColumn?: string;
    sortDirection?: 'asc' | 'desc';
}
