export interface WebSiteKasaHaraketleriFilter {
    webSiteId: string;
    durum: string,
    createdDateStart: string,
    createdDateEnd: string
}

export interface WebSiteKasaHaraketleriPagingFilter extends WebSiteKasaHaraketleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}