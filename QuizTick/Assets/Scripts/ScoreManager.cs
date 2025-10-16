using System.Linq;
using SQLite4Unity3d;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class ScoreManager
{
    private static string dbPath = Path.Combine(Application.streamingAssetsPath, "users.db");

    public static void SaveScore(string username, string language, string level, int value)
{
    using (var db = new SQLiteConnection(dbPath))
    {
        db.CreateTable<Score>();

        var existing = db.Table<Score>()
            .FirstOrDefault(s =>
                s.Username.Trim().ToLower() == username.Trim().ToLower() &&
                s.Language.Trim().ToLower() == language.Trim().ToLower() &&
                s.Level.Trim().ToLower() == level.Trim().ToLower()
            );

        if (existing == null)
        {
            db.Insert(new Score
            {
                Username = username,
                Language = language,
                Level = level,
                Value = value
            });
        }
        else
        {
            if (value > existing.Value)
            {
                existing.Value = value;
                db.Update(existing);
            }
        }
    }
}

public static int GetHighScore(string username, string language, string level)
{
    using (var db = new SQLiteConnection(dbPath))
    {
        var entry = db.Table<Score>()
            .Where(s =>
                s.Username.Trim().ToLower() == username.Trim().ToLower() &&
                s.Language.Trim().ToLower() == language.Trim().ToLower() &&
                s.Level.Trim().ToLower() == level.Trim().ToLower()
            )
            .FirstOrDefault();

        return entry != null ? entry.Value : 0;
    }
}
    public static List<Score> GetTopScores(string language, string level, int limit = 10)
    {
        using (var db = new SQLiteConnection(dbPath))
        {
            return db.Table<Score>()
                .Where(s => s.Language == language && s.Level == level)
                .OrderByDescending(s => s.Value)
                .Take(limit)
                .ToList();
        }
    }
}
