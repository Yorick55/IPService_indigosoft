using IPservice_indigosoft.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Net;
using System.Text.RegularExpressions;

namespace IPservice_indigosoft.Controllers
{
    public class ConnectionsController : Controller
    {
        private IConnectionService _connectionService;
        public ConnectionsController(IConnectionService connectionsService)
        {
            _connectionService = connectionsService;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddNewConnection(long userid, string ipaddress)
        {
            try
            {
                if (!IPAddress.TryParse(ipaddress, out var address))
                    throw new ArgumentException("Wrong ip-address format");
                await _connectionService.AddNewConnection(userid, ipaddress);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIPsByUser(long userid)
        {
            try
            {
                var lst = await _connectionService.GetIPsByUser(userid);
                return Ok(lst);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLastConnectionByUser(long userid)
        {
            try
            {
                var conn = await _connectionService.GetUserLastConnection(userid);
                return Ok(conn);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersByIP(string ipaddress)
        {
            try
            {
                if (!Regex.IsMatch(ipaddress, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){0,3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.)?$"))
                    throw new ArgumentException("Wrong ip-address format");
                var users = await _connectionService.GetUsersByIP(ipaddress);
                return Ok(users);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
