﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notas para el modo de juego
public class Note : MonoBehaviour
{
    //Número de nota
    public int nNote = 0;

    //Número de tracj
    public int numTrack;

    //Si es una nota simple o es una nota a mantener
    bool simpleNote = false;

    //Duración en ticks. SI es mayor de 150 entonces es nota larga
    public int tickDuration = 0;
    public bool longNote;

    //El porcentaje de compleción de una nota. Es decir, si ha sido pulsada satisfactoriamente estara a 1, si no a 0. Si es una nota larga entonces podra ser pulsada a un cierto porcentaje
    
    public float caminoRecorrido = 0;

    //Lista para ser destruida
    bool readyToDestroy = false;

    public float _vel = 0.005f;
    public bool moving = false;
    //detener la nota
    private float stopMovingAt = -6;

    Vector2 pushPoint;
    Vector2 releasePoint;
    bool pushing = false;
    public float totalDistance;
    public float totalRecorrido = 0;

    Level _level;

    SpriteRenderer sprite;


    public float perfectScore;
    public float goodScore;
    public float okScore;
    public float badScore;

    public int nLevelNotes;
    public float maxScore;

    public void setLevel(Level level)
    {
        _level = level;
    }

    public void setScoreMetters()
    {
        perfectScore = maxScore / nLevelNotes;
        goodScore = perfectScore / 1.5f;
        okScore = perfectScore / 2;
        badScore = -okScore / 2;
    }

    public void addScore(float distance, float percNoteCompleted)
    {
        float actualScore = 0;

            if (distance < 0)
            {
                actualScore += badScore;
            }
            else if (distance > 2)
            {
                actualScore += badScore * percNoteCompleted;

            }
            else if (distance > 1.5)
            {
                actualScore += okScore * percNoteCompleted;
            }
            else if (distance > 1)
            {
                actualScore += goodScore * percNoteCompleted;
            }
            else if (distance < 1)
            {
                actualScore += perfectScore * percNoteCompleted;
            }
      
        _level.addScore(actualScore);
    }

    public void setPushPointHit(float x, float y, float distance)
    {
        if (!longNote)
        {
            addScore(distance, 1);
            gameObject.SetActive(false);
        }
        else 
        {
            pushing = true;
            pushPoint = new Vector2(x,y);
            totalDistance = distance;
        }
      
    }

    public void setReleasePointHit(float x, float y)
    {
        if (longNote&&pushing)
        {
            if(totalRecorrido > .75)
            {
                _level.addScore((1 - totalRecorrido)* perfectScore); 
            }
            pushing = false;
            
            gameObject.SetActive(false);
        }
    }

    //Inicializar la nota
    public bool setNote (int num, int nTicks, int track, float vel)
    {

        nNote = num;

        tickDuration = nTicks;
        numTrack = track;

        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, (float)(tickDuration),0);
        _vel = vel;

        sprite = GetComponent<SpriteRenderer>();

        if (tickDuration > 150)
        {
            sprite.color = Color.green;
            longNote = true;
        }
        else longNote = false;

        return true;

    }
    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (pushing && longNote )
            {
                if ( pushPoint.y < transform.position.y + transform.localScale.y / 2 / 100) {

                    caminoRecorrido = (_vel * Time.deltaTime) * 100 / tickDuration;
                    totalRecorrido += caminoRecorrido;
                    addScore(totalDistance, caminoRecorrido);
                }
            }

            transform.Translate(Vector3.down * _vel * Time.deltaTime);

            if(transform.position.y < stopMovingAt - tickDuration)
            {
                moving = false;
            }
        }

    }
}
