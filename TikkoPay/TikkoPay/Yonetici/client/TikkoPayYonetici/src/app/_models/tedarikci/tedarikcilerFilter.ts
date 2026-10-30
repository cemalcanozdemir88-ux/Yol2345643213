export interface TedarikcilerFilter {
    bankaHavale: string;
    otoPuan: string;
    durum: string;
}

export interface TedarikcilerPagingFilter extends TedarikcilerFilter {
    pageNumber: number;
    recordsPerPage: number;
}