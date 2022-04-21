﻿using System;
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
        //Start note movement
        foreach(Note note in notes)
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
            movimientoVisualNotas();
            

            setStartTimer();
        }
        catch(Exception e)
        {
            print(e);
            scenes.changeScene("MainMenu");
        }

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

    void movimientoVisualNotas()
    {
        int aux = 0;
        int numTrack = 0;
        numOctava = indicatorGroup.getNumEscalas();
        notes = new List<Note>();
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

    // Update is called once per frame
    void Update()
    {
        text.text = scoreText + (int) actualScore;
    }
}
