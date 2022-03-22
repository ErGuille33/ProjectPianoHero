using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    /*
    //Para seleccionar el evento
    public FMODUnity.EventReference selectEvent;
    //Evento FMOD
    private FMOD.Studio.EventInstance pianoEvent;

    FMOD.Studio.Bus Master;
    float masterVolume = 1f;*/



    public void noteAssign(int noteNumber)
    {

    }

    void Start()
    {
        /*
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Audio 1");
        Master.setVolume(masterVolume);*/
       
       
    }

    public void playNote(int noteNumber,float intensity) 
    {
        /*
        pianoEvent = FMODUnity.RuntimeManager.CreateInstance(selectEvent);

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Note",noteNumber);
        pianoEvent.start();
        pianoEvent.release();*/
        //print(noteNumber);
    }
}
