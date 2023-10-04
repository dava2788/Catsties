using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
    [Fact]
    //For the Name the convetions should be:
    //Method_Scenario_ExpectedResult
    public void HasReservePrice_ReservePriceGtZero_True()
    {
        // Arrange
        var auction = new Auction{Id= Guid.NewGuid(), ReservePrice = 10};

        //Action
        var result = auction.HasReservePrice();

        //Assert
        Assert.True(result);

    }

    [Fact]
    public void HasReservePrice_ReservePriceIsZero_False()
    {
        // Arrange
        var auction = new Auction{Id= Guid.NewGuid(), ReservePrice = 0};

        //Action
        var result = auction.HasReservePrice();

        //Assert
        Assert.False(result);

    }
}