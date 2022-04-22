using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    [System.Serializable]
    public struct LevelData
    {
        public string levelName;
        public float score;
        public int attempts;

        public LevelData(string levelName, float score, int attempts)
        {
            this.levelName = levelName;
            this.score = score;
            this.attempts = attempts;
        }

        //Aqui vendrían mas
    }

    public string previousLevel = "";
    public int expPoints;
    public int levelPlayer;
    public bool[] awards;
    public List<LevelData> levelsData;

    public Data(int expPoints, int levelPlayer, bool[] awards, List<LevelData> levelsData, string previousLevel)
    {
        this.levelPlayer = levelPlayer;
        this.awards = awards;
        this.levelsData = levelsData;

        this.expPoints = expPoints;
        this.previousLevel = previousLevel;
    }

    public Data(Data _data)
    {
        this.levelPlayer = _data.levelPlayer;
        this.awards = _data.awards;
        this.levelsData = _data.levelsData;

        this.expPoints = _data.expPoints;
        this.previousLevel = _data.previousLevel;

    }

    public void addXp(int xp)
    {

        int aux = xp + this.expPoints;

        if (aux > levelPlayer * 100)
        {
            this.expPoints = aux - (levelPlayer * 100);
            this.levelPlayer += 1;
        }
        else this.expPoints += xp;

    }

}
