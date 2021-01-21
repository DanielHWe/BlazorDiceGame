using DiceGame.Interfaces.Helper;
using DiceGame.Interfaces.Messages;
using DiceGame.Logic;
using DiceGame.Model;
using DiceGameFunction.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace DiceGame.Services
{
    public class GameService: IDiceGameLogic
    {
        internal static string BaseUrlOfFunction { get; set; }
        private const bool useFunction = false;
        private readonly HttpClient client;
        private IDiceGameLogic _logic;
        public event Action<String> OnChange;
        public event Action<String, String> OnDiceDone;
        public event Action OnPlayerChange;
        public event Action<String> OnStart;
        public event Action<String> OnError;

        public GameService(HttpClient client)
        {            
            this.client = client;
            //client.DefaultRequestHeaders.Add("");
            _logic = new SinglePlayerDiceGameLogic(new Model.GameFieldModel());
        }        

        public int LastDice { get => _logic.LastDice; }

        public int LastMoveId {
            get => _logic.LastMoveId;
    }

        public string LastDiceView { get => _logic.LastDiceView; }

        public bool AllowMove { get => _logic.AllowMove; }

        public bool AllowDice { get => _logic.AllowDice; }

        public bool DisableDice
        {
            get
            {
                if (this.PvPStatus != null) {
                    if (!_logic.ActivePlayer.IsLocalPlayer) return true;
                    return false;
                }
                return _logic.AllowMove;
            }
        }

        public bool IsLocalGame { get => PvPStatus == null; }
        public IGameFieldModel GameField { get => _logic.GameField; }

        public IPlayerModel ActivePlayer { get => _logic.ActivePlayer; }

        public JoinRequest JoinRequest { get; set; }

        public PvPStatus PvPStatus { get; set; }
        public string GameId { get; set; }

        public async Task DoDice()
        {
            try
            {
                var header = ActivePlayer.Name;
                await _logic.DoDice();
                DiceDone(_logic.LastDiceView, header);
            } catch (Exception ex)
            {
                StateError(ex.Message);
            }
        }

        public async Task<IMoveModel> DoMove()
        {
            return await _logic.DoMove();
        }

        public async Task<IEnumerable<MoveInfoExtended>> GetMoves()
        {
            if (this.GameId == null) return new List<MoveInfoExtended>();
            return await _logic.GetMovesFromServer(this.GameId);
        }

        public async Task<IEnumerable<MoveInfoExtended>> GetLocalMoves()
        {
            if (this.GameId == null) return new List<MoveInfoExtended>();
            return await _logic.GetLocalMoves();
        }

        public IEnumerable<IMoveModel> GetPossibleKicks()
        {
            return _logic.GetPossibleKicks();
        }

        public async Task<IMoveModel> TryMove(IPieceModel piece)
        {
            return await _logic.TryMove(piece);
        }

        public async Task ActivatePvP(PvPGameModel game)
        {
            if (PvPStatus == null && game == null)
            {
                await StartNewGame();
            }
            else
            {
                await JoinGame(game);
            }
        }

        internal void StartPvPGame(string gameid)
        {
            OnChange?.Invoke(gameid);
            GameStarted("Started: " + gameid);
        }

        internal async Task SendStartPvPGame()
        {
            try
            {
                await ((PvPDiceGameLogic)_logic).SendStartPvPGame(PvPStatus.GameId);

            }
            catch (Exception ex)
            {
                StateError("Error on start Game: " + ex.Message);
            }
        }

        private async Task JoinGame(PvPGameModel game)
        {
            this.JoinRequest.GameId = game.Id;
            this.GameId = game.Id;
            this.JoinRequest.GameName = game.GameName;            
            PvPStatus = await((PvPDiceGameLogic)_logic).Connect(BaseUrlOfFunction, useFunction, this.JoinRequest, client, this);
            
            if (PvPStatus.Error != null)
            {
                StateError("Join to game: " + PvPStatus.Error.Message + "\r\n" + PvPStatus.Error.StackTrace);
            }
            else if (!PvPStatus.Connected)
            {
                StateChanged(null);
            }
            else
            {
                StateChanged("Player" + this.JoinRequest.Name +  " Connected to Server");
                await ((PvPDiceGameLogic)_logic).LoadPlayer(game.Id, this.JoinRequest.Name, this);
                PlayerChanged();
                if (PvPStatus.Error != null)
                {
                    StateError("Load player: " + PvPStatus.Error.Message);
                } else
                {
                    StateChanged(null);
                }
            }
        }

        public async Task LoadPvPGames()
        {
            _logic = new PvPDiceGameLogic(_logic.GameField);
            PvPStatus = await ((PvPDiceGameLogic)_logic).LoadGames(BaseUrlOfFunction, useFunction, client);
            if (PvPStatus.Error != null)
            {
                StateError(PvPStatus.Error.Message);
            }
            else
            {
                StateChanged("Load Remote Games");
            }
        }

        private async Task StartNewGame()
        {
            _logic = new PvPDiceGameLogic(_logic.GameField);
            for (int i = 0; i < 8; i++)
            {
                _logic.GameField.Player[i].IsPlaying = false;
            }

            PvPStatus = await ((PvPDiceGameLogic)_logic).Connect(BaseUrlOfFunction, useFunction, this.JoinRequest, client, this);
            if (PvPStatus.Error != null)
            {
                StateError(PvPStatus.Error.Message);
            }
            else if (!PvPStatus.Connected)
            {
                StateChanged(null);
            }
            else
            {
                ((PvPDiceGameLogic)_logic).MyPlayerId = 0;
                GameField.Player[0].IsLocalPlayer = true;
                if (String.IsNullOrEmpty(GameId)) GameId = PvPStatus.GameId;
                StateChanged("Connected to Server");
            }
            PlayerChanged();
        }

        public void UpdateCallbacks(Action connectionLost)
        {
            var pvpLogic = _logic as PvPDiceGameLogic;
            if (pvpLogic!=null) pvpLogic.UpdateCallbacks(this, connectionLost);
        }

        internal void StateChanged(string message) => OnChange?.Invoke(message);

        internal void PlayerChanged() => OnPlayerChange?.Invoke();

        internal void StateError(string message) => OnError?.Invoke(message);

        internal void GameStarted(string message) => OnStart?.Invoke(message);

        internal void DiceDone(string diceView, string header) => OnDiceDone?.Invoke(diceView, header);

        public async Task<IEnumerable<MoveInfoExtended>> GetMovesFromServer(string gameId)
        {
            return await GetMoves();
        }

       public bool DiceAnimation { get; set; }

        public bool Approved { get; set; }
        public Action OnApproved { get; set; }
    }
}
