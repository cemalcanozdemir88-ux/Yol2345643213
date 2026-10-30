export interface KasaHaraketleriFilter {
    durum: string,
    createdDateStart: string,
    createdDateEnd: string
}

export interface KasaHaraketleriPagingFilter extends KasaHaraketleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}