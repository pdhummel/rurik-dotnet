using System;
using System.Collections.Generic;
namespace rurik;

public class GameLog
{
    public RurikGame RurikGame { get; set; }
    public List<Entry> Entries { get; set; }

    public GameLog(RurikGame game)
    {
        RurikGame = game;
        Entries = new List<Entry>();
    }

    public void Log(string text)
    {
        AddEntry(text, RurikGame);
    }

    public void Info(string text)
    {
        AddEntry(text, RurikGame);
    }

    public void AddEntry(string text, RurikGame game = null)
    {
        var entry = new Entry(text, game);
        Entries.Add(entry);
    }
    
    public void AddLogEntry(string text)
    {
        AddEntry(text, RurikGame);
    }

    public List<Entry> GetEntriesSinceLastTurn(Player player)
    {
        // TODO: fix this
        //var ts = player.LastActionTimeStamp;
        //return GetEntriesAfterTimestamp(ts);
        return new List<Entry>();
    }

    public List<Entry> GetEntriesAfterTimestamp(long ts)
    {
        var entries = new List<Entry>();
        var firstMatch = false;
        
        for (int i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];
            if (entry.TimeStamp == ts && !firstMatch)
            {
                firstMatch = true;
            }
            else if (entry.TimeStamp >= ts)
            {
                entries.Add(entry);
            }
        }
        return entries;
    }

    public List<Entry> GetEntriesAfterPosition(int count = -1)
    {
        var entries = new List<Entry>();
        for (int i = 0; i < Entries.Count; i++)
        {
            if (i > count)
            {
                var entry = Entries[i];
                entries.Add(entry);
            }
        }
        return entries;
    }
}

public class Entry
{
    public long TimeStamp { get; set; }
    public string Text { get; set; }
    public string GameId { get; set; }
    public int Round { get; set; }
    public string Player { get; set; }

    public Entry(string text, RurikGame game)
    {
        TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Text = text;
        
        if (game != null)
        {
            GameId = game.Id;
            Round = game.CurrentRound;
            var player = game.Players.GetCurrentPlayer();
            Player = player.Color;
            // TODO: player.LastActionTimeStamp = TimeStamp;
            Globals.Log("[" + game.Id + "] " + text);
        }
        else
        {
            Globals.Log(text);
        }
    }
}
