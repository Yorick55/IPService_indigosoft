using IPservice_indigosoft.Data;
using System.Collections.Generic;

namespace IPservice_indigosoft
{
    public interface IConnectionService
    {
        public Task AddNewConnection(long userid, string ipaddress);
        public Task<List<string>> GetIPsByUser(long userid);
        public Task<UserConnection> GetUserLastConnection(long userid);
        public Task<List<long>> GetUsersByIP(string ipaddress);
    }
    public class ConnectionsService : IConnectionService
    {
        private UsersDbContext _usersDbContext;
        public ConnectionsService(UsersDbContext usersDbContext) 
        {
            _usersDbContext = usersDbContext;
        }
        public async Task AddNewConnection(long userid, string ipaddress)
        {
            DateTime connectedAt = DateTime.UtcNow;
            if (!_usersDbContext.Users.Any(usr => usr.Id == userid))
                await _usersDbContext.Users.AddAsync(new User { Id = userid });
            await _usersDbContext.Connections.AddAsync(new UserConnection { IpAddress = ipaddress, UserId = userid, ConnectedAt = connectedAt });
            await _usersDbContext.SaveChangesAsync();
        }
        public async Task<List<string>> GetIPsByUser(long userid)
        {
            var lst = await _usersDbContext.Connections.Where(con=>con.UserId==userid).Select(con=>con.IpAddress).ToListAsync();
            if (!lst.Any())
                throw new KeyNotFoundException("User not found"); 
            return lst;
        }
        public async Task<UserConnection> GetUserLastConnection(long userid)
        {
            var lastConn = await _usersDbContext.Connections.Where(con => con.UserId == userid).OrderByDescending(con=>con.ConnectedAt).FirstOrDefaultAsync();
            if (lastConn==null)
                throw new KeyNotFoundException("User not found");
            return lastConn;
        }
        public async Task<List<long>> GetUsersByIP(string ipaddress)
        {
            var users = await _usersDbContext.Connections.Where(con=>con.IpAddress.StartsWith(ipaddress)).Select(con=>con.UserId).Distinct().ToListAsync();
            if (!users.Any())
                throw new KeyNotFoundException("No users found for provided IP");
            return users;
        }
    }
}
