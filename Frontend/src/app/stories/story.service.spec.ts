import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { StoryService } from './story.service';
import { StoryDto } from '../shared/storyDto.model';
import { PaginatedResponse } from '../shared/paginatedResponse.model';

describe('StoryService', () => {
  let service: StoryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [StoryService]
    });
    service = TestBed.inject(StoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch stories correctly', () => {
    const mockResponse: PaginatedResponse<StoryDto> = {
      items: [{ title: 'Test Story', url: 'http://example.com' }],
      totalPages: 1,
      pageIndex: 1,
      totalCount: 1
    };

    service.getStories(1, 15, 'test').subscribe(response => {
      expect(response.items.length).toBe(1);
      expect(response.items[0].title).toEqual('Test Story');
    });

    const req = httpMock.expectOne('https://localhost:44398/api/HackerNews/newstories?page=1&pageSize=15&searchFilter=test');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });
});
