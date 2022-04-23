using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Este es el script que controla el funcionamiento de nivel, desde crear las distintas notas, manejar la puntuación, gameplay ...etc

public class Level : MonoBehaviour
{

    public FileManager fileManager;
    string file_name;

    public ManageScenes scenes;

    int timePerColumn = 100;

    public MidiFile midiFile;
    public NoteIndicatorGroup indicatorGroup;


     List<MidiFile.MidiTrack> midiTracks;

    int numOctava = 0;

    public GameObject prefabNote;

    List<Note> notes;

    [Header("count down")]
    public Count_down count_Down;

    //Nota minima y maxima
    List<int> _notes = new List<int>();

    public float maxScore = 10000;

    public float actualScore = 0;

    public Text text;
    private string scoreText = "Score: ";

    bool finishedTimer = false;
    public int notesLeft;

    public GameObject playButton;
    public GameObject menuButton;
    public GameObject restartButton;
    public GameObject score;
    public GameObject frame;
    public GameObject finishFrame;

    public bool finishedLevel = false;

    Data saveData;


    public float getMaxScoreCount()
    {
        return maxScore;
    }

    protected void setStartTimer()
    {
        count_Down.start_count_down();
        count_Down.handler += this.countDownOver;
    }
    protected void countDownOver()
    {
        finishedTimer = true;

        score.SetActive(true);
        frame.SetActive(true);
        menuButton.SetActive(true);
        restartButton.SetActive(true);
        
        //Start note movement
        foreach (Note note in notes)
        {
            note.moving = true;

            note.nLevelNotes = notes.Count();
            note.maxScore = maxScore;
            note.setScoreMetters();
        }
    }

    private void Start()
    {
        try
        {
            file_name = fileManager.OpenFileExplorer();

            midiFile.parseFile(file_name);

            midiFile = GetComponent<MidiFile>();
            midiTracks = midiFile.getMidiFileTracks();

            int i = 0;
            foreach (MidiFile.MidiTrack track in midiTracks)
            {
                foreach (MidiFile.MidiNote note in track.vecNotes)
                {
                    if (note.nKey > 0 && note.nKey < 100)
                    {
                        _notes.Add(note.nKey);

                    }
                }

                i++;
            }

            indicatorGroup.setNoteRange(_notes.Min(), _notes.Max());

            indicatorGroup.iniciate();

        }
        catch(Exception e)
        {
            print(e);
            scenes.changeScene("MainMenu");
        }

    }

    public void setNotesToDestroy()
    {
        if (notes != null && notes.Count > 0)
        {
            foreach (Note note in notes)
            {
                note.setReadyToDestroy();
            }
            notes.Clear();
        }
    }

    public void startLevel()
    {

        actualScore = 0f;
        finishedTimer = false;

        playButton.SetActive(false);
        finishFrame.SetActive(false);
        setNotesToDestroy();
        
        movimientoVisualNotas();
     
        setStartTimer();
    }

    public void addScore(float score)
    {
        if(score < 0)
        {
            score = -maxScore / notes.Count() / 6;
        }  
        actualScore += score;
        
        
        if (actualScore < 0) actualScore = 0;
    }

    public bool checkIfFinished()
    {
        if (notesLeft <= 0)
        {
            return true;
        }
        return false;
    }

    void movimientoVisualNotas()
    {
        int aux = 0;
        int numTrack = 0;
        numOctava = indicatorGroup.getNumEscalas();
        notes = new List<Note>();

        notesLeft = 0;

       foreach(MidiFile.MidiTrack track in midiTracks)
        {
            if(track.vecNotes != null && track.vecNotes.Any())
            {
                //Rango de notas
                int nNoteRange = track.nMaxNote - track.nMinNote;

                foreach (MidiFile.MidiNote note in track.vecNotes)
                {
                    GameObject noteAux;
                    noteAux = Instantiate(prefabNote);

                    notesLeft++;
                    
                    noteAux.transform.position = new Vector3(indicatorGroup.getNoteIndicatorPos(note.nKey).x,(note.nStartTime-10)/timePerColumn+15,0);

                    if(numOctava >= 4)
                        noteAux.transform.localScale = new Vector3( 50f/ (numOctava), 0, 0);
                    else
                        noteAux.transform.localScale = new Vector3(25f, 0, 0);

                    noteAux.GetComponent<Note>().setNote(note.nKey,(int)note.nDuration,numTrack, 3.75f);
                    noteAux.GetComponent<Note>().setLevel(this);

                   notes.Add(noteAux.GetComponent<Note>());

                    aux++;

                }
            }
            numTrack++;
        }
    }

    protected void finishLevel()
    {
        finishFrame.SetActive(true);
        saveLevelData();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (finishedTimer)
        {
            text.text = scoreText + (int)actualScore;
            if (checkIfFinished() && !finishedLevel)
            {
                finishedLevel = true;
                finishLevel();
            }
        }
    }

    protected void saveLevelData()
    {
        Data lastData;
        lastData = SaveController.LoadData();

        bool newLevel = true;
        float newScore;

        char[] separator = {'/', '.'};
        string[] auxName = file_name.Split(separator);

        string levelName = auxName[auxName.Length - 2];
        print(levelName);
        if (lastData != null) 
        {
            saveData = lastData;

            int i = 0;

            foreach (Data.LevelData levelData in saveData.levelsData)
            {
                if(levelData.levelName == file_name)
                {
                    newLevel = false;
                    newScore = levelData.score;
                    if (newScore < actualScore)
                    {
                        newScore = actualScore;
                        saveData.addXp((int)(actualScore * 100 / maxScore));
                    }
             
                    saveData.levelsData[i] = new Data.LevelData(levelName, newScore, 1 + levelData.attempts);

                    break;
                }
                i++;
            }

            if (newLevel)
            {
                saveData.levelsData.Add(new Data.LevelData(levelName, actualScore, 1));
                saveData.addXp((int)(actualScore * 100 / maxScore));
            }

            saveData.previousLevel = levelName;
            
        }
        else
        {
            List<Data.LevelData> auxList = new List<Data.LevelData>();
            auxList.Add(new Data.LevelData(levelName, actualScore, 1));
            saveData = new Data(0, 0,1, 1,new bool[25], auxList, levelName);
            saveData.addXp((int)(actualScore * 100 / maxScore));
        }

        SaveController.SaverData(saveData);
        
        //Data testData =SaveController.LoadData();
    }
}
