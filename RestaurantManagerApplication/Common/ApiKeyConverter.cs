using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Common.Definitions;

namespace Common.Helpers
{
    public static class ApiKeyConverter
    {
        public static string EnumRoleToStringRole(Role role) => role.ToString().ToLower();

        public static Role StringRoleToEnumRole(string roleStr)
        {
            var success = Enum.TryParse(roleStr, true, out Role role);
            if (!success)
                throw new ArgumentException();
            return role;
        }

        public static string GetApiKey(int id, Role role) => $"{id},{EnumRoleToStringRole(role)}";

        public static (int id, Role role) GetIdAndRole(string apiKey)
        {
            var apiKeyParts = apiKey.Split(ApiKeySplitStrings, StringSplitOptions.None);

            if (apiKeyParts.Length != 2)
                throw new ArgumentException("Api key in incorrect format");

            if (!(new Regex("^[a-z]+$")).IsMatch(apiKeyParts[1]))
                throw new ArgumentException("Api key in incorrect format");

            if (!int.TryParse(apiKeyParts[0], out int id))
                throw new ArgumentException("Api key in incorrect format");

            return (id, StringRoleToEnumRole(apiKeyParts[1]));
        }



    }
}