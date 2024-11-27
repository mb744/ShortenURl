Instructions for Running the Code
1. Clone the repository from GitHub.
2. Install dependencies using dotnet restore.
3. Run the application using dotnet run.
4. Use tools like Postman to test the endpoints:
      a. POST /api/shorten with { "OriginalUrl": "http://example.com" }.
      b. GET /api/{short_id} to resolve the shortened URL.
