import { TestBed } from '@angular/core/testing';
import { PaginationService } from './pagination.service';

describe('PaginationService', () => {
  let service: PaginationService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [PaginationService] });
    service = TestBed.inject(PaginationService);
  });

  it('should set and get current page correctly', (done: DoneFn) => {
    service.setCurrentPage(2);
    service.getCurrentPage().subscribe(page => {
      expect(page).toEqual(2);
      done();
    });
  });

  it('should set and get total pages correctly', (done: DoneFn) => {
    service.setTotalPages(5);
    service.getTotalPages().subscribe(pages => {
      expect(pages).toEqual(5);
      done();
    });
  });
});
