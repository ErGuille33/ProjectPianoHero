﻿using UnityEngine;
using MidiJack;

//Básicamente, la tecla de piano
public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;
    int nNotes = 1;

    public AudioSource source;

    private void Start()
    {
        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();
    }
    void Update()
    {
        transform.localScale =( Vector3.one * (0.1f + MidiMaster.GetKey(noteNumber))) / ((float)nNotes/25);

        var color = MidiMaster.GetKeyDown(noteNumber) ? Color.red : Color.white;
        GetComponent<Renderer>().material.color = color;

        if (MidiMaster.GetKeyDown(noteNumber)) playNote(MidiMaster.GetKey(noteNumber));

    
    }

    void playNote(float volume)
    {
        source.Play();
        source.volume = volume;
        print(noteNumber);

        
    }
    
    public void setNNotes(int num)
    {
        nNotes = num;
    }

   
               
        
    
}
