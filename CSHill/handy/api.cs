﻿using System;
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
using System.Threading;

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
            host_key = Server.Config.hostKey;
            port = Server.Config.port;
            players = Game.Players.Select(player => player.Token).ToArray();
        }
    }
    private class PlayerData
    {
        public class User
        {
            [JsonProperty("id")]
            public uint id { get; set; }

            [JsonProperty("username")]
            public string username { get; set; }

            [JsonProperty("is_admin")]
            public bool is_admin { get; set; }

            [JsonProperty("membership")]
            public uint? membership { get; set; }
        }

        [JsonProperty("validator")]
        public string validator { get; set; }


        [JsonProperty("user")]
        public User user { get; set; }
    }

    private class PostData
    {
        [JsonProperty("set_id")]
        public uint set_id { get; set; }


        [JsonProperty("banned_users")]
        public uint[] banned_users { get; set; }

        public class Error
        {
            [JsonProperty("prettyMessage")]
            public string prettyMessage { get; set; }
        }

        [JsonProperty("error")]
        public Error error { get; set; }
    }

    public static (string Name, uint userId, bool admin, uint? membership, string token) checkAuth(string Token)
    {
        static string AUTHENTICATION_API(string token, string hostKey) { return $"https://api.brick-hill.com/v1/auth/verifyToken?token={Uri.EscapeUriString(token)}&host_key={Uri.EscapeUriString(hostKey)}"; };
        Regex UID_REGEX = new(@"[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}");

        if (Server.Config.local)
        {
            id++;
            return ($"Player{id}", id, false, 1, Token);
        }

        if (!UID_REGEX.IsMatch(Token))
        {
            throw new Exception("Token invalid!");
        };

        HttpResponseMessage response = client.GetAsync(AUTHENTICATION_API(Token, Server.Config.hostKey)).Result;

        string boban = response.Content.ReadAsStringAsync().Result;
        //Console.WriteLine(boban);
        var plyrdata = JsonConvert.DeserializeObject<PlayerData>(boban);
        uint? v = plyrdata.user.membership.HasValue ? plyrdata.user.membership : 0;
        plyrdata.user.membership = v;
        return (plyrdata.user.username, plyrdata.user.id, plyrdata.user.is_admin, plyrdata.user.membership, plyrdata.validator);

    }
    public static async void postServer()
    {
        try
        {
            string gamdat = JsonConvert.SerializeObject(new GameData());
            //Console.WriteLine(gamdat);
            HttpResponseMessage postresponse = await client.PostAsync("https://api.brick-hill.com/v1/games/postServer", new StringContent(gamdat, Encoding.UTF8, "application/json"));
            string stringdata = await postresponse.Content.ReadAsStringAsync();
            PostData postdata = JsonConvert.DeserializeObject<PostData>(stringdata);  //for set id and banned players, implement later
                                                                                      //Console.WriteLine(await response.Content.ReadAsStringAsync());
            if(postdata.error != null)
            {
                if(postdata.error.prettyMessage == "You can only postServer once every minute")
                {
                    Console.WriteLine("Failed to post to the games page, retrying in 1 minute");
                    return;
                }
                Console.WriteLine(postdata.error.prettyMessage);
                System.Environment.Exit(1);
                return;
            }
            if (Game.SetData.data == null)
            {
                try
                {
                    HttpResponseMessage setresponse = await client.GetAsync($"https://api.brick-hill.com/v1/sets/{postdata.set_id}");
                    if (postdata.set_id == 0)
                    {
                        Console.WriteLine("Failed to post to the games page, retrying in 1 minute");
                        return;
                    };
                    string stringe = await setresponse.Content.ReadAsStringAsync();
                    Game._setData SetData = JsonConvert.DeserializeObject<Game._setData>(stringe);
                    Console.WriteLine($"Successfully posted! https://brick-hill.com/play/{SetData.data.id} [{SetData.data.Name}]");
                    Game.SetData = SetData;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
