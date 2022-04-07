using UnityEngine;
using MidiJack;

//Básicamente, la tecla de piano
public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;
    int nNotes = 1;

    public AudioSource source;
    private SpriteRenderer sprite;
    private bool pushed = false;

    private void Start()
    {
        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();
        transform.localScale = new Vector3(2.5f,2.5f,2.5f);
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        if (MidiMaster.GetKeyDown(noteNumber)) playNote(MidiMaster.GetKey(noteNumber));
        else if (MidiMaster.GetKeyUp(noteNumber)) { sprite.enabled = true; pushed = false; }
    
    }

    void playNote(float volume)
    {
        source.Play();
        source.volume = volume;
        print(noteNumber);
        sprite.enabled = false;
        pushed = true;

        
    }
    
    public void setNNotes(int num)
    {
        nNotes = num;
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
      
        if (pushed)
        {
           
            if (other.tag == "Note")
            {
                Destroy(other);
            }
        }
    }





}
