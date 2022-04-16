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

    RecordLevel recordLevel;


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
        sprite.enabled = true;
        pushed = false;

        if (modeRecord && !alreadyOff)
        {
            recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOff, (byte)noteNumber, 0);
            alreadyOn = false;
            alreadyOff = true;
   
        }
    }
    void playNote(float volume)
    {
        source.Play();
        source.volume = volume;
        print(noteNumber);
        sprite.enabled = false;
        pushed = true;

        if (modeRecord && !alreadyOn) 
        {
            recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOn, (byte)noteNumber, (byte)(volume * 100));
            alreadyOn = true;
            alreadyOff = false;

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

    private void OnTriggerEnter2D (Collider2D other)
    {
      
        if (pushed)
        {
           
            if (other.tag == "Note")
            {
                Destroy(other);
            }
        }
    }





}
