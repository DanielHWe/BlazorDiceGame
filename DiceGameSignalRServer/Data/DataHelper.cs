using DiceGameFunction.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGameSignalRServer.Data
{
    public class DataHelper
    {
        private static DataHelper _instance;
        private static object _syncRoot = new object();
        private Dictionary<String, PvPGameData> _openGames = new Dictionary<String, PvPGameData>();
        private Dictionary<String, PvPGameData> _runningGames = new Dictionary<String, PvPGameData>();

        public static DataHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {                            
                            _instance = new DataHelper();
                        }
                    }
                }

                return _instance;
            }
            set { _instance = value; }
        }

        public PvPGameData NewGame(String gameName, String player1Name, string userIdClaimValue)
        {
            var key = Guid.NewGuid().ToString();
            var entity = new PvPGameData()
            {                
                Key = key,                
                HostId = userIdClaimValue,
                Model = new PvPGameModel() { GameName = gameName, Id = key, HostName = player1Name, StartTime =DateTime.Now }
            };
            entity.Players.Add(new DiceGame.Interfaces.Messages.PlayerInfo() { Id = "0", Name = player1Name });

            _openGames.Add(key, entity);
            return entity;
        }

        internal void StartGame(string gameId)
        {
            if (!_openGames.ContainsKey(gameId)) throw new Exception("Unknown game");

            _runningGames.Add(gameId, _openGames[gameId]);
            _openGames.Remove(gameId);

        }

        internal int AddPlayer(string gameId, string name)
        {
            if (!_openGames.ContainsKey(gameId)) return -1;
            var id = _openGames[gameId].Players.Count;
            _openGames[gameId].Players.Add(new DiceGame.Interfaces.Messages.PlayerInfo() { Id = id.ToString(), Name = name });
            return id;
        }

        public PvPGameModel[] GetOpenGames()
        {
            return _openGames.Values.Select(g => g.Model).ToArray();
        }

        public DiceGame.Interfaces.Messages.PlayerInfo[] GetPlayerInfo(String gameId)
        {
            if (!_openGames.ContainsKey(gameId)) return null;

            return _openGames[gameId].Players.ToArray();
        }

        internal PvPGameData GetRunningGame(string gameId)
        {
            if (!_runningGames.ContainsKey(gameId)) throw new Exception("Unknown game");

            return _runningGames[gameId];            

        }
    }
}
