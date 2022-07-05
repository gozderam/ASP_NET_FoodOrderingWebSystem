using Microsoft.AspNetCore.Mvc;
using RestaurantManagerApplication.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AdminApplication.Controllers
{
    public class TablesController : AppControllerBase
    {
        //TODO: Zmienić na skonfigurowany URL
        private string url = @"https://localhost:5001/TempDbTest";
        public async Task<IActionResult> Tables()
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();


            return View();
        }
    }
}
