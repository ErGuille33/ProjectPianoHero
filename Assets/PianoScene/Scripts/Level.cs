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
    string file_name = "";

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
    public GameObject avisoFrame;
    public GameObject closeAvisoButton;
    public GameObject velFrame;
    public Slider velSlider;
    public Text velText;

    public float vel = 1;

    public bool finishedLevel = false;

    Data saveData;

    public int numBad = 0;
    public int numOk = 0;
    public int numPerf = 0;
    public int numGood = 0;
    public int numNan = 0;

    bool scalesInstanciated = false;
    bool canAddScores = false;

    public AudioSource blupAudio;
    public AudioSource tickAudio;

    public Image scoreBar;

    public void resetSaveUnits()
    {
        numBad = 0;
        numOk = 0;
        numPerf = 0;
        numGood = 0;
        numNan = 0;
    }

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

        canAddScores = true;

        foreach (Note note in notes)
        {
            note.moving = true;

            note.nLevelNotes = notes.Count();
            note.maxScore = maxScore;
            note.setScoreMetters();
        }
    }

    private IEnumerator openFile()
    {
        menuButton.SetActive(false);
        StartCoroutine(fileManager.OpenFileExplorer());
        while (file_name == "")
        {
            file_name = fileManager.getPath();
            yield return null;
        }
        if (file_name == "cancel")
        {
            scenes.changeScene("MainMenu");
        }
        menuButton.SetActive(true);
        chooseVel();

    }

    private void iniciate()
    {
        try
        {
            midiFile.parseFile(file_name);

            midiFile = GetComponent<MidiFile>();
            midiTracks = midiFile.getMidiFileTracks();

            if (midiTracks.Count == 0)
            {
                scenes.changeScene("MainMenu");
            }

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

            if (!scalesInstanciated)
            {
                indicatorGroup.setNoteRange(_notes.Min(), _notes.Max());
                indicatorGroup.iniciate();
                scalesInstanciated = true;
            }
        }
        catch (Exception e)
        {
            scenes.changeScene("MainMenu");
        }
    }

    private void Start()
    {
        try
        {
            saveData = SaveController.LoadData();
            if (saveData != null)
            {
                if (!saveData.alreadyPlayed)
                {
                    openAvisoCanvas();
                }
                else { StartCoroutine(openFile()); }
            }
            else { openAvisoCanvas(); }



        }
        catch (Exception e)
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
        if (!scalesInstanciated)
        {
            indicatorGroup.setNoteRange(_notes.Min(), _notes.Max());
            indicatorGroup.iniciate();
            scalesInstanciated = true;
        }

        canAddScores = false;

        actualScore = 0f;
        finishedTimer = false;
        finishedLevel = false;
        scoreBar.fillAmount = 0;

        resetSaveUnits();

        playButton.SetActive(false);
        finishFrame.SetActive(false);
        setNotesToDestroy();

        movimientoVisualNotas();



        setStartTimer();
    }

    public void addScore(float score, int typeInput)
    {
        if (canAddScores)
        {
            // 0 bad, 1 ok, 2 good, 3 perfect, -1 Nan

            switch (typeInput)
            {
                case 0:
                    numBad++;
                    break;
                case 1:
                    numOk++;
                    break;
                case 2:
                    numGood++;
                    break;
                case 3:
                    numPerf++;
                    break;
                case -1:
                    numNan++;
                    break;
                default:
                    break;

            }

            if (score < 0)
            {
                score = -maxScore / notes.Count() / 6;
            }
            actualScore += score;


            if (actualScore < 0) actualScore = 0;

            //text.text = scoreText + (int)actualScore;

            if(actualScore < 25000)
            {
                scoreBar.color = new Color(0.66f, 0, 0f);
            }

            else if (actualScore > 66666)
            {
                scoreBar.color = new Color(.25f,.29f,1);
            }

            else if (actualScore < 75000)
            {
                scoreBar.color = new Color(1, .6f, 0);
            }

            scoreBar.fillAmount = actualScore / maxScore;

        }
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

        float bpm = 0;
        float auxStartTime = 10000;

        foreach (MidiFile.MidiTrack track in midiTracks)
        {
            if (track.bpm > 0)
            {
                bpm = track.bpm;
            }
            if (track.vecNotes != null && track.vecNotes.Any())
            {
                //Rango de notas
                int nNoteRange = track.nMaxNote - track.nMinNote;

                foreach (MidiFile.MidiNote note in track.vecNotes)
                {


                    if (auxStartTime > note.nStartTime)
                    {
                        auxStartTime = note.nStartTime;
                    }

                }

                foreach (MidiFile.MidiNote note in track.vecNotes)
                {
                    if (note.nKey > 8 && note.nKey < 110) {

                        GameObject noteAux;
                        noteAux = Instantiate(prefabNote);

                        noteAux.transform.parent = this.gameObject.transform;

                        notesLeft++;


                        noteAux.transform.position = new Vector3(indicatorGroup.getNoteIndicatorPos(note.nKey).x, ((note.nStartTime) / timePerColumn) + 20, 0);

                        if (numOctava >= 4)
                            noteAux.transform.localScale = new Vector3(50f / (numOctava), 0, 0);
                        else
                            noteAux.transform.localScale = new Vector3(25f, 0, 0);

                        if (bpm <= 0)
                        {
                            noteAux.GetComponent<Note>().setNote(note.nKey, (int)note.nDuration, numTrack, (600f / bpm) * vel);
                        }
                        else
                        {
                            //float auxVel = (((20f - indicatorGroup.getYDetecor() + (auxStartTime / (float)timePerColumn) ) / (600f / bpm))) * vel;
                            float auxVel = (600f / bpm) * vel * .9f;
                            noteAux.GetComponent<Note>().setNote(note.nKey, (int)note.nDuration, numTrack, auxVel);
                        }
                        noteAux.GetComponent<Note>().setLevel(this);

                        notes.Add(noteAux.GetComponent<Note>());

                        aux++;
                    }

                }
            }
            numTrack++;
        }
    }

    protected void finishLevel()
    {
        if (actualScore > 99900) actualScore = 100000;
        else if (actualScore % 10 != 0) actualScore = roundToTen(actualScore);

        canAddScores = false;
        finishFrame.SetActive(true);
    }

    void Update()
    {

        if (finishedTimer)
        {
            
            if (checkIfFinished() && !finishedLevel)
            {
                finishedLevel = true;
                finishLevel();
            }
        }
    }

    float roundToTen(float n)
    {
        return n+10 - ((n+10)%10);
    }

    //Se le llama al pulsar el botón de continuar
    public void saveLevelData()
    {
        try
        {
            Data lastData;
            lastData = SaveController.LoadData();

            bool newLevel = true;
            float newScore;

            char[] separator = { '/', '.', '\\' };
            string[] auxName = file_name.Split(separator);

            string levelName = auxName[auxName.Length - 2];

            float auxVel = 0;

            if (lastData != null)
            {
                saveData = lastData;

                int i = 0;

                foreach (Data.LevelData levelData in saveData.levelsData)
                {
                    if (levelData.levelName == levelName)
                    {
                        newLevel = false;
                        newScore = levelData.score;
                        auxVel = levelData.vel;

                        if (newScore <= actualScore)
                        {
                            newScore = actualScore;
                            saveData.addXp((int)(actualScore * 100 / maxScore));

                            if (levelData.vel < vel)
                            {
                                auxVel = vel;

                            }
                        }
                        else
                        {
                            saveData.addXp((int)((actualScore * 100 / maxScore) / 2));
                        }

                        saveData.levelsData[i] = new Data.LevelData(levelName, newScore, 1 + levelData.attempts, actualScore, numBad, numOk, numPerf, numGood, numNan, auxVel);

                        break;
                    }
                    i++;
                }

                if (newLevel)
                {
                    saveData.levelsData.Add(new Data.LevelData(levelName, actualScore, 1, actualScore, numBad, numOk, numPerf, numGood, numNan, vel));
                    saveData.addXp((int)(actualScore * 100 / maxScore));
                }

                saveData.previousLevel = levelName;
                saveData.alreadyPlayed = true;

            }
            else
            {

                List<Data.LevelData> auxList = new List<Data.LevelData>();

                auxList.Add(new Data.LevelData(levelName, actualScore, 1, actualScore, numBad, numOk, numPerf, numGood, numNan, vel));

                saveData = new Data(0, 0, 1, 1, new bool[25], auxList, levelName, 1, 1, true, false);

                saveData.addXp((int)(actualScore * 100 / maxScore));
            }

            SaveController.SaverData(saveData);

            scenes.changeScene("ScoreScene");
        }
        catch(Exception e)
        {
            scenes.changeScene("ScoreScene");
        }

    }

    public void closeAvisoCanvas()
    {
        tickAudio.Play();
        avisoFrame.SetActive(false);
        closeAvisoButton.SetActive(false);
        menuButton.SetActive(true);
        playButton.SetActive(true);
        StartCoroutine(openFile());
    }

    public void openAvisoCanvas()
    {
        avisoFrame.SetActive(true);
        closeAvisoButton.SetActive(true);
        menuButton.SetActive(false);
        playButton.SetActive(false);
    }

    //Antes de elegir la velocidad
    public void chooseVel()
    {
        velFrame.SetActive(true);
        playButton.SetActive(false);
        vel = 1;
    }
    //Ui vel
    public void changeVel()
    {
        vel = velSlider.value * 0.25f;
        velText.text = "Velocidad de reproducción: \n\n" + "x" + vel;
    }

    //Settear la octava elegida
    public void setVel()
    {
        tickAudio.Play();
        velFrame.SetActive(false);
        playButton.SetActive(true);

      

        iniciate();


    }

}
