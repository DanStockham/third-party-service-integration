using System;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME.Controllers;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CUNAMUTUAL_TAKEHOME_TESTS
{
    public class ProxyControllerTests
    {
        [Fact]
        public async Task ShouldReturnStartedOnSuccess()
        {
            var mockProxyService = new Mock<IProxyService>();
            var request = new ServiceRequest();

            mockProxyService.Setup(x => x.RequestCallback(It.IsAny<ServiceRequest>())).ReturnsAsync("STARTED");
            
            var target = new ProxyController(mockProxyService.Object);
            var actual = await target.RequestService(request) as ContentResult;
            
            Assert.Equal("STARTED", actual?.Content);
        }

        public async Task ShouldReturnFailedOnFailure()
        {
            var mockProxyService = new Mock<IProxyService>();
            var request = new ServiceRequest();
            
            var target = new ProxyController(mockProxyService.Object);
            var actual = await target.RequestService(request) as ContentResult;
            
            Assert.Equal("FAILED", actual?.Content);
        }
    }
}