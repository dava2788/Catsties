using MongoDB.Entities;
using SerachService.Models;

namespace SerachService;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpadated= await DB.Find<Item,string>()
                            .Sort(x=>x.Descending(x=>x.UpdatedAt))
                            .Project(x=>x.UpdatedAt.ToString())
                            .ExecuteAnyAsync();
        
        return await _httpClient.GetFromJsonAsync<List<Item>>(
            _config["AuctionServiceURL"] + "/api/auctions?data="+lastUpadated
        );

    }

}
