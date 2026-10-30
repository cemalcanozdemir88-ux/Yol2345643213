import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent {
  @Input() activePage: number = 1;
  @Input() totalPages: number = 0;
  @Output() pageChanged = new EventEmitter<number>();

  constructor() { }

  onPageChange(pageNumber: number): void {
    if (pageNumber !== this.activePage && pageNumber > 0 && pageNumber <= this.totalPages) {
      this.pageChanged.emit(pageNumber);
    }
  }

  getPageNumbers(): any[] {
    if (this.totalPages <= 1) {
      return [1];
    }

    const pageNumbers: any[] = [];
    const delta = 2;
    const range = [];
    const rangeWithDots = [];
    let l;

    for (let i = Math.max(2, this.activePage - delta); i <= Math.min(this.totalPages - 1, this.activePage + delta); i++) {
      range.push(i);
    }

    if (this.activePage - delta > 2) {
      rangeWithDots.push(1);
      rangeWithDots.push('...');
    } else {
      for (let i = 1; i < Math.max(2, this.activePage - delta); i++) {
        rangeWithDots.push(i);
      }
    }

    rangeWithDots.push(...range);

    if (this.activePage + delta < this.totalPages - 1) {
      rangeWithDots.push('...');
      rangeWithDots.push(this.totalPages);
    } else {
      for (let i = Math.min(this.totalPages - 1, this.activePage + delta) + 1; i <= this.totalPages; i++) {
        rangeWithDots.push(i);
      }
    }

    return rangeWithDots;
  }
}
