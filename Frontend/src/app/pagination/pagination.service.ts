import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PaginationService {
  private currentPage = new BehaviorSubject<number>(1);
  private totalPages = new BehaviorSubject<number>(1);

  getCurrentPage() {
    return this.currentPage.asObservable();
  }

  getTotalPages() {
    return this.totalPages.asObservable();
  }

  setCurrentPage(page: number) {
    this.currentPage.next(page);
  }

  setTotalPages(pages: number) {
    this.totalPages.next(pages);
  }
}