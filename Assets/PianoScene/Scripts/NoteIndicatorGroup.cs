using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject[] pianoKeys;

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
            int aux = i%12;
            GameObject go;
            if (aux == 0 ) {  go = Instantiate<GameObject>(pianoKeys[0]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 1.20f, 0.2f, 0); }
            else if (aux == 1) { go = Instantiate<GameObject>(pianoKeys[1]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25))+1, 1.04f, 0); }
            else if (aux == 2) { go = Instantiate<GameObject>(pianoKeys[2]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 0.80f, 0.2f, 0); }
            else if (aux == 3) { go = Instantiate<GameObject>(pianoKeys[3]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 0.6f, 1.04f, 0); }
            else if (aux == 4) { go = Instantiate<GameObject>(pianoKeys[4]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 0.4f, 0.2f, 0); }
            else if (aux == 5) { go = Instantiate<GameObject>(pianoKeys[0]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 0.9f, 0.2f, 0); }  
            else if (aux == 6) {  go = Instantiate<GameObject>(pianoKeys[1]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25))+0.70f, 1.04f, 0); }
            else if (aux == 7) { go = Instantiate<GameObject>(pianoKeys[2]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) + 0.4f, 0.2f, 0); }
            else if (aux == 8) { go = Instantiate<GameObject>(pianoKeys[1]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25))+0.2f, 1.04f, 0); }          
            else if (aux == 9) { go = Instantiate<GameObject>(pianoKeys[2]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) , 0.2f, 0); }
            else if (aux == 10) {  go = Instantiate<GameObject>(pianoKeys[3]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25)) -.2f, 1.04f, 0); }
            
           
            else { go = Instantiate<GameObject>(pianoKeys[4]); go.transform.position = new Vector3(-offsetX + ((j % numTeclas) / ((float)numTeclas / 25))-.4f, 0.2f, 0); }


            if (numTeclas%2 == 0)
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
        numTeclas =  5+ (maxNote - minNote) + 12-((maxNote - minNote)%12 );

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
