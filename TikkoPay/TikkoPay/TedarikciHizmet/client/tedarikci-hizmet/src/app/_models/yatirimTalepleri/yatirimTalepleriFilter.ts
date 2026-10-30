export interface YatirimTalepleriFilter {
  bankaId: string;
  searchText: string;
  approveDateStart: string;
  approveDateEnd: string;
  createdDateStart: string;
  createdDateEnd: string;
}

export interface YatirimTalepleriPagingFilter extends YatirimTalepleriFilter {
  pageNumber: number;
  recordsPerPage: number;
}
