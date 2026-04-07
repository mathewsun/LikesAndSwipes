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

Docker Compose also starts a MinIO instance with an S3-compatible API on `http://host.docker.internal:9000` and the MinIO console on `http://host.docker.internal:9001` (not http://localhost:9000).


## MinIO API

The project now includes an API for working with MinIO objects:

- `POST /api/minio/upload` — upload a file using multipart form-data (`file`, optional `objectName`).
- `GET /api/minio/download/{objectName}` — download a file by object name.
- `GET /api/minio/presigned-url/{objectName}` — get a temporary download URL.
- `DELETE /api/minio/{objectName}` — delete an object from the bucket.

Configuration is read from the `Minio` section in `appsettings.json` or from environment variables such as `Minio__Endpoint` and `Minio__BucketName`.


## HTTPS with Certbot (auto-renew)

The Docker Compose stack now includes:

- `nginx` as a reverse proxy on ports `80` and `443`.
- `certbot` for Let's Encrypt certificate renewal every 12 hours.

### 1) Configure your domain + email

Create a `.env` file in the repository root:

```env
DOMAIN=your-domain.com
CERTBOT_EMAIL=admin@your-domain.com
```

Make sure your DNS `A/AAAA` record for `DOMAIN` points to this host before requesting certificates.

### 2) Create the initial certificate

Run once:

```bash
DOMAIN=your-domain.com CERTBOT_EMAIL=admin@your-domain.com ./scripts/init-letsencrypt.sh
```

### 3) Start the full stack

```bash
docker compose up -d --build
```

After that, certbot auto-renews in the background, and nginx reloads periodically to pick up renewed certificates.
