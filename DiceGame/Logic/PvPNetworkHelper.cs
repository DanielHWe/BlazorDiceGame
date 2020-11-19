using DiceGame.Interfaces.Helper;
using DiceGame.Interfaces.Messages;
using DiceGame.Model;
using DiceGame.Services;
using DiceGameFunction.Model;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame.Logic
{
    public class PvPNetworkHelper
    {
        private String _baseUrl;
        private HttpClient _client;
        private HubConnection _hubConnection;
        private INetworkUrlHelper _urlHelper;
        private PvPDiceGameLogic _logic;
        private List<MoveInfo> _ownMoves = new List<MoveInfo>();

        public PvPNetworkHelper(String url, HttpClient client, INetworkUrlHelper urlHelper)
        {
            _baseUrl = url;
            _client = client;
            _urlHelper = urlHelper;
        }

        public async Task<PlayerInfo[]> GetPlayerInfo(String gameId)
        {
            var result = await _client.GetAsync(_urlHelper.GetPlayerInfoUrl(_baseUrl, gameId));
            result.EnsureSuccessStatusCode();
            var info = JsonConvert.DeserializeObject<PlayerInfo[]>(await result.Content.ReadAsStringAsync());
            return info;
        }

        public async Task<MoveInfo[]> GetMovesFromServer(String gameId)
        {
            var result = await _client.GetAsync(_urlHelper.GetMoveInfoUrl(_baseUrl, gameId));
            result.EnsureSuccessStatusCode();
            var info = JsonConvert.DeserializeObject<MoveInfo[]>(await result.Content.ReadAsStringAsync());
            return info;
        }

        public async Task<PvPStatus> LoadGames()
        {
            try
            {
                var result = await _client.GetAsync(_urlHelper.GetListGamesUrl(_baseUrl));
                result.EnsureSuccessStatusCode();
                var info = JsonConvert.DeserializeObject<PvPGameModel[]>(await result.Content.ReadAsStringAsync());

                var status = new PvPStatus() { Connected = false, GameId = "?", OpenGames = info };


                return status;
            }
            catch (Exception ex)
            {
                return new PvPStatus() { Connected = false, GameId = ex.GetType().Name + ": " + ex.Message, Error = ex };                
            }
        }

        internal async Task SendStartPvPGame(String gameid)
        {
            var callInfo = _urlHelper.GetStartGameUrl(_baseUrl);
            if (callInfo.CallBySignalR)
            {
                await _hubConnection.SendAsync("StartGame", gameid);
            } else {
                var content = new StringContent(gameid, Encoding.UTF8, "application/json");
                var resultJoin = await _client.PostAsync(callInfo.Url, content);
                resultJoin.EnsureSuccessStatusCode();
            }
        }


        internal async Task JoinGameOnServer(JoinRequest joinRequest)
        {
            await _urlHelper.JoinGameOnServer(joinRequest, _baseUrl, _client);
            
        }

        internal async Task ConnectToSignalRHub(GameService service, PvPDiceGameLogic pvPDiceGameLogic, JoinRequest joinRequest)
        {
            var neotigation = await _urlHelper.GetSignalRNeogation(_baseUrl, _client);
            await ConnectToSignalRHubImpl(service, neotigation, pvPDiceGameLogic);
            _urlHelper.SetGub(_hubConnection);
            joinRequest.TechnicalClientId = neotigation.accessToken;
        }

        internal async Task SendMoveToServer(MoveInfo currentMoveInfo)
        {
            if (!currentMoveInfo.Finished && currentMoveInfo.MeepleId >= 0)
            {
                throw new Exception("If a meeple is set a move should be finished!");
            }
            try
            {
                var callInfo = _urlHelper.GetMoveUrl(_baseUrl);
                if (callInfo.CallBySignalR)
                {
                    await _hubConnection.SendAsync("move", currentMoveInfo);
                } else {
                    var content = new StringContent(JsonConvert.SerializeObject(currentMoveInfo), Encoding.UTF8, "application/json");
                    var resultJoin = await _client.PostAsync(callInfo.Url, content);
                    resultJoin.EnsureSuccessStatusCode();
                }
                _ownMoves.Add(currentMoveInfo.Clone());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to perform move on server: " + ex.Message, ex);
            }
        }

        private async Task ConnectToSignalRHubImpl(GameService service, SignalRNeogation info, PvPDiceGameLogic logic)
        {
            try
            {
                if (info.accessToken == null)
                {
                    _hubConnection = new HubConnectionBuilder()
                                .WithUrl(info.url)
                                .Build();
                }
                else
                {
                    _hubConnection = new HubConnectionBuilder()
                                    .WithUrl(info.url, option => GetSignalROptions(info, option))
                                    .Build();
                }

                _hubConnection.On("join", (Action<JoinRequest>)((join) =>
                {
                    logic.JoinCallback(service, join);
                }));

                _hubConnection.On("StartGame", (Action<string>)((gameid) => logic.StartgameCallback(service, gameid)));

                _hubConnection.On("move", (Action<MoveInfo>)((move) =>
                {
                    logic.MoveCallback(move);
                }));

                _hubConnection.Closed += _hubConnection_Closed;

                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                if (info.accessToken == null)
                    throw new Exception("Failed to init SignalR Hub on '"+ info.url+ "': " + ex.Message, ex);
                throw new Exception("Failed to init SignalR Hub (without token) on '" + info.url + "': " + ex.Message, ex);
            }
        }

        internal IEnumerable<MoveInfo> GetLocalMoves()
        {
            return _ownMoves;
        }

        private async Task _hubConnection_Closed(Exception arg)
        {
            _logic.Closed(arg);
        }

        private void GetSignalROptions(SignalRNeogation info, HttpConnectionOptions option)
        {
            option.AccessTokenProvider = () => Task.FromResult(info.accessToken);
        }
    }
}
