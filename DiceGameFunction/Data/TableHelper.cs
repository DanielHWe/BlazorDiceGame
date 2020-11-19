using DiceGame.Interfaces.Messages;
using DiceGameFunction.Model;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceGameFunction.Data
{
    public class TableHelper
    {
        internal static StorageHelper _storage = new StorageHelper();
        private static TableHelper _instance;
        private static CloudTable _gameTable;

        public static TableHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_storage)
                    {
                        if (_instance == null)
                        {
                            _storage.LoadConnectionString();
                            _instance = new TableHelper();
                        }
                    }
                }

                return _instance;
            }
            set { _instance = value; }
        }

        public async Task<PvPGameEntity> NewGame(String gameName, String player1Name, string userIdClaimValue)
        {
            var entity = new PvPGameEntity()
            {
                PartitionKey = "game",
                RowKey = Guid.NewGuid().ToString(),
                Player1Name = player1Name,
                OpenForJoin = true,
                GameName = gameName,
                HostId = userIdClaimValue
            };

            var table = await this.GetGameTable();
            TableOperation operation = TableOperation.Insert(entity);

            await table.ExecuteAsync(operation);
            return entity;
        }

        public async Task<PvPGameModel[]> GetOpenGames()
        {            
            var table = await this.GetGameTable();
            
            TableQuery<PvPGameEntity> query = new TableQuery<PvPGameEntity>().Where(TableQuery.GenerateFilterConditionForBool("OpenForJoin", QueryComparisons.Equal, true));

            var result = await table.ExecuteQueryAsync<PvPGameEntity>(query);
            return result.Select(o => new PvPGameModel() { GameName = o.GameName, Id = o.RowKey}).ToArray();
        }

        internal async Task<int> AddPlayer(string gameId, string name)
        {
            int result = -1;
            var table = await this.GetGameTable();

            TableOperation retrieveOperation =
                TableOperation.Retrieve<PvPGameEntity>("game", gameId);


            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);
            var tableResult = retrievedResult.Result as PvPGameEntity;

            var entity = new DynamicTableEntity(tableResult.PartitionKey, tableResult.RowKey);
            entity.ETag = tableResult.ETag;
            //TODO Ensure Name is qnique

            if (String.IsNullOrEmpty(tableResult.Player2Name))
            {
                result = 1;
                entity.Properties.Add("Player2Name", new EntityProperty(name));
            } else if (String.IsNullOrEmpty(tableResult.Player3Name))
            {
                result = 2;
                entity.Properties.Add("Player3Name", new EntityProperty(name));
            }
            else if (String.IsNullOrEmpty(tableResult.Player4Name))
            {
                result = 3;
                entity.Properties.Add("Player4Name", new EntityProperty(name));
            }
            else if (String.IsNullOrEmpty(tableResult.Player5Name))
            {
                result = 4;
                entity.Properties.Add("Player5Name", new EntityProperty(name));
            }
            else if (String.IsNullOrEmpty(tableResult.Player6Name))
            {
                result = 5;
                entity.Properties.Add("Player6Name", new EntityProperty(name));
            }
            else if (String.IsNullOrEmpty(tableResult.Player7Name))
            {
                result = 6;
                entity.Properties.Add("Player7Name", new EntityProperty(name));
            }
            else if (String.IsNullOrEmpty(tableResult.Player8Name))
            {
                result = 7;
                entity.Properties.Add("Player8Name", new EntityProperty(name));
            }
            return result;
        }

        public async Task<PlayerInfo[]> GetPlayerInfo(String gameId)
        {
            var table = await this.GetGameTable();

            TableOperation retrieveOperation =
                TableOperation.Retrieve<PvPGameEntity>("game", gameId);


            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);
            var tableResult = retrievedResult.Result as PvPGameEntity;

            if (tableResult == null) return null;
            List<PlayerInfo> result = new List<PlayerInfo>();
            if (!String.IsNullOrEmpty(tableResult.Player1Name)) result.Add(new PlayerInfo() { Id = "0", Name = tableResult.Player1Name });
            if (!String.IsNullOrEmpty(tableResult.Player2Name)) result.Add(new PlayerInfo() { Id = "1", Name = tableResult.Player2Name });
            if (!String.IsNullOrEmpty(tableResult.Player3Name)) result.Add(new PlayerInfo() { Id = "2", Name = tableResult.Player3Name });
            if (!String.IsNullOrEmpty(tableResult.Player4Name)) result.Add(new PlayerInfo() { Id = "3", Name = tableResult.Player4Name });
            if (!String.IsNullOrEmpty(tableResult.Player5Name)) result.Add(new PlayerInfo() { Id = "4", Name = tableResult.Player5Name });
            if (!String.IsNullOrEmpty(tableResult.Player6Name)) result.Add(new PlayerInfo() { Id = "5", Name = tableResult.Player6Name });
            if (!String.IsNullOrEmpty(tableResult.Player7Name)) result.Add(new PlayerInfo() { Id = "6", Name = tableResult.Player7Name });
            if (!String.IsNullOrEmpty(tableResult.Player8Name)) result.Add(new PlayerInfo() { Id = "7", Name = tableResult.Player8Name });            

            return result.ToArray();
        }

        #region Helper
        public static void ResetCache()
        {
            _gameTable = null;            
        }

        private async Task<CloudTable> GetGameTable()
        {
            if (_gameTable == null)
            {
                _gameTable = _storage.GetTable("games");
                await _gameTable.CreateIfNotExistsAsync();
            }

            return _gameTable;
        }
        #endregion
    }
}
