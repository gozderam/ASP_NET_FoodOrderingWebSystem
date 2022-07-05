using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagerApplication.Models;

namespace RestaurantManagerApplication
{
    public static class TempCollections
    {
        //public static IList<DiscountCodeModel> discount_codes = new List<DiscountCodeModel>{
        //        new DiscountCodeModel {Id =1,Code = "000333", DateFrom = DateTime.Parse("01.10.21"), DateTo = DateTime.Parse("11.11.11"), Percent = 11},
        //        new DiscountCodeModel {Id =2,Code = "44533", DateFrom = DateTime.Today, DateTo = DateTime.Parse("01.10.21"), Percent = 11},
        //        new DiscountCodeModel {Id =3,Code = "04233", DateFrom = DateTime.Today, DateTo = DateTime.Parse("09.10.21"), Percent = 11},
        //        new DiscountCodeModel {Id =4,Code = "0733", DateFrom = DateTime.Parse("04.10.21"), DateTo = DateTime.Parse("04.10.21"), Percent = 11}
        //};
        //public static IList<NewRestaurantModel> restaurants = new List<NewRestaurantModel> {
        //        new NewRestaurantModel { name = "Burger King", id = 1, address = new AddressModel { city = "Krakoów",street = "Polna"} },
        //        new NewRestaurantModel { name = "KFC", id = 1, address = new AddressModel { city = "Gdańsk",street = "Pożarba"} }};
        //public static IList<MenuPositionModel> menu_positions = new List<MenuPositionModel> { 
        //        new MenuPositionModel {Id =1,Name = "Borgar1",Description= "Very tasty borgar", Price = 9.99 ,MenuSection =  new MenuSectionModel{ Name = "borgars" }, Restaurants = new List<NewRestaurantModel>{ restaurants[0] }},
        //        new MenuPositionModel {Id =2, Name = "Borgar2", Description = "Very tasty borgar", Price = 19.99 ,MenuSection =  new MenuSectionModel{ Name = "borgars" }, Restaurants = new List<NewRestaurantModel>{ restaurants[0] }},
        //        new MenuPositionModel {Id =3,Name = "Borgar3",Description= "Very tasty borgar", Price = 4.99 ,MenuSection =  new MenuSectionModel{ Name = "borgars" }, Restaurants = new List<NewRestaurantModel>{ restaurants[0] }},
        //        new MenuPositionModel {Id =4,Name = "Borgar4",Description= "Very tasty borgar", Price = 5.99,MenuSection =  new MenuSectionModel{ Name = "borgars" }, Restaurants = new List<NewRestaurantModel>{ restaurants[0] } }};
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
