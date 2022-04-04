using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject prefab;

    public int numTeclas; //Número teclas
   
    public int startingNote; //LA Nota en la que empieza el teclado

    float offsetX;

    public AudioClip[] audioClips = new AudioClip[100];

    public GameObject camera;

    public NoteIndicator [] noteIndicators;

    public void iniciate()
    {
        noteIndicators = new NoteIndicator [numTeclas];

        offsetX = ( numTeclas / ((float)numTeclas / 25 )) / 2;
        int j = 0;

        for (var i = startingNote; i <  startingNote + numTeclas; i++)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.position = new Vector3(-offsetX + ((j % numTeclas)/((float)numTeclas /25)), 0, 0);
            if(numTeclas%2 == 0)
            {
                if (j == (numTeclas) / 2)
                {
                    camera.transform.position = new Vector3(go.transform.position.x - 1/((float)numTeclas / 25)/2 , 5, -1);
                }
            }
            else
            {
                if (j == (numTeclas) / 2)
                {
                    camera.transform.position = new Vector3(go.transform.position.x, 5, -1);
                }
            }
     
            go.transform.parent = transform;
            noteIndicators[j] = go.GetComponent<NoteIndicator>();
            noteIndicators[j].noteNumber = i;
            noteIndicators[j].setNNotes(numTeclas);
            assignAudioClip(go,i);
            j++;
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
        startingNote = minNote - (minNote%12);
        numTeclas = 2 + (maxNote - minNote) + 12-((maxNote - minNote)%12 );

        if(startingNote < 9 || numTeclas - startingNote > 97)
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

}
