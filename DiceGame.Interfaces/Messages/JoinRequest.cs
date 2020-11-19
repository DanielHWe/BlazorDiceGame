using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGame.Interfaces.Messages
{
    public class JoinRequest
    {
        public String GameId { get; set; }
        public String GameName { get; set; }
        public String Id { get; set; }
        public String Name { get; set; }
        public String TechnicalClientId { get; set; }
    }
}
