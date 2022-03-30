using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Todavía hay que ver si utilizaremos object pool, yo creo que no
public class Note : MonoBehaviour
{
    //Número de nota
    int nNote = 0;

    //Si es una nota simple o es una nota a mantener
    bool simpleNote = false;

    //Duración en ticks
    int tickDuration = 0;

    //El porcentaje de compleción de una nota. Es decir, si ha sido pulsada satisfactoriamente estara a 1, si no a 0. Si es una nota larga entonces podra ser pulsada a un cierto porcentaje
    float completedPercent = 0;

    //Lista para ser destruida
    bool readyToDestroy = false;

    //Inicializar la nota
    public bool setNote (int num, bool isSimple, int nTicks)
    {

        nNote = num;
        simpleNote = isSimple;
        tickDuration = nTicks;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
