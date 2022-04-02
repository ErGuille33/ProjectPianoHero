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
    int nTrackOffset = 0;

    public MidiFile midiFile;

     List<MidiFile.MidiTrack> midiTracksAux;
     List<MidiFile.MidiTrack> midiTracks;

    public GameObject prefabNote;

    // Start is called before the first frame update
    void Start()
    {
        midiFile = GetComponent<MidiFile>();
        midiTracksAux = midiFile.getMidiFileTracks();
        midiTracks = midiTracksAux;
        movimientoVisualNotas();
    }

    void movimientoVisualNotas()
    {
        int numTrack = 0;
       foreach(MidiFile.MidiTrack track in midiTracks)
        {
            if(track.vecNotes != null && track.vecNotes.Any())
            {
                print("nota");
                //Rango de notas
                int nNoteRange = track.nMaxNote - track.nMinNote;

                foreach (MidiFile.MidiNote note in track.vecNotes)
                {
                    GameObject noteAux;
                    noteAux = Instantiate(prefabNote);
                    noteAux.transform.position = new Vector3((nNoteRange - (note.nKey - track.nMinNote)) * noteHeight + offsetX ,(note.nStartTime - nTrackOffset) / timePerColumn,0);
                    noteAux.GetComponent<Note>().setNote(note.nKey,(int)note.nDuration,numTrack);

                }
                offsetX += (nNoteRange + 1) * noteHeight + 4;
            }
            numTrack++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
