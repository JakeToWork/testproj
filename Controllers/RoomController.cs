using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 
using static MasterCardServer.Methods;
namespace MasterCardServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
       private ChatMessageHandler ChatMessageHandler {get;set;}
       
       public RoomController(ChatMessageHandler handler)
       {
           ChatMessageHandler = handler;
       }
     
        [HttpGet]
        public IEnumerable<RoomItem> Get()
        { 
            using(var context = getDb()){
                
                var rooms = context.Rooms.ToList().Select(x=>{ x.SocketID = null; x.Password = null; return x; }); 
                 
                
                return rooms;
            } 
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateRoom([FromBody] createRoom createRoom)
        {
            var sockId = createRoom.SocketID; 
            using(var context = getDb()){    
                if((createRoom is null)|| !context.Players.Where(x=>x.SockID==sockId).Any() || context.Rooms.Where(x=>x.SocketID==sockId).Any()){
                    return NotFound();
                }else{  
                   var room =  new RoomItem{
                         SocketID = createRoom.SocketID,
                         RoomName = createRoom.RoomName,
                         roomMembers = 1,
                         isPrivate = createRoom.isPrivate,
                         Password = createRoom.Password,
                         Creator = createRoom.PlayerName,
                         CreateTime = DateTime.UtcNow.ToString()
                     };
                     context.Rooms.Add(room);
                     await context.SaveChangesAsync(); 
                     var player = context.Players.Where(x=>x.SockID == sockId).Single(); 
                     player.roomID = room.roomID;
                     await context.SaveChangesAsync(); 
                return room.roomID;
                }
            }
          
        }
        [HttpPut]
        public async Task<ActionResult<string>> JoinRoom([FromBody] Player player)
        { 
            using(var context = getDb()){  
                var room = context.Rooms.Where(x=>x.roomID == player.roomID);
                if(!room.Any()){
                         return NotFound();
                }else{
                    var theroom = room.First();
                    if(theroom.roomMembers>=2){
                        return NotFound();
                    }else{ 
                        context.Players.Where(x=>x.SockID == player.SockID).Single().roomID = player.roomID; 
                        theroom.roomMembers = 2;
                        
                        await context.SaveChangesAsync();
                        
                        return Ok();
                    }
                }
            }
          
        }

        [HttpDelete]
        public async Task<ActionResult> CloseRoom([FromBody] string socketId)
        {
            using(var context = getDb()){
                var room = context.Rooms.Where(x=>x.SocketID==socketId);
                if(room.Any())
                    context.Rooms.Remove(room.Single());

                await context.SaveChangesAsync();
                return Ok();
            }
           
        }
    }
}
