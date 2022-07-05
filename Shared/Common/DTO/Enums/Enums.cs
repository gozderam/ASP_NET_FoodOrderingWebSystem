using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public enum PaymentMethodDTO
    {
        card,
        transfer
    }

    public enum OrderStateDTO
    {
        unrealized,
        pending,
        completed,
        cancelled
    }

    public enum RestaurantStateDTO
    {
        disabled,
        active,
        deactivated,
        blocked
    }
}
