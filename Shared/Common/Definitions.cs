using System;

namespace Common
{
    public static class Definitions
    {
        #region constant fields
        public const string API_KEY_HEADER_NAME = "api-key";

        public const string API_URL_CONFIG_KEY = "ApiURL";

        public static string[] ApiKeySplitStrings { get; } = new[] { ", ", "," };

        #endregion

        #region enums
        public enum Role
        {
            Empty,
            Customer,
            Restaurateur,
            Employee,
            Admin
        }

        public enum Application
        {
            CustomerApp,
            RestaurantManagerApp,
            AdminApp,
        }
        #endregion

    }
}
