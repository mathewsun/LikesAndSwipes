# LikesAndSwipes

Видео: https://www.youtube.com/watch?v=uHWjsT1mebw

Visual Studio Package Manager Console:

Add-Migration InitialCreate

Update-Database

"DefaultConnection": "Host=host.docker.internal;Port=5432;Database=likesandswipes;Username=likesandswipes;Password=likesandswipes1" //start from docker

"DefaultConnection": "Host=localhost;Port=5432;Database=likesandswipes;Username=likesandswipes;Password=likesandswipes1" //apply migrations: Update-Database
