﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class api
{
    private static int id = 0;
    public static (string Name,int userId,bool admin,int membership) checkAuth(string token)
    {
        static string AUTHENTICATION_API(string token, string hostKey) { return $"https://api.brick-hill.com/v1/auth/verifyToken?token={Uri.EscapeUriString(token)}&host_key={Uri.EscapeUriString(hostKey)}"; };
        Regex UID_REGEX = new("/^[\\w]{8}(-[\\w]{4}){3}-[\\w]{12}$/");

        if(Game.Config.local.ToLower() == "true" )
        {
            id++;
            return ($"Player{id}", id, false, 1);
        }

        if (!UID_REGEX.IsMatch(token))
        {
            
        };
        throw new Exception("Token invalid!");
    }
}