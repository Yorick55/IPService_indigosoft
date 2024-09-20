using IPservice_indigosoft;
using IPservice_indigosoft.Data;
using IPservice_indigosoft.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace TestIPService
{
    [TestClass]
    public class TestConnectionsService
    {
        private UsersDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            return new UsersDbContext(options);
        }

        [TestMethod]
        public async Task AddNewConnection_AddsConnectionSuccessfully()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ConnectionsService(dbContext);

            await service.AddNewConnection(100001, "127.0.0.1");

            var user = await dbContext.Users.Include(u => u.Connections).FirstOrDefaultAsync(u => u.Id == 100001);
            Assert.IsNotNull(user);
            var connection = user.Connections.FirstOrDefault(con => con.IpAddress == "127.0.0.1");
            Assert.IsNotNull(connection, "New connection ip for provided user was not found.");
        }

        [TestMethod]
        public async Task GetIPsByUser_ReturnsListOfIPs()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ConnectionsService(dbContext);
            long userId = 100002;

            dbContext.Users.Add(new User
            {
                Id = userId,
                Connections = new List<UserConnection>
                {
                    new UserConnection { IpAddress = "192.168.0.1", UserId = userId, ConnectedAt = DateTime.UtcNow },
                    new UserConnection { IpAddress = "192.168.0.2", UserId = userId, ConnectedAt = DateTime.UtcNow }
                }
            });
            await dbContext.SaveChangesAsync();

            var ips = await service.GetIPsByUser(userId);

            Assert.AreEqual(2, ips.Count);
            Assert.IsTrue(ips.Contains("192.168.0.1"));
            Assert.IsTrue(ips.Contains("192.168.0.2"));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await service.GetIPsByUser(333333);
            });
        }

        [TestMethod]
        public async Task GetUserLastConnection_ReturnsLatestConnection()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ConnectionsService(dbContext);
            long userId = 100003;

            dbContext.Users.Add(new User
            {
                Id = userId,
                Connections = new List<UserConnection>
                {
                    new UserConnection { IpAddress = "192.168.0.1", UserId = userId, ConnectedAt = DateTime.UtcNow.AddHours(-1) },
                    new UserConnection { IpAddress = "192.168.0.2", UserId = userId, ConnectedAt = DateTime.UtcNow }
                }
            });
            await dbContext.SaveChangesAsync();

            var lastConnection = await service.GetUserLastConnection(userId);

            Assert.IsNotNull(lastConnection);
            Assert.AreEqual("192.168.0.2", lastConnection.IpAddress);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await service.GetUserLastConnection(333333);
            });
        }

        [TestMethod]
        public async Task GetUsersByIP_ReturnsUsersWithGivenIP()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ConnectionsService(dbContext);
            long userId1 = 100004;
            long userId2 = 100005;

            dbContext.Users.Add(new User
            {
                Id = userId1,
                Connections = new List<UserConnection>
                {
                    new UserConnection { IpAddress = "62.4.36.194", UserId = userId1, ConnectedAt = DateTime.UtcNow },
                    new UserConnection { IpAddress = "31.214.157.141", UserId = userId1, ConnectedAt = DateTime.UtcNow },
                    new UserConnection { IpAddress = "192.168.10.10", UserId = userId1, ConnectedAt = DateTime.UtcNow }
                }
            });
            dbContext.Users.Add(new User
            {
                Id = userId2,
                Connections = new List<UserConnection>
                {
                    new UserConnection { IpAddress = "31.214.10.1", UserId = userId2, ConnectedAt = DateTime.UtcNow },
                    new UserConnection { IpAddress = "188.32.1.1", UserId = userId2, ConnectedAt = DateTime.UtcNow },
                    new UserConnection { IpAddress = "192.168.10.10", UserId = userId1, ConnectedAt = DateTime.UtcNow }
                }
            });
            await dbContext.SaveChangesAsync();

            var users1 = await service.GetUsersByIP("192.168.10.10");
            Assert.AreEqual(2, users1.Count);
            Assert.IsTrue(users1.Contains(userId1));
            Assert.IsTrue(users1.Contains(userId2));

            var users2 = await service.GetUsersByIP("31.214.10.1");
            Assert.AreEqual(1, users2.Count);
            Assert.IsTrue(users2.Contains(userId2));

            var users3 = await service.GetUsersByIP("31.214");
            Assert.AreEqual(2, users3.Count);
            Assert.IsTrue(users3.Contains(userId1));
            Assert.IsTrue(users3.Contains(userId2));

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await service.GetUsersByIP("10.10.10.10");
            });
        }

    }
}