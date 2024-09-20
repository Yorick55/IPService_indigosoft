using IPservice_indigosoft.Controllers;
using IPservice_indigosoft.Models;
using IPservice_indigosoft;
using Moq;
using Microsoft.AspNetCore.Mvc;


namespace TestIPService
{
    [TestClass]
    public class TestConnectionsController
    {
        [TestMethod]
        public async Task AddNewConnection_ReturnsOk_WhenValidDataProvided()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 100001;
            string ipAddress = "127.0.0.1";

            mockService.Setup(s => s.AddNewConnection(It.IsAny<long>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await controller.AddNewConnection(userId, ipAddress);

            Assert.IsInstanceOfType(result, typeof(OkResult));
            mockService.Verify(s => s.AddNewConnection(userId, ipAddress), Times.Once);
        }
        [TestMethod]
        public async Task AddNewConnection_ReturnsBadRequest_WhenInvalidIpProvided()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 999;
            string invalidIp = "InvalidIP";

            var result = await controller.AddNewConnection(userId, invalidIp);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Wrong ip-address format", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetIPsByUser_ReturnsOk_WithListOfIPs()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 100002;
            var ipList = new List<string> { "192.168.0.1", "192.168.0.2" };

            mockService.Setup(s => s.GetIPsByUser(userId)).ReturnsAsync(ipList);

            var result = await controller.GetIPsByUser(userId);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            CollectionAssert.AreEqual(ipList, okResult.Value as List<string>);
        }
        [TestMethod]
        public async Task GetIPsByUser_ReturnsNotFound_WhenUserNotExists()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 999; 

            mockService.Setup(s => s.GetIPsByUser(userId)).ThrowsAsync(new KeyNotFoundException("User not found"));

            var result = await controller.GetIPsByUser(userId);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetLastConnectionByUser_ReturnsOk_WithLastConnection()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 1;
            var connection = new UserConnection { UserId = userId, IpAddress = "192.168.0.1", ConnectedAt = DateTime.UtcNow };

            mockService.Setup(s => s.GetUserLastConnection(userId)).ReturnsAsync(connection);

            var result = await controller.GetLastConnectionByUser(userId);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual(connection, okResult.Value);
        }
        [TestMethod]
        public async Task GetLastConnectionByUser_ReturnsNotFound_WhenUserNotExists()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            long userId = 999; // Non-existing user

            mockService.Setup(s => s.GetUserLastConnection(userId)).ThrowsAsync(new KeyNotFoundException("User not found"));

            var result = await controller.GetLastConnectionByUser(userId);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetUsersByIP_ReturnsOk_WithListOfUserIds()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            string ipAddress = "192.168.0";
            var userIds = new List<long> { 1, 2 };

            mockService.Setup(s => s.GetUsersByIP(ipAddress)).ReturnsAsync(userIds);

            var result = await controller.GetUsersByIP(ipAddress);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            CollectionAssert.AreEqual(userIds, okResult.Value as List<long>);
        }
        [TestMethod]
        public async Task GetUsersByIP_ReturnsNotFound_WhenNoUsersFound()
        {
            var mockService = new Mock<IConnectionService>();
            var controller = new ConnectionsController(mockService.Object);
            string ipAddress = "192.168.0";

            mockService.Setup(s => s.GetUsersByIP(ipAddress)).ThrowsAsync(new KeyNotFoundException("No users found for provided IP"));

            var result = await controller.GetUsersByIP(ipAddress);

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("No users found for provided IP", notFoundResult.Value);
        }


    }
}
