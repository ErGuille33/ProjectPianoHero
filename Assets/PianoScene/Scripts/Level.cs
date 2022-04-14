using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Este es el script que controla el funcionamiento de nivel, desde crear las distintas notas, manejar la puntuación, gameplay ...etc

public class Level : MonoBehaviour
{

    int offsetX = 0;
    int noteHeight = 1;
    int timePerColumn = 50;

    //offset entre notas
    int nTrackOffset = 1;

    public MidiFile midiFile;
    public NoteIndicatorGroup indicatorGroup;

     List<MidiFile.MidiTrack> midiTracksAux;
     List<MidiFile.MidiTrack> midiTracks;

    int numOctava = 0;

    public GameObject prefabNote;

    private void Start()
    {
        midiFile = GetComponent<MidiFile>();
        midiTracksAux = midiFile.getMidiFileTracks();
        midiTracks = midiTracksAux;

        //Nota minima y maxima
        List<int> notes = new List <int>();
    
        int i = 0;
        foreach (MidiFile.MidiTrack track in midiTracks)
        {
            foreach (MidiFile.MidiNote note in track.vecNotes)
            {
                if (note.nKey > 0 && note.nKey < 100)
                {
                    notes.Add (note.nKey);
 
                }
                

            }
           
            i++;
        }

        indicatorGroup.setNoteRange(notes.Min(), notes.Max());

        indicatorGroup.iniciate();
        movimientoVisualNotas();

    }


    void movimientoVisualNotas()
    {
        int aux = 0;
        int numTrack = 0;
        numOctava = indicatorGroup.getNumEscalas();
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
                    
                    noteAux.transform.position = new Vector3(indicatorGroup.getNoteIndicatorPos(note.nKey).x,(note.nStartTime-nTrackOffset)/timePerColumn+2,0);
                    noteAux.transform.localScale = new Vector3(.2f, .2f, 0);

                    noteAux.GetComponent<Note>().setNote(note.nKey,(int)note.nDuration,numTrack);
                    aux++;

                }
                offsetX += 1;
            }
            numTrack++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
