export interface WebSiteKasaHaraketleriFilter {
    webSiteId: string;
    durum: string,
    islemTipi: string,
    createdDateStart: string,
    createdDateEnd: string
}

export interface WebSiteKasaHaraketleriPagingFilter extends WebSiteKasaHaraketleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}