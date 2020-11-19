using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiceGameFunction.Data
{
    public class PvPGameEntity: TableEntity
    {
        public String Id { get; set; }

        public String GameName { get; set; }
        public String HostId { get; set; }

        public String Player1Name { get; set; }
        public String Player2Name { get; set; }
        public String Player3Name { get; set; }
        public String Player4Name { get; set; }
        public String Player5Name { get; set; }
        public String Player6Name { get; set; }
        public String Player7Name { get; set; }
        public String Player8Name { get; set; }

        public bool OpenForJoin { get; set; }
    }
}
