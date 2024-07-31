

using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService;

public class CheckAuctionFinished : BackgroundService
{
    private readonly ILogger<CheckAuctionFinished> _logger;
    private readonly IServiceProvider _services;

    public CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting check for finished auctions");
        stoppingToken.Register(() => _logger.LogInformation("==> Auction heck is stopping"));
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        var finihsedAuction  = await DB.Find<Auction>()
            .Match(x => x.AuctionEnd <= DateTime.UtcNow)
            .Match(x => !x.Finished)
            .ExecuteAsync(stoppingToken);

        if (finihsedAuction.Count == 0) return;

        _logger.LogInformation("==> Found {count} auctions taht have complted",finihsedAuction.Count);

        // The background service is going to run as a singleton.
        // The mass transit service lifetime is scoped to the scope of the request.
        // So we can't inject something that's got a different lifetime into our background service that we're creating here.
        // So we have to create a scope inside this so that we can get access to the I publish endpoint and we
        // can actually publish the event for the finished auction.

        using var scope = _services.CreateScope();
        var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        foreach (var auction in finihsedAuction)
        {
            auction.Finished = true;
            await auction.SaveAsync(null, stoppingToken);

            var winnigBid = await DB.Find<Bid>()
            .Match(a => a.AuctionId.Equals(auction.ID))
            .Match(b => b.BidStatus.Equals(BidStatus.Accepted))
            .Sort(x => x.Descending(x => x.Amount))
            .ExecuteFirstAsync();

            await endpoint.Publish(new AuctionFinished{
                ItemSold = winnigBid != null,
                AuctionId = auction.ID,
                Winner = winnigBid?.Bidder,
                Amount = winnigBid?.Amount,
                Seller = auction.Seller
            }, stoppingToken);
        }



    }
}
