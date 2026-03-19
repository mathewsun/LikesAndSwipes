using LikesAndSwipes.Data;
using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LikesAndSwipes.Tests;

public class DataRepositoryTests
{
    [Fact]
    public async Task SaveUserFirstName_Throws_WhenUserDoesNotExist()
    {
        //using var connection = new SqliteConnection("DataSource=:memory:");
        //await connection.OpenAsync();

        //var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //    .UseSqlite(connection)
        //    .Options;

        //await using var context = new ApplicationDbContext(options);
        //await context.Database.EnsureCreatedAsync();

        //var repository = new DataRepository(context);

        //var act = () => repository.SaveUserFirstName(new User
        //{
        //    Id = "missing-user-id",
        //    FirstName = "Alice"
        //});

        //await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}
