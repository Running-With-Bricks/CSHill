using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EventEmitter
{
    private Dictionary<string, List<dynamic>> Events = new();

    public void on(string Event, dynamic func)
    {
        if (!Events.ContainsKey(Event))
        {
            Events.Add(Event, new List<dynamic>());
        }
        Events[Event].Add(func);
    }
    public void off(string Event, dynamic func)
    {
        if (!Events.ContainsKey(Event))
        {
            Events.Add(Event, new List<dynamic>());
        }
        Events[Event].Remove(func);
    }
    public void emit(string Event, params dynamic[] args)
    {
        if (!Events.ContainsKey(Event))
        {
            Events.Add(Event, new List<dynamic>());
        }

        foreach (dynamic func in Events[Event])
        {
            func(args);
        }
    }
}

