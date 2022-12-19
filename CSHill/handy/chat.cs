using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scripts
{
    public class chat
    {
        public static string GenerateTitle(Player p, string message)
        {
            string title = "<color:ffde0a>" + p.Name + "\\c1:\\c0 " + message;

            if (p.Team != null) title = p.Team.Color.gmlhex() + p.Name + "\\c1:\\c0 " + message;
            if (p.admin) title = "<color:ffde0a>" + p.Name + "\\c1:\\c0 " + message;
            if (p.ChatColor != null) title = p.ChatColor.gmlhex() + p.Name + "\\c1:\\c0 " + message;

            return Color.formatHex(title);
        }
    }
}