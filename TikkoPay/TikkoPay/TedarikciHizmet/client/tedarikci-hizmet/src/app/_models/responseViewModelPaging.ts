export class ResponseViewModelPaging<T> {
  success: boolean;
  message: string;
  result: any;
  totalPages: number;
  totalRecords: number;

  constructor() {
    this.success = false;
    this.message = '';
    this.result;
    this.totalPages = 0;
    this.totalRecords = 0;
  }
}
