


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MasterCardServer {
    public class RoomItem { 

            [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int roomID{get;set; }
            public string SocketID{get;set;}
            public string RoomName{get;set;}
            public string Creator{get;set;}
            public bool isPrivate{get;set;}
            public string Password{get;set;}
            public string Ping{get;set;}
            public string CreateTime{get;set;} 
            public int roomMembers{get;set;}
    }
}