using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiApplication.Abstracts.Results;
using WebApiApplication.MappingProfiles;
using Xunit;

namespace WebApiTests.Base
{
    public class BaseTests
    {
        protected static IMapper Mapper { get; private set;  }

        public BaseTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new FoodDeliveryMappingProfile());
            });
            Mapper = mappingConfig.CreateMapper();
        }

        protected void AssertSuccessServiceResult<T>(IOperationResult<T> serviceResult)
        {
            Assert.NotNull(serviceResult);
            Assert.Equal(ResultCode.Success, serviceResult.Code);
            Assert.NotNull(serviceResult.Data);
        }
    }
}
