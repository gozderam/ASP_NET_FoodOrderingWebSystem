using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagerApplication.Models
{
    public class RestaurantStatsModel
    {
        public OrderModel[] Orders { get; set; }
        public double TotalMoney { get; private set; }

        public double AverageMoney { get; private set; }
        public EmployeeModel MostActiveEmployee { get; private set; }

        public MenuPositionModel MostPopularFood { get; private set; }

        
        public RestaurantStatsModel(OrderModel[] orders)
        {
            this.Orders = orders;
            if(orders != null && orders.Length > 0)
            {
                TotalMoney = orders.Sum(o => o.finalPrice);
                AverageMoney = TotalMoney / orders.Length;
                MostActiveEmployee = GetMostActiveEmployee(orders);
                MostPopularFood = GetMostPopularFood(orders);
            }
        }

        private EmployeeModel GetMostActiveEmployee(OrderModel[] orders)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach(var o in orders)
            {
                var currEmployee = o.employee;
                if (dict.ContainsKey(currEmployee.Id))
                {
                    dict[currEmployee.Id]++;
                }
                else
                {
                    dict.Add(currEmployee.Id, 1);
                }

            }
            //https://stackoverflow.com/questions/10290838/how-to-get-max-value-from-dictionary/10290858
            var bestId = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            return orders.First(o => o.employee.Id == bestId).employee;
        }

        private MenuPositionModel GetMostPopularFood(OrderModel[] orders)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach(var o in orders)
            {
                foreach(var p in o.positions) {
                    if (dict.ContainsKey(p.Id))
                    {
                        dict[p.Id]++;
                    }
                    else
                    {
                        dict.Add(p.Id, 1);
                    }
                }
            }
            //https://stackoverflow.com/questions/10290838/how-to-get-max-value-from-dictionary/10290858
            var bestId = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

            foreach (var o in orders)
            {
                foreach (var p in o.positions)
                {
                    if (p.Id == bestId)
                        return p;
                }
            }
            return null;
        }
    }
}
