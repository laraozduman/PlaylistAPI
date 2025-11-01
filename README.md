# PLAYLIST API

Playlist API is a RESTful API service built with .NET. This API allows users to create and manage playlists by adding or removing songs from their playlist.

The project demonstrates:

- CRUD operations
- API documentation with Swagger
- Containerization using Docker
- Deployment to Google Cloud Run

## Source Code Repository

📁 **GitHub Repository:**  
[https://github.com/laraozduman/PlaylistAPI](https://github.com/laraozduman/PlaylistAPI)

## Assumptions

1. **InMemory Database**:

   - No persistent storage is used, all data resets when the app restarts.
   - Chosen for simplicity and to avoid database setup overhead.

2. **No Authentication/Authorization**:

   - The focus is on CRUD functionality and clean REST design.

3. **Playlist–Song Relationship**:

   - One playlist can have multiple songs.
   - Each song references its parent playlist using `PlaylistId` (foreign key).

4. **Deployment Region**:
   - Deployed to **Google Cloud Run (europe-west1)** for cost efficiency and latency balance.

---

## Project structure

PlaylistApi/ \
├── Controllers/ \
│ └── PlaylistController.cs \
├── Models/ \
│ ├── PlaylistModel.cs  
│ ├── SongModel.cs  
│ ├── SongDTO.cs  
│ ├── PlaylistDTO.cs  
│ ├── PlaylistContext.cs \
├── Program.cs \
├── Dockerfile \
└── README.md

---

## System Architecture

┌────────────────────────────────┐ \
│ PlaylistController -> Receives REST requests \
├────────────────────────────────┤ \
│ PlaylistContext (EF) -> Manages in-memory data \
├────────────────────────────────┤\
│ Models / DTOs -> Define data structures\
├────────────────────────────────┤\
│ Swagger / Middleware -> Documentation & routing\
└────────────────────────────────┘

## Technologies Used

- Backend : ASP.NET Core Web API (.NET 9)
- ORM : Entity Framework Core (InMemory)
- Containerization : Docker
- Cloud Hosting : Google Cloud Run
- API Documentation : Swagger

## API Endpoints

### Playlist Operations

#### Create a Playlist

**POST** `/api/playlist/playlist`

#### Delete a Playlist

**DELETE** `/api/playlist/playlists/{id}`

#### Get All Playlists

**GET** `/api/playlist/playlists`

#### Get All Playlists

**PUT** `/api/playlist/{playlistId}`

### Song Operations

#### Add a Playlist

**POST** `/api/playlist`

#### Get All Songs in a Playlist

**GET** `/api/playlist/playlist/{playlistId}/songs`

#### Search Songs in a Playlist

**GET** `/api/playlist/search?search=Ever&playlistId=1`

#### Delete a Song

**DELETE** `/api/playlist/{songId}`

#### Delete all Songs in a Playlist

**DELETE** `/api/playlist/playlist/{playlistId}/songs`

**Example Request Body For Create Playlist:**

```json
{
  "playlistName": "Rock Classics"
}
```

PowerShell Example:

```bash
Invoke-RestMethod -Uri "http://localhost:5180/api/playlist/playlist" `
-Method POST -Headers @{ "Content-Type" = "application/json" } `
-Body '{ "playlistName": "Rock Classics" }'

```

**Example Request Body For Add a Song:**

```json
{
  "SongName": "Everlong",
  "Artist": "Foo Fighters",
  "Duration": "4:10",
  "PlaylistId": 1
}
```

PowerShell Example:

```bash
Invoke-RestMethod -Uri "http://localhost:5180/api/playlist" -Method POST `
-ContentType "application/json" -Body '{ "SongName": "Take Me Out", "Artist": "Franz Ferdinand", "Duration": "3:55", "PlaylistId": 1 }'

```

## Local Development

**Requirements**

- .NET 9 SDK
- Docker Desktop (Optional)

**Run the API Locally**

From the root directory run the commands:

```bash
dotnet restore
dotnet build
dotnet run
```

After running the hostwll be available at http://localhost:5180

## Build Using Docker

Run these commands to build a docker container

```Bash
docker build -t playlist-api .
```

```Bash
docker run -p 8080:8080 playlist-api
```

## Deployment

This API is deployedon Google Cloud with the url:

```
https://playlist-api-794683868475.europe-west1.run.app
```

## Testing

This API has swagger documentation for testing. Swagger provides an interactive interface for testing and visualizing API endpoints.  
It automatically generates documentation from controller attributes and route definitions.

**Local Testing**

To test locally first build a docker container, then swagger will be available in the host:

```
http://localhost:8080/swagger
```

**Deployed Swagger(Cloud Run)**

```
https://playlist-api-794683868475.europe-west1.run.app/swagger/index.html
```

## Issues Encountered & Solutions

**Circular JSON serialization error**

EF’s navigation properties caused recursive loops in `/playlists` response. As a solution used DTOs and simplified model relationships.

**400 Bad Request for DTOs**

DTO validation mismatch during POST requests. Fixed by aligning property names and ensuring `[FromBody]` attributes were properly set.

**500 Internal Error during JSON serialization**

Songs contained Playlist references creating nested objects. Adjusted serialization and limited Include depth asasolution.

**Docker runtime error (Static folder missing)**

Container tried to access `/app/Static/`. To solve this,removed unnecessary static file dependency.

## Author

**Lara Özudman**

- SE4458 - Software Construction
- Yaşar University
- Deployed on Google Cloud Run – playlist-api
- GitHub: https://github.com/laraozduman
