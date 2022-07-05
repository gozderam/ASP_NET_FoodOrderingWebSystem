using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTests.Base;
using WebApiApplication.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using WebApiApplication.Abstracts;
using Xunit;
using WebApiApplication.Exceptions;
using Common;
using static Common.Definitions;

namespace WebApiTests
{
    public class ApiKeyAuthAttributeTests : BaseTests
    {
        [Theory]
        [InlineData(new Role[] { Role.Admin}, "10,admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer}, "10,admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer}, "5,customer")]
        [InlineData(new Role[] { Role.Admin, Role.Admin, Role.Admin}, "10,admin")]
        [InlineData(new Role[] { Role.Employee}, "10,employee")]
        [InlineData(new Role[] { Role.Restaurateur}, "10,restaurateur")]
        [InlineData(new Role[] { Role.Customer}, "10,customer")]
        [InlineData(new Role[] { Role.Admin, Role.Restaurateur, Role.Employee, Role.Customer}, "10,customer")]
        [InlineData(new Role[] { Role.Admin }, "10, admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer }, "10, admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer }, "5, customer")]
        [InlineData(new Role[] { Role.Admin, Role.Admin, Role.Admin }, "10, admin")]
        [InlineData(new Role[] { Role.Employee }, "10, employee")]
        [InlineData(new Role[] { Role.Restaurateur }, "10, restaurateur")]
        [InlineData(new Role[] { Role.Customer }, "10, customer")]
        [InlineData(new Role[] { Role.Admin, Role.Restaurateur, Role.Employee, Role.Customer }, "10, customer")]
        public async void OnActionExecutionAsyncTests_Success(Role[] roles, string apiKey)
        {
            // mocking IAuthService
            var id = int.Parse(apiKey.Split(ApiKeySplitStrings, StringSplitOptions.None)[0]);
            var role = (Role)Enum.Parse(typeof(Role), apiKey.Split(ApiKeySplitStrings, StringSplitOptions.None)[1], true);
            
            var mockedAuthService = new Mock<IAuthService>();
            mockedAuthService.Setup(mas => mas.AreRoleAndIdConsistent(role, id)).ReturnsAsync(true);

            // creating ApiKeyAuthAttribute instance
            var attribute = new ApiKeyAuthAttribute(roles);

            // mocking OnActionExecutionAsync parameters
            var context = MockActionExecutingContext(apiKey, mockedAuthService);
            var nextNelegate = Mock.Of<ActionExecutionDelegate>();
         
            await attribute.OnActionExecutionAsync(context, nextNelegate);
        }

        [Theory]
        [InlineData(new Role[] { Role.Admin }, "10,admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer }, "10,admin")]
        [InlineData(new Role[] { Role.Admin, Role.Customer }, "5,customer")]
        [InlineData(new Role[] { Role.Admin, Role.Admin, Role.Admin }, "10,admin")]
        [InlineData(new Role[] { Role.Employee }, "10,employee")]
        [InlineData(new Role[] { Role.Restaurateur }, "10,restaurateur")]
        [InlineData(new Role[] { Role.Customer }, "10,customer")]
        [InlineData(new Role[] { Role.Admin, Role.Restaurateur, Role.Employee, Role.Customer }, "10,customer")]
        public async void OnActionExecutionAsyncTests_InconsistentIdAndRole(Role[] roles, string apiKey)
        {
            // mocking IAuthService
            var id = int.Parse(apiKey.Split(ApiKeySplitStrings, StringSplitOptions.None)[0]);
            var role = (Role)Enum.Parse(typeof(Role), apiKey.Split(ApiKeySplitStrings, StringSplitOptions.None)[1], true);
            
            var mockedAuthService = new Mock<IAuthService>();
            mockedAuthService.Setup(mas => mas.AreRoleAndIdConsistent(role, id)).ReturnsAsync(false);

            // creating ApiKeyAuthAttribute instance
            var attribute = new ApiKeyAuthAttribute(roles);

            // mocking OnActionExecutionAsync parameters
            var context = MockActionExecutingContext(apiKey, mockedAuthService);
            var nextNelegate = Mock.Of<ActionExecutionDelegate>();

            await Assert.ThrowsAsync<UnauthorizedRequestException>(() => attribute.OnActionExecutionAsync(context, nextNelegate));
        }

        [Theory]
        [InlineData(new Role[] { Role.Admin }, "10,aadmin")]
        [InlineData(new Role[] { Role.Admin }, "sdfsersefsf")]
        [InlineData(new Role[] { Role.Admin }, "admin,10")]
        [InlineData(new Role[] { Role.Customer }, "10customer")]
        [InlineData(new Role[] { Role.Employee }, "10employee")]
        [InlineData(new Role[] { Role.Restaurateur }, "10;restaurateur")]
        [InlineData(new Role[] { Role.Admin }, "10,ADMIN")]
        [InlineData(new Role[] { Role.Customer }, "10,Customer")]
        [InlineData(new Role[] { Role.Employee }, "10,eMpLoYEE")]
        public async void OnActionExecutionAsyncTests_IncorrectApiKeyFormat(Role[] roles, string apiKey)
        {
            // mocking IAuthService
            var mockedAuthService = new Mock<IAuthService>();

            // creating ApiKeyAuthAttribute instance
            var attribute = new ApiKeyAuthAttribute(roles);

            // mocking OnActionExecutionAsync parameters
            var context = MockActionExecutingContext(apiKey, mockedAuthService);
            var nextNelegate = Mock.Of<ActionExecutionDelegate>();

            await Assert.ThrowsAsync<UnauthorizedRequestException>(() => attribute.OnActionExecutionAsync(context, nextNelegate));
        }

        private static ActionExecutingContext MockActionExecutingContext(string apiKey, Mock<IAuthService> mockedAuthService)
        {
            var modelState = new ModelStateDictionary();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[Definitions.API_KEY_HEADER_NAME] = apiKey;

            var actionContext = new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            var mockedServiceProvider = new Mock<IServiceProvider>();
            mockedServiceProvider.Setup(msp => msp.GetService(typeof(IAuthService))).Returns(mockedAuthService.Object);
            context.HttpContext.RequestServices = mockedServiceProvider.Object;
            return context;
        }
    }
}
