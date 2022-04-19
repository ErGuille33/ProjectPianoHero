using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject recImage;

    public GameObject recButton;
    public GameObject stopRecButton;
    public GameObject restartButton;
    public GameObject exportButton;

    bool finished = false;

    public void setStartTimer()
    {
        recButton.SetActive(false);
        stopRecButton.SetActive(false);
        restartButton.SetActive(false);
        recImage.SetActive(false);
        exportButton.SetActive(false);

        count_Down.start_count_down();
        count_Down.handler += this.countDownOver;
    }
    protected void countDownOver()
    {
        finishedTimer = true;
        noteIndicatorGroup.startRecording();

        recImage.SetActive(true);
        stopRecButton.SetActive(true);
        restartButton.SetActive(true);
    }

    //

    // Start is called before the first frame update
    void Start()
    {
        recordedMidiEvents = new List<MidiFile.MidiEvent>();
        midiRecorder = GetComponent<MidiRecorder>();
        //setStartTimer();

    }

    // Update is called once per frame
    void Update()
    {
        if (finishedTimer && !finished)
        {
            //Por ahora este valor parece funcionar (24* 32 ticks - 24 * 8 = 576)
            deltaTicks += (byte)(Time.deltaTime * 576);
            handleInput();
        }
    }

    public void finishRecord()
    {
   

        stopRecButton.SetActive(false);
        recImage.SetActive(false);

        restartButton.SetActive(true);
        exportButton.SetActive(true);

        finished = true;
        

    }

    public void Export()
    {
        Debug.Log("Grabado");
        for (int i = 0; i < recordedMidiEvents.Count; i++)
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

    public void restartLevel()
    {
        recordedMidiEvents.Clear();
        finishedTimer = false;
        finished = false;

        deltaTicks = 0;
        setStartTimer();
    }

    void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            finishRecord();
            
        }
    }
}
