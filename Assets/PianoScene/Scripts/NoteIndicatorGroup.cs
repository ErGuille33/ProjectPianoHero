using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject prefab;

    public int numTeclas; //Número teclas
   
    public int startingOctave; //LA OCTAvA en la que empieza el teclado

    public int lengthOvtaves; // Número de octavas del teclado

    public AudioClip[] audioClips = new AudioClip[100];

    void Start()
    {
        int j = 0;
        for (var i = 12 * startingOctave ; i < (12 * startingOctave) + numTeclas; i++)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.position = new Vector3(j % numTeclas, 0, 0);
            go.transform.parent = transform;
            go.GetComponent<NoteIndicator>().noteNumber = i;
            assignAudioClip(go,i);
            j++;
        }
    }

    public void assignAudioClip(GameObject go, int i)
    {

        go.GetComponent<AudioSource>().clip = audioClips[i];

    }

}
