using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace AuctionService.IntegrationTests;

//Class Add our Test services, so we can have a fake database
// fake service bus and how we can have fake authentication.

//IAsyncLifetime Interface is for create the Postgress container.
//we want to start it when this web application factory class is initialized and
// we want to dispose of it once the tests have been run.
public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    //PostgreSqlContainer get for Testcontainers.PostgreSql
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //for configure the  test Services
        builder.ConfigureTestServices(services => 
        {
            //check if the Service DB Context Exists
            //ANd If so remove it to the services
            services.RemoveDbContext<AuctionDbContext>();

            //Now addd the test AuctionDbContext
            services.AddDbContext<AuctionDbContext>(options =>
            {
                //get the conection string from the Container
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });
            //For the rabbitMq the AddMassTransitTestHarness 
            services.AddMassTransitTestHarness();

            services.EnsureCreated<AuctionDbContext>();

            // For fake the JWT identity
            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                .AddFakeJwtBearer(opt =>
                {
                    opt.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                });
        });
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}

internal class PostgresSqlContainer
{
}