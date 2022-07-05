using ClientApplication.Abstracts;
using ClientApplication.Controllers.Base;
using ClientApplication.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClientApplication.Controllers
{
    public class ReviewController : AppControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }
        public IActionResult Index(int id)
        {
            NewReviewViewModel review = new() { Rating = null, RestaurantId = id };
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int id, NewReviewViewModel newReview)
        {
            newReview.RestaurantId = id;

            if (ModelState.IsValid)
            {
                var res = await reviewService.AddNewReview(newReview, HttpContext.Session);
                if (!res)
                {
                    TempData["Message"] = "Server rejected reivew!";
                }
                else
                {
                    TempData["Message"] = "Review sent successfully!";
                }
                return RedirectToAction("Index", new { id = newReview.RestaurantId });
            }

            return View(newReview);
            //return RedirectToAction("Index", new { id = newReview.RestaurantId });
        }
    }
}
