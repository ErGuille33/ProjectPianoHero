using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase para guardar los datos del jugador
[System.Serializable]
public class Data
{
    //Datos del nivel en específico
    [System.Serializable]
    public struct LevelData
    {
        public string levelName;
        public float score;
        public float lastScore;
        public int attempts;

        public int numBad;
        public int numOk;
        public int numPerf;
        public int numGood;
        public int numNan;


        public LevelData(string levelName, float score, int attempts, float lastScore, int numBad, int numOk, int numPerf, int numGood, int numNan)
        {
            this.levelName = levelName;
            this.score = score;
            this.attempts = attempts;
            this.lastScore = lastScore;

            this.numBad = numBad;
            this.numOk = numOk;
            this.numPerf = numPerf;
            this.numGood = numGood;
            this.numNan = numNan;
        }

        //Aqui vendrían mas
    }

    //Datos del jugador
    public string previousLevel = "";

    public float volumePiano;
    public float volumeFx;

    public bool alreadyPlayed;
    public bool alreadyRecorded;

    public int expPoints;
    public int previousExpPoints;
    public int levelPlayer;
    public int previousLevelPlayer;
    public bool[] awards;
    public List<LevelData> levelsData;

    //public float pianoVol;
    //public float fxVol;

    public Data(int expPoints, int previousExpPoints, int levelPlayer, int previousLevelPlayer, bool[] awards, List<LevelData> levelsData, string previousLevel, float volumePiano, float volumeFx, bool alreadyPlayed, bool alreadyRecorded)
    {
        this.levelPlayer = levelPlayer;
        this.awards = awards;
        this.levelsData = levelsData;
        this.previousLevelPlayer = previousLevelPlayer;
        this.expPoints = expPoints;
        this.previousExpPoints = previousExpPoints;
        this.previousLevel = previousLevel;
        this.volumePiano = volumePiano;
        this.volumeFx = volumeFx;
        this.alreadyPlayed = alreadyPlayed;
        this.alreadyRecorded = alreadyRecorded;
    }

    public Data(Data _data)
    {
        this.levelPlayer = _data.levelPlayer;
        this.awards = _data.awards;
        this.levelsData = _data.levelsData;
        this.previousLevelPlayer = _data.previousLevelPlayer;
        this.expPoints = _data.expPoints;
        this.previousExpPoints = _data.previousExpPoints;
        this.previousLevel = _data.previousLevel;
        this.volumePiano = _data.volumePiano;
        this.volumeFx = _data.volumeFx;
        this.alreadyPlayed = _data.alreadyPlayed;
        this.alreadyRecorded = _data.alreadyRecorded;

    }

    //Añadir XP
    public void addXp(int xp)
    {
        if (this.previousLevelPlayer != 99)
        {
            int aux = xp + this.expPoints;
            this.previousLevelPlayer = this.levelPlayer;
            this.previousExpPoints = this.expPoints;

            if (aux > levelPlayer * 75)
            {
                this.expPoints = aux - (levelPlayer * 75);
                this.levelPlayer += 1;
            }
            else this.expPoints += xp;
        }

    }

    //Máximo de experiencia que se puede alcanzar en cierto nivel
    public int getMaxXp(int lvl)
    {
        return lvl * 75;
    }

}
