using System;

namespace Common
{
    public static class Definitions
    {
        #region constant fields
        public const string API_KEY_HEADER_NAME = "api-key";
        public static string[] ApiKeySplitStrings { get; } = new[] { ", ", "," };

        #endregion

        #region enums
        public enum Role
        {
            Customer,
            Restaurateur,
            Employee,
            Admin
        }
        #endregion

    }
}