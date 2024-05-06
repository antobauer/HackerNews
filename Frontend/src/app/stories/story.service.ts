import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StoryDto } from '../shared/storyDto.model';
import { PaginatedResponse } from '../shared/paginatedResponse.model';

@Injectable({
  providedIn: 'root'
})
export class StoryService {
  private apiUrl = 'https://localhost:44398/api/HackerNews/newstories';

  constructor(private http: HttpClient) {}

  getStories(page: number = 1, pageSize: number = 15, search: string = ''): Observable<PaginatedResponse<StoryDto>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('searchFilter', search);

    return this.http.get<PaginatedResponse<StoryDto>>(this.apiUrl, { params });
  }
}
