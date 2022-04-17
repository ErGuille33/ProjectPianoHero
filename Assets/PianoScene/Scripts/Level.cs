using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Este es el script que controla el funcionamiento de nivel, desde crear las distintas notas, manejar la puntuación, gameplay ...etc

public class Level : MonoBehaviour
{


    int timePerColumn = 100;

    public MidiFile midiFile;
    public NoteIndicatorGroup indicatorGroup;


     List<MidiFile.MidiTrack> midiTracks;

    int numOctava = 0;

    public GameObject prefabNote;

    List<Note> notes;

    [Header("count down")]
    public Count_down count_Down;

    bool finishedTimer = false;

    protected void setStartTimer()
    {
        count_Down.start_count_down();
        count_Down.handler += this.countDownOver;
    }
    protected void countDownOver()
    {
        finishedTimer = true;
        //Start note movement
        foreach(Note note in notes)
        {
            note.moving = true;
        }
    }

    private void Start()
    {
        midiFile = GetComponent<MidiFile>();
        midiTracks = midiFile.getMidiFileTracks();

        //Nota minima y maxima
        List<int> _notes = new List <int>();
 
        int i = 0;
        foreach (MidiFile.MidiTrack track in midiTracks)
        {
            foreach (MidiFile.MidiNote note in track.vecNotes)
            {
                if (note.nKey > 0 && note.nKey < 100)
                {
                    _notes.Add (note.nKey);
 
                }
            }
           
            i++;
        }

        indicatorGroup.setNoteRange(_notes.Min(), _notes.Max());

        indicatorGroup.iniciate();
        movimientoVisualNotas();

        setStartTimer();

    }


    void movimientoVisualNotas()
    {
        int aux = 0;
        int numTrack = 0;
        numOctava = indicatorGroup.getNumEscalas();
        notes = new List<Note>();
       foreach(MidiFile.MidiTrack track in midiTracks)
        {
            if(track.vecNotes != null && track.vecNotes.Any())
            {

                //Rango de notas
                int nNoteRange = track.nMaxNote - track.nMinNote;

                foreach (MidiFile.MidiNote note in track.vecNotes)
                {
                    GameObject noteAux;
                    noteAux = Instantiate(prefabNote);
                    
                    noteAux.transform.position = new Vector3(indicatorGroup.getNoteIndicatorPos(note.nKey).x,(note.nStartTime-10)/timePerColumn+15,0);

                    if(numOctava >= 4)
                        noteAux.transform.localScale = new Vector3( 50f/ (numOctava), 0, 0);
                    else
                        noteAux.transform.localScale = new Vector3(25f, 0, 0);

                    noteAux.GetComponent<Note>().setNote(note.nKey,(int)note.nDuration,numTrack, 5f);

                    notes.Add(noteAux.GetComponent<Note>());

                    aux++;

                }

            }
            numTrack++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
