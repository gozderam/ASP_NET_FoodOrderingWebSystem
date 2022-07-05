using Common;
using Common.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        #region private fields
        private static readonly Dictionary<Application, string> urlLoginSuffixes = new()
        {
            {Application.AdminApp,  "/user/admin/login"},
            {Application.CustomerApp, "/user/customer/login" },
            {Application.RestaurantManagerApp, "/user/employee/login" },
        };

        private static readonly Dictionary<Role, string> urlGetUserSuffixes = new()
        {
            { Role.Admin, "/user/admin" },
            { Role.Customer, "/user/customer" },
            { Role.Employee, "/user/employee" },
            { Role.Restaurateur, "/user/employee" },
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
        #region login/logout
        public static async Task<bool> Login(string email, Application app, ISession session, IConfiguration configuration)
        {
            string url = configuration[API_URL_CONFIG_KEY] + urlLoginSuffixes[app];
            HttpClient client = new();
            var response = await client.GetAsync(url + "?" + $"email={email}");
            if (!response.IsSuccessStatusCode)
                return false;

            var apiKey = await response.Content.ReadAsStringAsync();
            return await SetSessionAttributesForUser(session, apiKey, configuration);
        }

        public static void Logout(ISession session)
        {
            session.Clear();
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
        private static async Task<bool> SetSessionAttributesForUser(ISession session, string apiKey, IConfiguration configuration)
        {    
            (var id, var role) = ApiKeyConverter.GetIdAndRole(apiKey);
            session.Set(USER_ID_SK, BitConverter.GetBytes(id));
            session.Set(USER_ROLE_SK, Encoding.UTF8.GetBytes(ApiKeyConverter.EnumRoleToStringRole(role)));

            using HttpClient client = new();
            client.AddApiKeyHeader(session);
            var response = await client.GetAsync(configuration[API_URL_CONFIG_KEY] + urlGetUserSuffixes[role] + "?" + $"id={id}");
            if (!response.IsSuccessStatusCode)
            {
                session.Clear();
                return false;
            }

            var loggedUser  = JsonConvert.DeserializeObject<LoggedUserDTO>(await response.Content.ReadAsStringAsync());

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
