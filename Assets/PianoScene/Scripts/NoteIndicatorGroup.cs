using System;
using UnityEngine;
//Este script se engarga de gestionar la creación de las distintas teclas del piano, y asignarles los valores y assets correspondientes
public class NoteIndicatorGroup : MonoBehaviour
{
    //Prefab de escalas y array de escalas
    public GameObject[] scales;
    public GameObject scale;

    public int numTeclas; //Número teclas
   
    public int startingScale; //La escala mas baja
    public int endingScale; //La escala mas Alta

    //Todos los sonidos de las notas
    public AudioClip[] audioClips = new AudioClip[120];

    //Ajustar la cámara al número de piezas
    public GameObject camera;

    //Array de teclas
    public NoteIndicator [] noteIndicators;

    //Modos de juego
    public bool recording;
    public RecordLevel recordLevel;


    public Level level;
    public bool modeGame;

    ObjectPool pool;

    //Métdo que instancia todas las teclas del piano y sus distintos valores dependiendo del número de escalas que se vayan a tener
    public void iniciate()
    {
        int numEscalas = endingScale - startingScale + 1;
        scales = new GameObject[numEscalas];

        numTeclas = (endingScale - startingScale + 1) * 12;

        //Solo instanciaremos los detectores de notas si estamos en game mode
        if (modeGame)
        {
            pool = gameObject.GetComponent<ObjectPool>();
            pool.amountToPool = numTeclas;
        }

        noteIndicators = new NoteIndicator [numTeclas];

        int j = 0;
        scales = new GameObject[11];
        //iniciamos las escalas y le asignamos el sonido correspondiente a cada nota
        for (var i = startingScale; i <= endingScale; i++)
        {
            int posScaleAux = i * 12;
            int scaleJaux = j * 12;

            scales[i] = Instantiate<GameObject>(scale);

            setScalesAndPositions(numEscalas, i,j);          

            //Asignamos clips de audio
            assignAudioClip(scales[i].transform.Find("C").gameObject, posScaleAux);
            assignAudioClip(scales[i].transform.Find("C#").gameObject, posScaleAux + 1);
            assignAudioClip(scales[i].transform.Find("D").gameObject, posScaleAux + 2);
            assignAudioClip(scales[i].transform.Find("D#").gameObject, posScaleAux + 3);
            assignAudioClip(scales[i].transform.Find("E").gameObject, posScaleAux + 4);
            assignAudioClip(scales[i].transform.Find("F").gameObject, posScaleAux + 5);
            assignAudioClip(scales[i].transform.Find("F#").gameObject, posScaleAux + 6);
            assignAudioClip(scales[i].transform.Find("G").gameObject, posScaleAux + 7);
            assignAudioClip(scales[i].transform.Find("G#").gameObject, posScaleAux + 8);
            assignAudioClip(scales[i].transform.Find("A").gameObject, posScaleAux + 9);
            assignAudioClip(scales[i].transform.Find("A#").gameObject, posScaleAux + 10);
            assignAudioClip(scales[i].transform.Find("B").gameObject, posScaleAux + 11);

            //Añadimos a lista de note indicators
            noteIndicators[scaleJaux] = scales[i].transform.Find("C").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+1] = scales[i].transform.Find("C#").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+2] = scales[i].transform.Find("D").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+3] = scales[i].transform.Find("D#").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+4] = scales[i].transform.Find("E").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+5] = scales[i].transform.Find("F").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+6] = scales[i].transform.Find("F#").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+7] = scales[i].transform.Find("G").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+8] = scales[i].transform.Find("G#").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+9] = scales[i].transform.Find("A").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+10] = scales[i].transform.Find("A#").gameObject.GetComponent<NoteIndicator>();
            noteIndicators[scaleJaux+11] = scales[i].transform.Find("B").gameObject.GetComponent<NoteIndicator>();

            //Asignamos el numero de nota a cada uno y el nivel si estamos en modo de juego
            for(int k = 0; k < 12; k++)
            {
                noteIndicators[scaleJaux + k].noteNumber = posScaleAux + k;
                if(modeGame)
                    noteIndicators[scaleJaux+k].level = level;
            }

            j++;
        }

        if (modeGame)
        {
            activateDetecting();
        }

    }
    //Asignar a los note indicators si es modo grabación
    public void startRecording()
    {
        int j = 0;
        for (var i = startingScale; i <= endingScale; i++)
        {
            int scaleJaux = j * 12;
            
            for (int k = scaleJaux; k < scaleJaux + 12; k++)
            {
                noteIndicators[k].setModeRecord(recording);
                if (recording)
                {
                    noteIndicators[k].setRecordLevel(recordLevel);
                }
            }
            j++;
        }
    }
    //Activar la detección de notas si es modo juego
    public void activateDetecting()
    {
        int j = 0;
        for (var i = startingScale; i <= endingScale; i++)
        {
            int scaleJaux = j * 12;
            //Asignar a los note indicators si es modo grabación
            for (int k = scaleJaux; k < scaleJaux + 12; k++)
            {
                noteIndicators[k].setModeGame(modeGame);
              
            }
            j++;
        }
    }

    //Devuelve la posición de la tecla, para de esta manera colocar las notas en su sitio
    public Vector3 getNoteIndicatorPos(int nNote)
    {
        Vector3 pos = new Vector3(0,0,0);
        int aux = 0;
        int i = 0;
        bool found = false;
        while (!found && i < noteIndicators.Length)
        {
            aux = noteIndicators[i].noteNumber;
            if(aux == nNote)
            {
                pos = noteIndicators[i].transform.position;
            }
            
            i++;
        }
        return pos;

    }
    //Calcula el número de teclas que vamos a necesitar en función del archivo MIDI que abramos, o en su defecto el número de escalas que elijamos
    public bool setNoteRange(int minNote, int maxNote)
    {
        //Le pnemos al teclado que la primera nota sea el do de la escala mas baja de la cancion, y el si de la mas alta
        startingScale = (int)System.Math.Truncate((double)minNote / 12);
        endingScale = (int)System.Math.Truncate( (double)maxNote / 12);

        if(startingScale <= 0 || endingScale > 7)
        {
            print("Midi inválido");
            return false;
        }
        else
        {
            return true;
        }
    }
    //Asignar el clip de audio
    public void assignAudioClip(GameObject go, int i)
    {

        go.GetComponent<AudioSource>().clip = audioClips[i];

    }

    public float getYDetecor()
    {
        return noteIndicators[3].getYDetectorPos();
    }

    //Para colocar las teclas en su sitio dependiendo del numero de escalas a representar
    private void setScalesAndPositions(int numEscalas, int i, int j)
    {
        if (numEscalas == 1)
        {
            scales[i].transform.localScale = new Vector3(2.3f, 2.3f, 1);
            scales[i].transform.position = new Vector3(0, 0, 0);
        }

        else if (numEscalas == 2)
        {
            scales[i].transform.localScale = new Vector3(2.3f, 2.3f, 1);
            scales[i].transform.position = new Vector3(-4.6f + (j * 8.6f), 0, 0);
        }

        else if (numEscalas == 3)
        {
            scales[i].transform.localScale = new Vector3(2.2f, 2.2f, 1);
            scales[i].transform.position = new Vector3(-8.25f + (j * 8.25f), -.1f, 0);
        }

        else if (numEscalas == 4)
        {
            scales[i].transform.localScale = new Vector3(1.7f, 1.7f, 1);
            scales[i].transform.position = new Vector3(-9.355f + (j * 6.25f), -.6f, 0);
        }

        else if (numEscalas == 5)
        {
            scales[i].transform.localScale = new Vector3(1.35f, 1.35f, 1);
            scales[i].transform.position = new Vector3(-10.2f + (j * 5.1f), -.9f, 0);
        }

        else if (numEscalas == 6)
        {
            scales[i].transform.localScale = new Vector3(1.15f, 1.15f, 1);
            scales[i].transform.position = new Vector3(-10.65f + (j * 4.25f), -1.15f, 0);
        }

        else if (numEscalas == 7)
        {
            scales[i].transform.localScale = new Vector3(1f, 1f, 1);
            scales[i].transform.position = new Vector3(-10.95f + (j * 3.65f), -1.25f, 0);
        }
                        
    }
    //Coger el número de escalas desde el nivel
    public int getNumEscalas()
    {
        return 1+(endingScale - startingScale);
    }
}
