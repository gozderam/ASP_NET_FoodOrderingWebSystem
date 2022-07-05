using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiApplication.Database.POCO;

namespace WebApiTests.Helpers
{
    public static class TestHelper
    {
        public static IList<MenuPosition> FlattenMenuPositions(IEnumerable<Order_MenuPosition> ordersMenuPositions)
        {
            var flattenedPositions = new List<MenuPosition>();
            foreach (var omp in ordersMenuPositions)
                for (int i = 0; i < omp.PositionsInOrder; i++)
                    flattenedPositions.Add(omp.MenuPosition);

            return flattenedPositions;
        }

        public static IList<Order_MenuPosition> PackMenuPositions(IEnumerable<MenuPosition> menuPositions, Order order)
        {
            var ordersMenuPositions = menuPositions
               .GroupBy(mp => mp.Id)
               .Select(x => new Order_MenuPosition() { Order = order, MenuPosition = x.ElementAt(0), PositionsInOrder = x.Count() })
               .ToList();

            return ordersMenuPositions;
        }
    }
}
