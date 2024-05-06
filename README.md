HackerNews App Challenge:

Assumptions:
-The app will work with the top 500 newest stories from the hacker-news api.

Details:
-The app loads the top stories from the hacker-news api at initialization.
-It refreshes the cache only with the newest stories each 15 minutes with a background task.
-It provides both search and pagination capabilities.

How to run:
-Run the HackerNews.Application project.
-Run "npm install node" command in the terminal.
-Run the Angular application with "ng serve" command.

Configuration:
The appsettings.json file contains some configuration for the app.
- DefaultPageSize (int): The default size of each page of the stories list
- CacheRefreshTime (int):  The time that takes to the background task to update the cache with the newest stories (in milliseconds).

Technologies used:
-Styling: Bootstrap.
-Testing: NUnit, NSubstitute.

Work tracking:
Started: Friday 26 - 4 hours.
Monday 29 - 7 hours.
Tuesday 30 - 7 hours.
Wednesday 1 - 0 hours (public holiday).
Thursday 2 - 7 hours.
Friday 3 - 7 hours.
