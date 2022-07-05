# ASP_NET_FoodOrderingWebSystem
ASP.NET (WebApi + 3xMVC) application system for online food ordering.

The system consists of 5 projects: 
* WebApi provides the backend logic and communicates with the database (EntityFramework ORM is used)
* Shared library containing common tools for other applications
* RestaurantManager application - MVC-based front-end app dedicated to managing the restaurant by a restaurant owner
* Client application - MVC-based front-end app created for making food orders
* Admin application - MVC-based front-end app system administrator application

RestaurantManager, Client, and Admin applications communicate with WebAPI.
