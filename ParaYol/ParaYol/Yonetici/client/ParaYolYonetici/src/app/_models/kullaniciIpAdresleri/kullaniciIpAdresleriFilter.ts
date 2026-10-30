export interface KullaniciIpAdresleriFilter {
    tedarikciId: string;
    searchText: string;
}

export interface KullaniciIpAdresleriPagingFilter extends KullaniciIpAdresleriFilter {
    pageNumber: number;
    recordsPerPage: number;
}
