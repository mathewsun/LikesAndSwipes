# LikesAndSwipes

Figma: https://www.figma.com/design/FyjMt9uKAi5pY2MW4ukQa7/Likes-And-Swipes--Design-?node-id=0-1&t=X6mCexQpCzonVMd6-1

Видео: https://www.youtube.com/watch?v=uHWjsT1mebw

Visual Studio Package Manager Console:

Add-Migration InitialCreate

Update-Database

"DefaultConnection": "Host=host.docker.internal;Port=5432;Database=likesandswipes;Username=likesandswipes;Password=likesandswipes1" //start from docker

"DefaultConnection": "Host=localhost;Port=5432;Database=likesandswipes;Username=likesandswipes;Password=likesandswipes1" //apply migrations: Update-Database

## Docker Compose

Run the application and PostgreSQL together with Docker Compose:

```bash
docker compose up --build
```

The web app will be available at `http://localhost:8080` and will connect to the `db` service using the `ConnectionStrings__DefaultConnection` environment variable defined in `docker-compose.yml`.
