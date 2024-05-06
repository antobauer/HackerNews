import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StoryListComponent } from './story-list.component';
import { StoryService } from '../story.service';
import { PaginationService } from '../../pagination/pagination.service';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('StoryListComponent', () => {
  let component: StoryListComponent;
  let fixture: ComponentFixture<StoryListComponent>;
  let storyServiceMock: any;
  let paginationServiceMock: any;

  beforeEach(async () => {
    storyServiceMock = jasmine.createSpyObj('StoryService', ['getStories']);
    paginationServiceMock = jasmine.createSpyObj('PaginationService', ['getCurrentPage', 'setTotalPages']);

    TestBed.configureTestingModule({
      declarations: [ StoryListComponent ],
      providers: [
        { provide: StoryService, useValue: storyServiceMock },
        { provide: PaginationService, useValue: paginationServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    });

    fixture = TestBed.createComponent(StoryListComponent);
    component = fixture.componentInstance;
    storyServiceMock.getStories.and.returnValue(of({
      items: [{ id: 1, title: 'Test Story', url: 'http://example.com' }],
      totalPages: 1
    }));
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stories on init', () => {
    paginationServiceMock.getCurrentPage.and.returnValue(of(1));
    fixture.detectChanges();
    expect(storyServiceMock.getStories).toHaveBeenCalledWith(1, 15, '');
    expect(component.stories.length).toBe(1);
    expect(component.stories[0].title).toEqual('Test Story');
  });
});
