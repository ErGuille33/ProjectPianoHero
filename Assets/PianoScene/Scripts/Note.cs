using System.Collections;
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
    float completedPercent = 0;
    float caminoRecorrido = 0;

    //Lista para ser destruida
    bool readyToDestroy = false;

    public float _vel = 0.005f;
    public bool moving = false;
    //detener la nota
    private float stopMovingAt = -6;

    Vector2 pushPoint;
    Vector2 releasePoint;
    bool pushing = false;

    Level _level;

    SpriteRenderer sprite;

    public void setLevel(Level level)
    {
        _level = level;
    }


    public void setPushPointHit(float x, float y, float distance)
    {
        if (!longNote)
        {
            _level.addScore(distance, 1);
            gameObject.SetActive(false);
        }
        else 
        {
            pushing = true;
            pushPoint = new Vector2(x,y);
        }
      
    }

    public void setReleasePointHit(float x, float y)
    {
        if (longNote)
        {
            pushing = false;
            completedPercent = tickDuration / caminoRecorrido;
            if (completedPercent > 1) completedPercent = 1;

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
            if (pushing)
            {
                caminoRecorrido += _vel * Time.deltaTime;
            }

            transform.Translate(Vector3.down * _vel * Time.deltaTime);

            if(transform.position.y < stopMovingAt - tickDuration)
            {
                moving = false;
            }
        }

    }
}
