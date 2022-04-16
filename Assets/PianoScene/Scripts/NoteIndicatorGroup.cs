using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject[] scales;
    public GameObject scale;

    public int numTeclas; //Número teclas
   
    public int startingScale; //La escala mas baja
    public int endingScale; //La escala mas Alta

    float offsetX;

    public AudioClip[] audioClips = new AudioClip[120];

    public GameObject camera;

    public NoteIndicator [] noteIndicators;

    public bool recording;
    public RecordLevel recordLevel;

    public void iniciate()
    {
        int numEscalas = endingScale - startingScale + 1;
        scales = new GameObject[numEscalas];

        numTeclas = (endingScale - startingScale + 1) * 12;

        noteIndicators = new NoteIndicator [numTeclas];

        offsetX = 1;
        int j = 0;
        scales = new GameObject[11];
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

            //Asignamos el numero de nota a cada uno
            noteIndicators[scaleJaux].noteNumber = posScaleAux;
            noteIndicators[scaleJaux + 1].noteNumber = posScaleAux+1;
            noteIndicators[scaleJaux + 2].noteNumber = posScaleAux+2;
            noteIndicators[scaleJaux + 3].noteNumber = posScaleAux+3;
            noteIndicators[scaleJaux + 4].noteNumber = posScaleAux+4;
            noteIndicators[scaleJaux + 5].noteNumber = posScaleAux+5;
            noteIndicators[scaleJaux + 6].noteNumber = posScaleAux+6;
            noteIndicators[scaleJaux + 7].noteNumber = posScaleAux+7;
            noteIndicators[scaleJaux + 8].noteNumber = posScaleAux+8;
            noteIndicators[scaleJaux + 9].noteNumber = posScaleAux+9;
            noteIndicators[scaleJaux + 10].noteNumber = posScaleAux+10;
            noteIndicators[scaleJaux + 11].noteNumber = posScaleAux+11;

            //Asignar a los note indicators si es modo grabación
            for(int k = scaleJaux; k < scaleJaux + 12; k++)
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

    private void Start()
    {
        if (recording)
        {
            iniciate();
        }
    }

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

    public void assignAudioClip(GameObject go, int i)
    {

        go.GetComponent<AudioSource>().clip = audioClips[i];

    }
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

    public int getNumEscalas()
    {
        return 1+(endingScale - startingScale);
    }
}
