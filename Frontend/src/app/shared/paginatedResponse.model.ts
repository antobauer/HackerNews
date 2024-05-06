export interface PaginatedResponse<T> {
    items: T[];
    pageIndex: number;
    totalPages: number;
    totalCount: number;
  }