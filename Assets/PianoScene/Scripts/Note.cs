using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Todavía hay que ver si utilizaremos object pool, yo creo que no
public class Note : MonoBehaviour
{
    //Número de nota
    public int nNote = 0;

    //Número de tracj
    public int numTrack;

    //Si es una nota simple o es una nota a mantener
    bool simpleNote = false;

    //Duración en ticks
    public int tickDuration = 0;

    //El porcentaje de compleción de una nota. Es decir, si ha sido pulsada satisfactoriamente estara a 1, si no a 0. Si es una nota larga entonces podra ser pulsada a un cierto porcentaje
    float completedPercent = 0;

    //Lista para ser destruida
    bool readyToDestroy = false;

    float _vel = 0.005f;

    public bool moving = false;

    private float stopMovingAt = -6;

    //Inicializar la nota
    public bool setNote (int num, int nTicks, int track, float vel)
    {

        nNote = num;

        tickDuration = nTicks;
        numTrack = track;

        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, (float)(tickDuration),0);
        _vel = vel;

        return true;

    }

    public void setPercent(float percent)
    {
        completedPercent = percent;
    }

    public bool getReadyToDestroy()
    {
        return readyToDestroy;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            transform.Translate(Vector3.down * _vel * Time.deltaTime);

            if(transform.position.y < stopMovingAt)
            {
                moving = false;
            }
        }
    }
}
