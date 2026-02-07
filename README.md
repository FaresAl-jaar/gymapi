# GymApi Backend

ASP.NET Core Web API für die GymApp.

## Features

- ?? JWT Authentication (Register/Login)
- ??? Workout Session Tracking (Check-in/Check-out)
- ?? Personal Records (PR) Feed
- ?? Weekly Leaderboard
- ?? Location-based "Bros in der Nähe"
- ?? Stoke System (Motivation senden)

## API Endpoints

### Auth (Kein Token erforderlich)
- `POST /api/auth/register` - Registrierung
- `POST /api/auth/login` - Login

### User (Token erforderlich)
- `GET /api/user/me` - Aktueller User
- `PUT /api/user/location` - Standort aktualisieren

### Workout (Token erforderlich)
- `GET /api/workout/active` - Aktive Sessions
- `POST /api/workout/checkin` - Einchecken
- `POST /api/workout/checkout/{id}` - Auschecken
- `PUT /api/workout/{id}` - Session aktualisieren
- `POST /api/workout/stoke/{userId}` - Stoke senden

### Exercise (Token erforderlich)
- `GET /api/exercise` - Alle Übungen
- `POST /api/exercise` - Neue Übung erstellen

### PR (Token erforderlich)
- `GET /api/pr/feed` - PR Feed
- `POST /api/pr` - Neuen PR loggen

### Leaderboard (Token erforderlich)
- `GET /api/leaderboard` - Wöchentliches Ranking

### Health
- `GET /health` - Health Check

## Deployment auf Coolify

### 1. Repository pushen
```bash
cd GymApi
git init
git add .
git commit -m "Initial commit"
git remote add origin <your-repo-url>
git push -u origin main
```

### 2. In Coolify
1. Neues Projekt erstellen
2. "Add Resource" ? "Docker"
3. Repository URL eingeben
4. Build Pack: "Dockerfile"
5. Port: 8080

### 3. Environment Variables in Coolify setzen
```
Jwt__Key=DEIN_SUPER_GEHEIMER_KEY_MINDESTENS_32_ZEICHEN
Jwt__Issuer=GymApi
Jwt__Audience=GymApp
ConnectionStrings__DefaultConnection=Data Source=/app/data/gymapi.db
```

### 4. Persistent Storage
- Volume: `/app/data` (für SQLite Datenbank)

## Lokale Entwicklung

```bash
cd GymApi
dotnet run
```

Swagger UI: http://localhost:5000/swagger

## Verwendung in der MAUI App

Die API-URL in der MAUI App aktualisieren:
```csharp
_httpClient.BaseAddress = new Uri("https://deine-api.coolify.domain/api/");
```
