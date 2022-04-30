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
    
    public float caminoRecorrido = 0;

    //Lista para ser destruida
    bool readyToDestroy = false;

    public float _vel = 0.005f;
    public bool moving = false;
    //detener la nota
    private float stopMovingAt = -6;
    //Punto de presión
    Vector2 pushPoint;
    //Variables si la nota es larga
    bool pushing = false;
    public float totalDistance;
    public float totalRecorrido = 0;
    //Referencia al nivel
    Level _level;
    //Sprite principal
    SpriteRenderer sprite;
    //Tipos de puntuación
    public float perfectScore;
    public float goodScore;
    public float okScore;
    public float badScore;
    //El número de notas del nivel
    public int nLevelNotes;
    public float maxScore;
    //Partícuas al destruir 
    public ParticleSystem particle;
    //Destruir la nota desde Level
    public void setReadyToDestroy()
    {

        Destroy(this.gameObject);
    }
    //Añadir el level
    public void setLevel(Level level)
    {
        _level = level;
    }
    //Desde level se asignan los valores a los distintos tipos de scores
    public void setScoreMetters()
    {
        perfectScore = maxScore / nLevelNotes;
        goodScore = perfectScore / 1.5f;
        okScore = perfectScore / 2;
        badScore = -okScore / 2;
    }
    //Añadir puntuación al nivel dependiendo de la distancia al detector
    public void addScore(float distance, float percNoteCompleted)
    {
        float actualScore = 0;
        int typeInput = -2;

        // 0 bad, 1 ok, 2 good, 3 perfect, -1 Nan

            if (distance < 0)
            {
                actualScore += badScore;
                typeInput = 0;
            }
            else if (distance > 2)
            {
                actualScore += badScore * percNoteCompleted;
                typeInput = 0;

            }
            else if (distance > 1.5)
            {
                actualScore += okScore * percNoteCompleted;
                typeInput = 1;
            }
            else if (distance > 1)
            {
                actualScore += goodScore * percNoteCompleted;
                typeInput = 2;
            }
            else if (distance < 1)
            {
                actualScore += perfectScore * percNoteCompleted;
                typeInput = 3;
            }
      
        _level.addScore(actualScore, typeInput);

    }
    //Si es una nota corta se añade la puntuación al ser pulsada, si es larga se define el punto de inicio de pulsación
    public void setPushPointHit(float x, float y, float distance)
    {
        if (!longNote)
        {
            addScore(distance, 1);

            _level.notesLeft--;

            particle.transform.position = transform.position;
            Instantiate(particle);
            
            gameObject.SetActive(false);
        }
        else 
        {
            pushing = true;
            pushPoint = new Vector2(x,y);
            totalDistance = distance;
        }
      
    }
    //Si la nota es larga, se define el punto en el que se deja la puntuación y se añaden los puntos correspondientes.
    public void setReleasePointHit(float x, float y)
    {
        if (longNote&&pushing)
        {
            if(totalRecorrido > .75)
            {
                _level.addScore((1 - totalRecorrido)* perfectScore,3);

            }
            pushing = false;

            _level.notesLeft--;

            particle.transform.position = new Vector3(x, y, 0);
            Instantiate(particle);
            
            
            gameObject.SetActive(false);
        }
    }

    //Inicializar la nota
    public bool setNote (int num, int nTicks, int track, float vel)
    {

        nNote = num;

        tickDuration = nTicks;
        numTrack = track;

        int aux = num % 12;

        if (tickDuration < 100) tickDuration = 100;

        sprite = GetComponent<SpriteRenderer>();

        if (aux == 1 || aux == 3 || aux == 6 || aux == 8 || aux == 10)
        {
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x / 1.6f, (float)(tickDuration) / 2, 0);
            sprite.color = new Color(0, .8f, 1f, .75f);
        }

        else gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, (float)(tickDuration) / 2, 0);

        _vel = vel;

       

        if (tickDuration > 200)
        {
            
            longNote = true;
        }
        else longNote = false;

        return true;

    }
    // Update de la nota
    void Update()
    {
        //Si esta en movimiento
        if (moving)
        {
            //Aquí se van añadiendo poco a poco los puntos directamente, sin pasar por el addScore, ya que se hace en las notas largas
            if (pushing && longNote )
            {
                if ( pushPoint.y < transform.position.y + transform.localScale.y / 2 / 100) {

                    caminoRecorrido = (_vel * Time.deltaTime) * 100 / tickDuration;
                    totalRecorrido += caminoRecorrido;
                    addScore(totalDistance, caminoRecorrido);

                }
            }
            //Si ha llegado al final del nivel, se desactiva la nota
            if(transform.position.y < stopMovingAt - tickDuration/100)
            {
                moving = false;
                _level.notesLeft--;
                _level.addScore(0, -1);
                gameObject.SetActive(false);
            }

            //Movimiento de la nota
            transform.Translate(Vector3.down * _vel * Time.deltaTime);
        }

    }
}
