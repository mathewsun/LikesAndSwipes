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

Docker Compose also starts a MinIO instance with an S3-compatible API on `http://localhost:9000` and the MinIO console on `http://localhost:9001`.


## MinIO API

The project now includes an API for working with MinIO objects:

- `POST /api/minio/upload` — upload a file using multipart form-data (`file`, optional `objectName`).
- `GET /api/minio/download/{objectName}` — download a file by object name.
- `GET /api/minio/presigned-url/{objectName}` — get a temporary download URL.
- `DELETE /api/minio/{objectName}` — delete an object from the bucket.

Configuration is read from the `Minio` section in `appsettings.json` or from environment variables such as `Minio__Endpoint` and `Minio__BucketName`.
