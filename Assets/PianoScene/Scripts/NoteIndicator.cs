using UnityEngine;
using MidiJack;

public class NoteIndicator : MonoBehaviour
{
    public int noteNumber;
    public NotePlayer notePlayer;

    public AudioSource source;

    private void Start()
    {
        if(GetComponent<AudioSource>() != null)
            source = GetComponent<AudioSource>();
    }
    void Update()
    {
        transform.localScale = Vector3.one * (0.1f + MidiMaster.GetKey(noteNumber));

        var color = MidiMaster.GetKeyDown(noteNumber) ? Color.red : Color.white;
        GetComponent<Renderer>().material.color = color;

        notePlayer.noteAssign(noteNumber);
        if (MidiMaster.GetKeyDown(noteNumber)) playNote(MidiMaster.GetKey(noteNumber));
    }

    void playNote(float volume)
    {
        source.Play();
        source.volume = volume;
        print(noteNumber);

        
    }

   
               
        
    
}
