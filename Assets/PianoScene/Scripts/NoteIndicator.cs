using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

//Básicamente, la tecla de piano
public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;


    public AudioSource source;
    private SpriteRenderer sprite;
    private bool pushed = false;

    [SerializeField]
    private bool modeRecord;
    [SerializeField]
    private bool modeGame;


    RecordLevel recordLevel;

    Detector detector;

    public Level level;


    //Variables para el modo grabación
    List<MidiFile.MidiEvent> recordedMidiEvents;
    bool alreadyOn = false;
    bool alreadyOff = false;

    public void setRecordLevel(RecordLevel rl)
    {
        recordLevel = rl;
    }

    private void Start()
    {
        if(GetComponentInChildren<Detector>() != null)
        {
            detector = GetComponentInChildren<Detector>();
        }

        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();

        if (modeRecord)
        {
            recordedMidiEvents = new List<MidiFile.MidiEvent>();
        }
        
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        
        if (MidiMaster.GetKeyDown(noteNumber))
        {
            playNote(MidiMaster.GetKey(noteNumber));
        }
        else if (MidiMaster.GetKeyUp(noteNumber))
        {
            releaseNote();
        }
    
    }
    void releaseNote()
    {
    

        if (!alreadyOff)
        {
          
            sprite.enabled = true;
            pushed = false;
            alreadyOn = false;
            alreadyOff = true;

            if(modeRecord)
                recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOff, (byte)noteNumber, 0);

        }
    }
    void playNote(float volume)
    {
    

        if ( !alreadyOn) 
        {
            source.Play();
            source.volume = volume;
            sprite.enabled = false;
            pushed = true;
            
            alreadyOn = true;
            alreadyOff = false;



            if(modeRecord)
                recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOn, (byte)noteNumber, (byte)(volume * 100));

            if (modeGame) 
            {
                level.addScore(detector.detectNoteDistance());
            }
                


        }

    }

    public List<MidiFile.MidiEvent> GetMidiEvents()
    {
        return recordedMidiEvents;
    }
    
    public void setModeRecord(bool record) 
    {
        modeRecord = record;
    }
    public void setModeGame(bool game)
    {
        modeGame = game;
    }







}
