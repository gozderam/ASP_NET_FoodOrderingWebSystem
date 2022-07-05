using Microsoft.AspNetCore.Mvc;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Controllers.Base;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Controllers
{
    public class ReviewController : AppControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService, IHttpClientFactory clientFactory)
        {
            this.reviewService = reviewService;
        }
        public async Task<IActionResult> Index()
        {
            var reviews = await reviewService.GetReviews(HttpContext.Session);
            return View(reviews);
        }
    }
}
