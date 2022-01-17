using System;
using System.Threading.Tasks;
using CUNAMUTUAL_TAKEHOME;
using CUNAMUTUAL_TAKEHOME.Controllers;
using CUNAMUTUAL_TAKEHOME.Repositories;
using CUNAMUTUAL_TAKEHOME.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.Frameworks;
using Xunit;

namespace CUNAMUTUAL_TAKEHOME_TESTS
{
    public class ProxyControllerTests
    {
        [Fact]
        public async Task Request_ShouldReturnStartedOnSuccess()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();
            var request = new ServiceItemRequest();

            mockProxyService.Setup(x => x.RequestCallback(It.IsAny<ServiceItem>())).ReturnsAsync("STARTED");
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.RequestService(request) as ContentResult;
            
            Assert.Equal("STARTED", actual?.Content);
        }

        [Fact]
        public async Task Request_ShouldReturnInternalServerErrorWhenThirdPartyFails()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();
            var request = new ServiceItemRequest();
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.RequestService(request) as ContentResult;
            
            Assert.Equal(500, actual?.StatusCode);
        }

        [Fact]
        public async Task PostCallback_ShouldInvokeServiceItemUpdateStatus()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(new ServiceItem());
            mockServiceItemRepo.Setup(x => x.UpdateServiceItemStatus(It.IsAny<Statuses>(), It.IsAny<string>()))
                .ReturnsAsync(new ServiceItem());
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            await target.Callback(It.IsAny<string>(), Statuses.STARTED.ToString());
            
            mockServiceItemRepo.Verify(x => x.UpdateServiceItemStatus(It.IsAny<Statuses>(), It.IsAny<string>()), Times.AtLeastOnce());
        }
        
        [Fact]
        public async Task PostCallback_ShouldReturnNoContentOnSuccess()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(new ServiceItem());
            mockServiceItemRepo.Setup(x => x.UpdateServiceItemStatus(It.IsAny<Statuses>(), It.IsAny<string>()))
                .ReturnsAsync(new ServiceItem());
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.Callback(It.IsAny<string>(), Statuses.STARTED.ToString()) as ContentResult;
            
            Assert.Equal(204, actual?.StatusCode);
        }
        
        [Fact]
        public async Task PutCallback_ShouldReturnNoContentOnSuccess()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();
            var serviceItemUpdatePayload = new ServiceItemUpdate
            {
                Status = "COMPLETED",
                Detail = "This request has been completed"
            };

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(new ServiceItem());
            mockServiceItemRepo.Setup(x => x.UpdateServiceItem(It.IsAny<Statuses>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ServiceItem());
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.Callback(It.IsAny<string>(), serviceItemUpdatePayload) as ContentResult;
            
            Assert.Equal(204, actual?.StatusCode);
        }
        
        [Fact]
        public async Task PutCallback_ShouldReturnBadRequestOnInvalidStatus()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();
            var serviceItemUpdatePayload = new ServiceItemUpdate
            {
                Status = "INVALID",
                Detail = "This request status is invalid"
            };

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(new ServiceItem());
            mockServiceItemRepo.Setup(x => x.UpdateServiceItem(It.IsAny<Statuses>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ServiceItem());
            
            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.Callback(It.IsAny<string>(), serviceItemUpdatePayload) as ContentResult;
            
            Assert.Equal(400, actual?.StatusCode);
        }

        [Fact]
        public async Task GetStatus_ShouldReturnServiceItemOnSuccess()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(new ServiceItem());

            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.GetStatus(It.IsAny<string>()) as ContentResult;
            
            Assert.NotNull(actual.Content);
            Assert.Equal(200, actual.StatusCode);
        }
        
        [Fact]
        public async Task GetStatus_ShouldReturnNotFoundWhenServiceItemNotExist()
        {
            var mockProxyService = new Mock<IProxyService>();
            var mockServiceItemRepo = new Mock<IServiceItemRepository>();

            mockServiceItemRepo.Setup((x => x.GetServiceItem((It.IsAny<string>())))).ReturnsAsync(() => null);

            var target = new ProxyController(mockProxyService.Object, mockServiceItemRepo.Object);
            var actual = await target.GetStatus(It.IsAny<string>()) as ContentResult;
            
            Assert.Equal(404, actual.StatusCode);
        }
    }
}