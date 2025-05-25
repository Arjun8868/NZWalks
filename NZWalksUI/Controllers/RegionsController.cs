using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models.DTO;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            
        }
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();

            //Get all regions from Web API
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpresponsemessage = await client.GetAsync("https://localhost:7016/api/regions");

                httpresponsemessage.EnsureSuccessStatusCode();

                response.AddRange(await httpresponsemessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

            }
            catch (Exception)
            {

               
            }
            return View(response);
        }
    }
}
