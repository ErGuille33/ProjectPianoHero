using UnityEngine;
using MidiJack;

//Básicamente, la tecla de piano
public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;


    public AudioSource source;
    private SpriteRenderer sprite;
    private bool pushed = false;

    private void Start()
    {
        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();
        
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
