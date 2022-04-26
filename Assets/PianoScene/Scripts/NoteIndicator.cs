﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

//Básicamente, una tecla del piano
public class NoteIndicator : MonoBehaviour
{
    //Número de nota
    public int noteNumber;

    //Para tocar la nota
    public AudioSource source;

    //Sprite de la nota
    private SpriteRenderer sprite;
    //Si se encuentra pulsada
    private bool pushed = false;
    //Si la nota está en modo grabación
    [SerializeField]
    private bool modeRecord;
    //Si la nota se encuentra en modo nivel
    [SerializeField]
    private bool modeGame;
    public Level level;
    //Referencias a nivel normal o nivel de grabación (solo es necesaria la del nivel en que se encuentre)
    RecordLevel recordLevel;

    //Detector de notas
    Detector detector;
    //Variables para el modo grabación
    bool alreadyOn = false;
    bool alreadyOff = true;
    //Distanci

    public Animator animator;

   //Se llama desde note indicatorGroup
    public void setRecordLevel(RecordLevel rl)
    {
        recordLevel = rl;
    }
    //Tomar distintas variables
    private void Start()
    {
        if(GetComponentInChildren<Detector>() != null)
        {
            detector = GetComponentInChildren<Detector>();
        }

        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();

        
        
        sprite = GetComponent<SpriteRenderer>();
    }
    //Controla la pulsación de la tecla
    void Update()
    {
        //Tecla ha sido liberada
        if (MidiMaster.GetKey(noteNumber) == 0)
        {
            releaseNote();
            animator.gameObject.SetActive(false); 
        }
        //Tecla ja sido pulsada
        if (MidiMaster.GetKey(noteNumber) !=0)
        {
            playNote(MidiMaster.GetKey(noteNumber));

        }
        
    
    }
    //Gestionar cuando la tecla es liberada
    void releaseNote()
    {
        if (!alreadyOff)
        {
          
            sprite.enabled = true;
            pushed = false;
            alreadyOn = false;
            alreadyOff = true;

            if(modeRecord)
                recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOff, (byte)(noteNumber), 0);
            if (modeGame)
            {
                detector.detectNoteReleaseDistance();
            }

        }
    }
    //Gestión de nota presionada
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
                recordLevel.addEventToPool(MidiFile.MidiEvent.Type.NoteOn, (byte)(noteNumber), (byte)(volume * 100));

            if (modeGame) 
            {
                if(!detector.detectNotePushDistance())
                    level.addScore(-1, -1);
                else
                {
                    animator.gameObject.SetActive(true);
                    animator.Play("Star", 0);
                }
                
            }
        }
    }

    //Modos de juego
    public void setModeRecord(bool record) 
    {
        modeRecord = record;
    }
    public void setModeGame(bool game)
    {
        modeGame = game;
    }







}
