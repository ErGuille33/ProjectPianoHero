using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject prefab;

    public int numTeclas; //Número teclas
   
    public int startingNote; //LA Nota en la que empieza el teclado

    float offsetX;

    public AudioClip[] audioClips = new AudioClip[100];

    public GameObject camera;

    void Start()
    {
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
            go.GetComponent<NoteIndicator>().noteNumber = i;
            go.GetComponent<NoteIndicator>().setNNotes(numTeclas);
            assignAudioClip(go,i);
            j++;
        }
        
    }

    public void assignAudioClip(GameObject go, int i)
    {

        go.GetComponent<AudioSource>().clip = audioClips[i];

    }

}
