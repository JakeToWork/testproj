using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static MasterCardServer.Methods;
using System.Linq;
using System.Collections.Generic;

namespace MasterCardServer.Controllers{

    [Route("[controller]")]    
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Player> Get(){
             using(var context = getDb()){   
                  return context.Players.ToList();
              }
        }
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] Player player){
              using(var context = getDb()){    
                  context.Players.Add(new Player{playerName = player.playerName,SockID = player.SockID});
                  await context.SaveChangesAsync();
                  return Ok();
              }
        }
    }


    [Route("[controller]")]    
    [ApiController]
    public class MessageController : ControllerBase
    {
        [HttpGet]
        public Message Get(){
            return new Message
            {
              fromSockID = "From",
              toSockID = "To",
              messageType = Message.type.player  
            };
        } 
    }
}