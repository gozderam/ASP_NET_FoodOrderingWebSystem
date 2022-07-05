using Microsoft.Extensions.Configuration;
using RestaurantManagerApplication.Abstracts;
using RestaurantManagerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Common.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLib.Security;
using Common.DTO;

namespace RestaurantManagerApplication.Services
{
    public class MenuService : IMenuService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private IMapper mapper;

        private readonly string MENU_SECTION_URL = @"/restaurant/menu/section";
        private readonly string MENUPOSITION_URL = @"/restaurant/menu/position";
        private readonly string GET_MENU_URL = @"/restaurant/menu";
        private readonly string BASE_URL;

        public MenuService(IHttpClientFactory clientFactory, IConfiguration configuration, IMapper mapper)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.mapper = mapper;
            BASE_URL = this.configuration[Definitions.API_URL_CONFIG_KEY];
        }
        public async Task<MenuSectionModel[]> GetMenu(ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            HttpResponseMessage response = await httpClient.GetAsync(BASE_URL + GET_MENU_URL);
            MenuSectionModel[] menu = null;

            SectionDTO[] mmm = JsonSerializer.Deserialize<SectionDTO[]>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                menu =  mmm.ToList().ConvertAll(s => mapper.Map<MenuSectionModel>(s)).ToArray();
                for (int i = 0; i < mmm.Length; i++)                
                    menu[i].MenuPositions = mmm[i].positions.ToList().ConvertAll(pos => mapper.Map<MenuPositionModel>(pos)).ToArray();
                return menu;
            }
            
            return new MenuSectionModel[0];
        }

        public async Task<bool> CreateMenuPosition(NewMenuPositionModel newPosition, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var content = new StringContent(
                JsonSerializer.Serialize(mapper.Map<NewPositionFromMenuDTO>(newPosition)),
                Encoding.UTF8,
                "application/json");
            NewPositionFromMenuDTO aaa = mapper.Map<NewPositionFromMenuDTO>(newPosition);
            var response = await httpClient.PostAsync(BASE_URL + MENUPOSITION_URL + $@"?id={newPosition.sectionID}", content);
            
            return response.IsSuccessStatusCode;     
        }

        public async Task<bool> EditMenuPosition(MenuPositionModel positionToEdit, ISession session)   ///TODO
        {
            NewMenuPositionModel toEdit = mapper.Map<NewMenuPositionModel>(positionToEdit);
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var content = new StringContent(
                JsonSerializer.Serialize(toEdit),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PatchAsync(BASE_URL + MENUPOSITION_URL + $@"?id={positionToEdit.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMenuPosition(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var response = await httpClient.DeleteAsync(BASE_URL + MENUPOSITION_URL + $@"?id={id}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        public async Task<bool> AddMenuSection(MenuSectionModel newSection, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            string url = BASE_URL + MENU_SECTION_URL + $@"?section={newSection.Name}";

            var response = await httpClient.PostAsync(BASE_URL + MENU_SECTION_URL + $@"?section={newSection.Name}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> EditMenuSection(int sectionId, string newSectionName, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var content = new StringContent(
                JsonSerializer.Serialize(newSectionName),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PatchAsync(BASE_URL + MENU_SECTION_URL + $@"?id={sectionId}", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteMenuSection(int id, ISession session)
        {
            HttpClient httpClient = clientFactory.CreateClient();
            Authentication.AddApiKeyHeader(httpClient, session);
            var response = await httpClient.DeleteAsync(BASE_URL + MENU_SECTION_URL + $@"?id={id}");
            return response.IsSuccessStatusCode;
            
        }
    }
}
