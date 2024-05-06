import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PaginationComponent } from './pagination.component';
import { PaginationService } from '../pagination.service';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('PaginationComponent', () => {
  let component: PaginationComponent;
  let fixture: ComponentFixture<PaginationComponent>;
  let paginationServiceMock: any;

  beforeEach(async () => {
    paginationServiceMock = jasmine.createSpyObj('PaginationService', ['getCurrentPage', 'getTotalPages', 'setCurrentPage']);
    paginationServiceMock.getCurrentPage.and.returnValue(of(1));
    paginationServiceMock.getTotalPages.and.returnValue(of(5));  

    await TestBed.configureTestingModule({
      declarations: [ PaginationComponent ],
      providers: [
        { provide: PaginationService, useValue: paginationServiceMock }
      ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PaginationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call setCurrentPage when changePage is called with a valid page', () => {
    component.changePage(3);
    expect(paginationServiceMock.setCurrentPage).toHaveBeenCalledWith(3);
  });

  it('should not call setCurrentPage when changePage is called with an invalid page', () => {
    component.changePage(0); 
    expect(paginationServiceMock.setCurrentPage).not.toHaveBeenCalled();
  });
});
