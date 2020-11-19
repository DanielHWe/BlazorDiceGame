using DiceGame.Helper;
using DiceGame.Interfaces.Helper;
using DiceGame.Interfaces.Messages;
using DiceGame.Model;
using DiceGame.Services;
using DiceGameFunction.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DiceGame.Logic
{
    public class PvPDiceGameLogic : IDiceGameLogic
    {
        public int _currentPlayerId = 0;
        
        private PvPNetworkHelper _networkHelper;
        private MoveInfo _currentMoveInfo;
        private Action _connectionLost;
        private GameService _service;

        private PvPStatus _status = new PvPStatus();
        public int LastDice { get; set; }
        public int LastMoveId { get; set; }
        public String LastDiceView { get; set; }

        public bool AllowMove { get; set; }
        public bool AllowDice { get; set; }

        public int MyPlayerId { get; set; }


        public PvPDiceGameLogic(IGameFieldModel model)
        {
            this.GameField = model;
            
        }

        public async Task<PvPStatus> Connect(String url, bool useFunction, JoinRequest joinRequest, HttpClient client, GameService service)
        {
            try
            {
                _service = service;
                CreateNetworkHelper(url, useFunction, client);

                await _networkHelper.ConnectToSignalRHub(service, this, joinRequest);

                int playerId;
               
                _status = new PvPStatus() { Connected = true, GameId = joinRequest.GameId };

                await _networkHelper.JoinGameOnServer(joinRequest);

                return _status;
            }
            catch (Exception ex)
            {
                if (ex.Message == "TypeError: Failed to fetch")
                {
                    _status = new PvPStatus() { Connected = false, GameId = ex.GetType().Name + ": CORS failed - " + ex.Message, Error = ex };
                }
                else 
                {
                    _status = new PvPStatus() { Connected = false, GameId = ex.GetType().Name + ": " + ex.Message, Error = ex };
                }
                return _status;
            }
        }

        private void CreateNetworkHelper(string url, bool useFunction, HttpClient client)
        {
            if (_networkHelper != null) return;
            INetworkUrlHelper urlHelper;
            if (useFunction)
            {
                urlHelper = new INetworkUrlHelperAzFuction();
            }
            else
            {
                urlHelper = new INetworkUrlHelperServer();
            }

            _networkHelper = new PvPNetworkHelper(url, client, urlHelper);
        }



        internal async Task SendStartPvPGame(String gameid)
        {
            await _networkHelper.SendStartPvPGame(gameid);
        }

        internal void MoveCallback(MoveInfo move)
        {
            if (move != null)
            {
                LastMoveId = move.MoveId;

                if (move.PlayerId == MyPlayerId)
                {
                    //Was the local player
                    if (_service != null) _service.DiceDone(null, null);
                    return;
                }

                var player = GameField.Player[move.PlayerId];                
                if (move.Finished)
                {
                    //Move                            
                    GameField.ResetChangedPieces();
                    if (move.MeepleId >= 0)
                    {
                        player.RemoteMove(move);
                    }

                    AllowMove = false;
                    if (move.Dice != 6)
                    {
                        SelectNextPlayer();
                    }
                    if(_service != null) _service.StateChanged(player.Name + " moved");
                }
                else
                {
                    //Dice
                    SetLastDiceInfo(move.Dice);
                    player.RemoteDice(move.Dice);
                    
                }
                if (_service != null) _service.DiceDone(LastDiceView, player.Name);
            }
        }

        internal void StartgameCallback(GameService service, string gameid)
        {
            _service = service;
            service.GameId = gameid;
            service.StartPvPGame(gameid);
            if (ActivePlayer.Id == MyPlayerId)
            {
                AllowDice = true;
            }
        }

        internal void JoinCallback(GameService service, JoinRequest join)
        {
            _service = service;
            if (_status != null) _status.GameId = join.GameId;
            GameField.Player[int.Parse(join.Id)].Name = join.Name;
            GameField.Player[int.Parse(join.Id)].IsPlaying = true;
            service.StateChanged($"Player {join.Name} joind game {join.GameId}");
            service.PlayerChanged();
        }

        public async Task<IEnumerable<MoveInfoExtended>> GetMovesFromServer(String gameId)
        {
            return (await _networkHelper.GetMovesFromServer(gameId)).Select((p, res) => new MoveInfoExtended(p, GameField.Player[p.PlayerId].Name));
        }

        public async Task<IEnumerable<MoveInfoExtended>> GetLocalMoves()
        {
            return (_networkHelper.GetLocalMoves()).Select((p, res) => new MoveInfoExtended(p, GameField.Player[p.PlayerId].Name));
        }

        internal async Task LoadPlayer(string id, string playerName, GameService gameService)
        {
            try
            {
                _service = gameService;
                var info = await _networkHelper.GetPlayerInfo(id);

                if (info != null)
                {
                    foreach (var p in info)
                    {
                        int idx = 0;
                        if (int.TryParse(p.Id, out idx) && idx >= 0 && idx < GameField.Player.Length)
                        {
                            if (!String.IsNullOrEmpty(p.Name))
                            {
                                GameField.Player[idx].Name = p.Name;
                                GameField.Player[idx].IsPlaying = true;
                                if (p.Name.Equals(playerName))
                                {
                                    MyPlayerId = idx;
                                    GameField.Player[idx].IsLocalPlayer = true;
                                }
                            } else
                            {
                                GameField.Player[idx].IsPlaying = false;
                            }
                        }
                        
                    }
                }
                gameService.PlayerChanged();
            }
            catch (Exception ex)
            {
                if (ex.Message == "TypeError: Failed to fetch")
                {
                    _status.Error = ex ;
                }
                else
                {
                    _status.Error = ex ;
                }                
            }
        }

        internal void UpdateCallbacks(GameService gameService,Action connectionLost)
        {
            _connectionLost = connectionLost;
            _service = gameService;
        }

        internal void Closed(Exception arg)
        {
            if (_service != null)
            {
                _service.StateError("Connection Lost: " + arg.Message);
            }
            if (_connectionLost !=null)
            {
                _connectionLost();
            }
        }

        internal async Task<PvPStatus> LoadGames(string url, bool useFunction, HttpClient client)
        {
            CreateNetworkHelper(url, useFunction, client);
            return await _networkHelper.LoadGames();
        }

        public IGameFieldModel GameField { get; private set; }

        public IPlayerModel ActivePlayer
        {
            get
            {
                return GameField.Player[_currentPlayerId];
            }
        }

        private void SetLastDiceInfo(int curr)
        {
            LastDice = curr;
            LastDiceView = DiceUnicode.GetUnicode(LastDice).ToString();
        }

        private void SelectNextPlayer()
        {
            ActivePlayer.IsActive = false;
            do
            {
                _currentPlayerId++;
                if (_currentPlayerId >= GameField.Player.Length || GameField.Player[_currentPlayerId] == null)
                {
                    _currentPlayerId = 0;
                }
            } while (!GameField.Player[_currentPlayerId].IsPlaying);
            ActivePlayer.IsActive = true;

            if (ActivePlayer.Id == MyPlayerId)
            {
                AllowDice = true;
            }
        }

        public async Task DoDice()
        {
            if (AllowDice)
            {
               
                var curr = ActivePlayer;
                curr.DiceAndCalcMove();

                _currentMoveInfo = new MoveInfo() { GameId = this._service.GameId, Dice = curr.LastDice, MeepleId = -1, PlayerId = curr.Id, Finished = false };
                await _networkHelper.SendMoveToServer(_currentMoveInfo);

                SetLastDiceInfo(curr.LastDice);

                if (curr.PossibleMoves.Any())
                {
                    AllowDice = false;
                    AllowMove = true;
                }
                else
                {
                    if (LastDice != 6)
                    {
                        _currentMoveInfo.Finished = true;
                        _currentMoveInfo.MeepleId = -1;
                        await _networkHelper.SendMoveToServer(_currentMoveInfo);
                        SelectNextPlayer();                        
                    }
                } 
            }
        }

        public async Task<IMoveModel> DoMove()
        {
            if (AllowMove)
            {

                var curr = ActivePlayer;
                var result = curr.Move();
                
                _currentMoveInfo.MeepleId = result.Meeple.MeepleId;
                _currentMoveInfo.Finished = true;
                await _networkHelper.SendMoveToServer(_currentMoveInfo);                

                AllowMove = false;
                if (LastDice == 6)
                {
                    AllowDice = true;
                } else
                {
                    AllowDice = false;
                    SelectNextPlayer();
                }
                return result;
            }
            return null;
        }

        public IEnumerable<IMoveModel> GetPossibleKicks()
        {
            if (!ActivePlayer.IsLocalPlayer || ActivePlayer.PossibleMoves == null) return new List<IMoveModel>();            
            return ActivePlayer.PossibleMoves.Where(m => m.ThrownMeeple != null).ToList();
        }

        public async Task<bool> TryMove(IPieceModel piece)
        {
            if (AllowMove)
            {
                var meepleId = piece.Meeple != null ? piece.Meeple.MeepleId : -1;
                var curr = ActivePlayer;
                var result = curr.TryMove(piece, GameField.ResetChangedPieces);
                if (meepleId < 0) meepleId = piece.Meeple != null ? piece.Meeple.MeepleId : meepleId;

                _currentMoveInfo.MeepleId = meepleId;
                _currentMoveInfo.Finished = true;
                await _networkHelper.SendMoveToServer(_currentMoveInfo);

                AllowMove = false;
                if (LastDice == 6)
                {
                    AllowDice = true;
                }
                else
                {
                    AllowDice = false;
                    SelectNextPlayer();
                }
                return result;
            }
            return false;
        }
    }

   
}
