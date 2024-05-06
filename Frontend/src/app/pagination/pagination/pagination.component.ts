import { Component } from '@angular/core';
import { PaginationService } from '../pagination.service';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html'
})
export class PaginationComponent {
  currentPage!: number;
  totalPages!: number;

  constructor(private paginationService: PaginationService) {
    this.paginationService.getCurrentPage().subscribe(page => this.currentPage = page);
    this.paginationService.getTotalPages().subscribe(pages => this.totalPages = pages);
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.paginationService.setCurrentPage(page);
  }

  getPages(): number[] {
    const pagesToShow = 2;
    let startPage: number = Math.max(1, this.currentPage - pagesToShow);
    let endPage: number = Math.min(this.totalPages, this.currentPage + pagesToShow);

    const pages: number[] = [];
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }
}