using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        public ChatsController()
        {
            
        }

        [HttpGet]
        public async Task<bool> GetChat()
        {
            return true;
        }
    }
}
