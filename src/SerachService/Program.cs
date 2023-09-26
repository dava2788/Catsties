using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SerachService;
using SerachService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit(x=>
{
    //With this approach all the consumers 
    //within the same namespace will be added
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    //this will add a prefix to all the consumers
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search",false));

    x.UsingRabbitMq((context,config)=>
    {
        config.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        //configure for the search-auction endpoint
        config.ReceiveEndpoint("search-auction-created", e => 
        {
            //will do 5 times and wait 5 second
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        config.ConfigureEndpoints(context);
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async() => 
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (System.Exception e)
    {
        
        Console.WriteLine(e);
    }
});


app.Run();

static IAsyncPolicy<HttpResponseMessage>GetPolicy() 
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg=> msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_=> TimeSpan.FromSeconds(3));


