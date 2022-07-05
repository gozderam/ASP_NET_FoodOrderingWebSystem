using AdminApplication.Models;
using Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdminApplication.Abstracts
{
    public interface IRestaurantManagmentService
    {
        public Task<RestaurantDTO[]> GetAllRestaurants(HttpClient httpClient);
        public Task<bool> RestaurantActivate(HttpClient httpClient, int id);
        public Task<bool> RestaurantBlock(HttpClient httpClient, int id);
        public Task<bool> RestaurantUnblock(HttpClient httpClient, int id);
        public Task<bool> DeleteRestaurant(HttpClient httpClient, int id);
    }
}
