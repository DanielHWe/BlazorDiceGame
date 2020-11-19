using DiceGame.Interfaces.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame.Interfaces.Helper
{
    public interface INetworkUrlHelper
    {
        String GetNeogationUrl(String baseUrl);
        String GetJoinUrl(String baseUrl);
        NetworkCallInfo GetMoveUrl(String baseUrl);
        String GetListGamesUrl(String baseUrl);
        String GetPlayerInfoUrl(String baseUrl, String gameId);

        String GetMoveInfoUrl(String baseUrl, String gameId);

        Task<SignalRNeogation> GetSignalRNeogation(String baseUrl, HttpClient client);
        void SetGub(HubConnection hubConnection);
        Task JoinGameOnServer(JoinRequest joinRequest, String baseUrl, HttpClient client);
        NetworkCallInfo GetStartGameUrl(string baseUrl);
    }

    public class INetworkUrlHelperAzFuction : INetworkUrlHelper
    {
        private HubConnection _hubConnection;

        public string GetNeogationUrl(string baseUrl)
        {
            return baseUrl + "negotiate";
        }

        public string GetJoinUrl(string baseUrl)
        {
            return baseUrl + "join";
        }
        public NetworkCallInfo GetMoveUrl(string baseUrl)
        {
            return new NetworkCallInfo(baseUrl + "Move", false);
        }
        public String GetListGamesUrl(String baseUrl)
        {
            return baseUrl + "ListGames";
        }
        public String GetPlayerInfoUrl(String baseUrl, String gameId)
        {
            return baseUrl + "PlayerInfo?gameId=" + gameId;
        }
        public String GetMoveInfoUrl(String baseUrl, String gameId)
        {
            return baseUrl + "MoveInfo?gameId=" + gameId;
        }

        public NetworkCallInfo GetStartGameUrl(string baseUrl)
        {
            return new NetworkCallInfo(baseUrl + "StartGame", false);
        }

        public async Task<SignalRNeogation> GetSignalRNeogation(String baseUrl, HttpClient client)
        {
            try
            {
                var result = await client.PostAsync(this.GetNeogationUrl(baseUrl), null);
                result.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<SignalRNeogation>(await result.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to negotiate SignalR server: " + ex.Message, ex);
            }
        }

        public void SetGub(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public async Task JoinGameOnServer(JoinRequest joinRequest, String baseUrl, HttpClient client)
        {
            try
            {
                var request = JsonConvert.SerializeObject(joinRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var resultJoin = await client.PostAsync(this.GetJoinUrl(baseUrl), content);
                resultJoin.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to join game on server: " + ex.Message, ex);
            }
        }

        
    }

    public class INetworkUrlHelperServer : INetworkUrlHelper
    {
        private HubConnection _hubConnection;

        public string GetNeogationUrl(string baseUrl)
        {
            return baseUrl + "negotiate";
        }

        public string GetJoinUrl(string baseUrl)
        {
            return baseUrl + "Game/join";
        }
        

        public NetworkCallInfo GetMoveUrl(string baseUrl)
        {
            return new NetworkCallInfo(baseUrl + "gamehub", true);
        }
        public String GetListGamesUrl(String baseUrl)
        {
            return baseUrl + "Game/ListGames";
        }
        public String GetPlayerInfoUrl(String baseUrl, String gameId)
        {
            return baseUrl + "Game/PlayerInfo?gameId=" + gameId;
        }
        public String GetMoveInfoUrl(String baseUrl, String gameId)
        {
            return baseUrl + "Game/MoveInfo?gameId=" + gameId;
        }

        public NetworkCallInfo GetStartGameUrl(string baseUrl)
        {
            return new NetworkCallInfo(baseUrl + "gamehub", true);            
        }

        public async Task<SignalRNeogation> GetSignalRNeogation(String baseUrl, HttpClient client)
        {
            try
            {
                return new SignalRNeogation()
                {
                    accessToken = null,
                    url = baseUrl + "gamehub"
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to negotiate SignalR server: " + ex.Message, ex);
            }
        }

        public void SetGub(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public async Task JoinGameOnServer(JoinRequest joinRequest, String baseUrl, HttpClient client)
        {
            try
            {
                await _hubConnection.SendAsync("Join", joinRequest);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to join game on server: " + ex.Message, ex);
            }
        }
    }

    public class SignalRNeogation
    {
        public String url { get; set; }
        public String accessToken { get; set; }

    }

}
