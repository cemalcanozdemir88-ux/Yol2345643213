export interface KasaHaraketleriFilter {
    tedarikciId: string;
    durum: string,
    createdDateStart: string,
    createdDateEnd: string
}

export interface KasaHaraketleriPagingFilter extends KasaHaraketleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}