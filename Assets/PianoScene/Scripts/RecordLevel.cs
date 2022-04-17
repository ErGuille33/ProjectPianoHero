using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordLevel : MonoBehaviour
{
    //offset entre notas
    List<MidiFile.MidiEvent> recordedMidiEvents;
    public NoteIndicatorGroup noteIndicatorGroup;

    MidiRecorder midiRecorder;

    [Header("count down")]
    public Count_down count_Down;

    uint deltaTicks;

    bool finishedTimer = false;

    protected void setStartTimer()
    {
        count_Down.start_count_down();
        count_Down.handler += this.countDownOver;
    }
    protected void countDownOver()
    {
        finishedTimer = true;
        noteIndicatorGroup.startRecording();
    }

    //
    bool finished = false;
    // Start is called before the first frame update
    void Start()
    {
        recordedMidiEvents = new List<MidiFile.MidiEvent>();
        midiRecorder = GetComponent<MidiRecorder>();
        setStartTimer();

    }

    // Update is called once per frame
    void Update()
    {
        if (finishedTimer)
        {
            //Por ahora este valor parece funcionar (24* 32 ticks - 24 * 8 = 576)
            deltaTicks += (byte)(Time.deltaTime * 576);
            handleInput();
        }
    }

    void finishRecord()
    {
        finished = true;
        for(int i = 0; i < recordedMidiEvents.Count; i++)
        {
            midiRecorder.addMidiNote(recordedMidiEvents[i]);
        }
        midiRecorder.openMidiFile();

    }

    public void addEventToPool(MidiFile.MidiEvent.Type type, byte noteNumber, byte vel)
    {
        recordedMidiEvents.Add(new MidiFile.MidiEvent(type,(byte) (noteNumber+12), vel, deltaTicks));
        deltaTicks = 0;
    }

    void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            finishRecord();
            Debug.Log("Grabado");
        }
    }
}
