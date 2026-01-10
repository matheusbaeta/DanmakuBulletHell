using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreEntry
{
    public string levelName;
    public string timestamp;
    public int totalScore;
    public int damageDealt;
    public float timeTaken;
    public int hitsTaken;
    public int enemiesKilled;
}

[Serializable]
public class ScoreHistory
{
    public List<ScoreEntry> entries = new List<ScoreEntry>();
}
