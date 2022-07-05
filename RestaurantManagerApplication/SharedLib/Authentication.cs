using Common;
using Common.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Common.Definitions;

namespace SharedLib.Security
{
    public static class Authentication
    {
        //TODO: move and change
        private const string addressLogin = "https://localhost:5001/user/login";
        private const string addressGetCustomer = "https://localhost:5001/user/customer";
        private const string addressGetEmployee = "https://localhost:5001/user/employee";
        private const string addressGetAdmin = "https://localhost:5001/user/admin";

        #region private fields
        private static readonly Dictionary<Role, string> addresses = new()
        {
            { Role.Admin, addressGetAdmin },
            { Role.Customer, addressGetCustomer },
            { Role.Employee, addressGetEmployee },
            { Role.Restaurateur, addressGetEmployee },
        };
        #endregion

        #region session keys
        private const string USER_ID_SK = "id";
        private const string USER_ROLE_SK = "role";
        private const string USER_NAME_SK = "name";
        private const string USER_SURNAME_SK = "surname";
        private const string USER_EMAIL_SK = "email";
        #endregion

        #region public methods
        #region login
        public static async Task<bool> Login(string email, ISession session)
        {
            HttpClient client = new();
            var response = await client.GetAsync(addressLogin + "?" + $"email={email}");
            if (!response.IsSuccessStatusCode)
                return false;

            var apiKey = await response.Content.ReadAsStringAsync();
            return await SetSessionAttributesForUser(session, apiKey);
        }
        #endregion

        #region preparing requests to WebApi calls
        public static void AddApiKeyHeader(this HttpRequest request, ISession session)
        {
            request.Headers[Definitions.API_KEY_HEADER_NAME]
                = ApiKeyConverter.GetApiKey(GetLoggedUserId(session), ApiKeyConverter.StringRoleToEnumRole(GetLoggedUserStringRole(session)));
        }

        public static void AddApiKeyHeader(this HttpClient client, ISession session)
        {
            client.DefaultRequestHeaders.Add(Definitions.API_KEY_HEADER_NAME,
                ApiKeyConverter.GetApiKey(GetLoggedUserId(session), ApiKeyConverter.StringRoleToEnumRole(GetLoggedUserStringRole(session))));
        }
        #endregion

        #region getting logged user info
        public static bool IsUserLogged(ISession session)
        {
            return session.TryGetValue(USER_ID_SK, out byte[] _);
        }

        public static int GetLoggedUserId(ISession session)
        {
            if (session.TryGetValue(USER_ID_SK, out byte[] result))
                return BitConverter.ToInt32(result);
            return -1;
        }

        public static string GetLoggedUserName(ISession session)
        {
            if (session.TryGetValue(USER_NAME_SK, out byte[] result))
                return Encoding.UTF8.GetString(result);
            return null;
        }

        public static string GetLoggedUserSurname(ISession session)
        {
            if (session.TryGetValue(USER_SURNAME_SK, out byte[] result))
                return Encoding.UTF8.GetString(result);
            return null;
        }

        public static string GetLoggedUserEmail(ISession session)
        {
            if (session.TryGetValue(USER_EMAIL_SK, out byte[] result))
                return Encoding.UTF8.GetString(result);
            return null;
        }

        public static Role GetLoggedUserRole(ISession session)
        {
            return ApiKeyConverter.StringRoleToEnumRole(GetLoggedUserStringRole(session));
        }
        #endregion
        #endregion

        #region private methods
        private static async Task<bool> SetSessionAttributesForUser(ISession session, string apiKey)
        {
            (var id, var role) = ApiKeyConverter.GetIdAndRole(apiKey);
            session.Set(USER_ID_SK, BitConverter.GetBytes(id));
            session.Set(USER_ROLE_SK, Encoding.UTF8.GetBytes(ApiKeyConverter.EnumRoleToStringRole(role)));

            HttpClient client = new();
            client.AddApiKeyHeader(session);
            var response = await client.GetAsync(addresses[role] + "?" + $"id={id}");
            if (!response.IsSuccessStatusCode)
            {
                session.Clear();
                return false;
            }

            var loggedUser = JsonConvert.DeserializeObject<LoggedUserDTO>(await response.Content.ReadAsStringAsync());

            session.Set(USER_NAME_SK, Encoding.UTF8.GetBytes(loggedUser.Name));
            session.Set(USER_SURNAME_SK, Encoding.UTF8.GetBytes(loggedUser.Surname));
            session.Set(USER_EMAIL_SK, Encoding.UTF8.GetBytes(loggedUser.Email));

            return true;
        }

        private static string GetLoggedUserStringRole(ISession session)
        {
            if (!session.TryGetValue(USER_ROLE_SK, out byte[] result))
                return null;

            return Encoding.UTF8.GetString(result);
        }
        #endregion
    }
}