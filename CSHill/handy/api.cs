using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;


public class api
{
    private static HttpClient client = new();
    private static uint id = 0;
    private class GameData
    {
        public string host_key;
        public int port;
        public string[] players;
        public GameData()
        {
            host_key = Game.Config.hostKey;
            port = Game.Config.port;
            players = Game.Players.Select(player => player.Token).ToArray();
        }

    }
    public static (string Name, uint userId, bool admin, uint membership) checkAuth(string token)
    {
        //static string AUTHENTICATION_API(string token, string hostKey) { return $"https://api.brick-hill.com/v1/auth/verifyToken?token={Uri.EscapeUriString(token)}&host_key={Uri.EscapeUriString(hostKey)}"; };
        Regex UID_REGEX = new("/^[\\w]{8}(-[\\w]{4}){3}-[\\w]{12}$/");

        if (Game.Config.local.ToLower() == "true")
        {
            id++;
            return ($"Player{id}", id, false, 1);
        }

        if (!UID_REGEX.IsMatch(token))
        {

        };
        id++;
        return ($"Player{id}", id, false, 1);
        //throw new Exception("Token invalid!");
    }
    public static async void postServer()
    {
        string gamdat = JsonConvert.SerializeObject(new GameData());
        HttpResponseMessage response = await client.PostAsync("https://api.brick-hill.com/v1/games/postServer", new StringContent(gamdat, Encoding.UTF8, "application/json"));
        //await response.Content.ReadAsStringAsync()  //for set id and banned players, implement later
        Console.WriteLine(gamdat);
    }
}
