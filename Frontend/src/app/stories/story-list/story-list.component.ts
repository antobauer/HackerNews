import { Component, OnInit } from '@angular/core';
import { StoryService } from '../story.service';
import { StoryDto } from '../../shared/storyDto.model';
import { PaginationService } from '../../pagination/pagination.service';

@Component({
  selector: 'app-story-list',
  templateUrl: './story-list.component.html',
  styleUrls: ['./story-list.component.css']
})
export class StoryListComponent implements OnInit {
  stories: StoryDto[] = [];
  searchQuery: string = '';
  
  constructor(
    private storyService: StoryService,
    private paginationService: PaginationService
  ) {}

  ngOnInit(): void {
    this.loadStories();
    this.paginationService.getCurrentPage().subscribe(page => {
      this.loadStories(page);
    });
  }

  loadStories(page: number = 1): void {
    this.storyService.getStories(page, 15, this.searchQuery).subscribe({
      next: response => {
        this.stories = response.items;
        this.paginationService.setTotalPages(response.totalPages);
      },
      error: error => console.error('Failed to fetch stories', error)
    });
  }

  onSearch(): void {
    this.loadStories(1);
  }
}