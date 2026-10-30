import { HavaleBankalariDurumFilter } from "src/app/_enums/havaleBankalariDurumFilter.enum";

export interface HavaleBankalariFilter {
    tedarikciId: string;
    bankaId: string;
    searchText: string;
    havaleBankalariDurumFilter: HavaleBankalariDurumFilter
}

export interface HavaleBankalariPagingFilter extends HavaleBankalariFilter {
    pageNumber: number;
    recordsPerPage: number;
}
