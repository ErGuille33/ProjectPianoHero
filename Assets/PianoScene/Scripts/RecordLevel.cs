using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script que administra las pantallas de grabación de MIDI en tiempo real
public class RecordLevel : MonoBehaviour
{
    //Lista de eventos MIDI grabados
    List<MidiFile.MidiEvent> recordedMidiEvents;
    //Referencia al script que administra las teclas del piano
    public NoteIndicatorGroup noteIndicatorGroup;
    //Referencia al archivo que escribe MIDI
    MidiRecorder midiRecorder;
    //Para la cuenta atrás
    [Header("count down")]
    public Count_down count_Down;
    bool finishedTimer = false;
    //Los ticks para la grabación
    uint deltaTicks;

    //UI
    public GameObject recImage;
    public GameObject recButton;
    public GameObject stopRecButton;
    public GameObject restartButton;
    public GameObject exportButton;
    public GameObject menuButton;
    public GameObject menuButton_1;

    public GameObject avisoFrame;
    public GameObject closeAvisoButton;

    public GameObject octFrame;
    public GameObject setOctButton;
    public Slider iniOctSlider;
    public Slider lastOctSlider;
    public Text iniOctText;
    public Text lastOctText;
    public GameObject errorText;

    //Final de grabación
    bool finished = false;
    //Datos guardados
    Data saveData;

    public FileManager fileManager;
    string filename;

    public ManageScenes scenes;

    public AudioSource tickAudio;
    public AudioSource blupAudio;

    public GameObject notePrefab;

    //Empezar el timer y ajustar valores de UI
    public void setStartTimer()
    {
        recButton.SetActive(false);
        stopRecButton.SetActive(false);
        restartButton.SetActive(false);
        recImage.SetActive(false);
        exportButton.SetActive(false);
        menuButton.SetActive(false);
        menuButton_1.SetActive(false);

        count_Down.start_count_down();
        count_Down.handler += this.countDownOver;
    }

    //Al terminar el timer y ajustar valores UI
    protected void countDownOver()
    {
        finishedTimer = true;
        noteIndicatorGroup.startRecording();

        recImage.SetActive(true);
        stopRecButton.SetActive(true);
        restartButton.SetActive(true);
    }

    public void iniciate()
    {
        recordedMidiEvents = new List<MidiFile.MidiEvent>();
        midiRecorder = GetComponent<MidiRecorder>();
        noteIndicatorGroup.iniciate();
    }
    //Antes de elegir la octava
    public void chooseOct()
    {
        octFrame.SetActive(true);
    }
    //Ui octava
    public void changeStartOct()
    {
        iniOctText.text = "Octava Inicial: " + iniOctSlider.value;
    }
    //Ui octava
    public void changeEndOct()
    {
        lastOctText.text = "Octava Final: " + lastOctSlider.value;
    }
    //Settear la octava elegida
    public void setOctave()
    {
        tickAudio.Play();
        if (lastOctSlider.value >= iniOctSlider.value)
        {
            noteIndicatorGroup.startingScale = (int)iniOctSlider.value;
            noteIndicatorGroup.endingScale = (int)lastOctSlider.value;
            errorText.SetActive(false);
            octFrame.SetActive(false);
            iniciate();
        }
        else
        {
            errorText.SetActive(true);
        }

    }

    void Start()
    {
        saveData = SaveController.LoadData();

        if (saveData != null)
        {
            if (!saveData.alreadyPlayed)
            {
                openAvisoCanvas();
            }
            else { chooseOct(); }
        }
        else { openAvisoCanvas(); }



    }

    //Update al recibir los imputs al grabar
    void Update()
    {
        if (finishedTimer && !finished)
        {
            //Por ahora este valor parece funcionar (24* 32 ticks - 24 * 8 = 576)
            deltaTicks += (byte)(Time.deltaTime * 535);
            handleInput();
        }
    }

    //Al terminar de grabar
    public void finishRecord()
    {

        tickAudio.Play();
        stopRecButton.SetActive(false);
        recImage.SetActive(false);

        restartButton.SetActive(true);
        exportButton.SetActive(true);

        finished = true;
    }

    IEnumerator saveFile()
    {

        filename = "";

        StartCoroutine(fileManager.SaveFileExplorer());

        while (filename == "")
        {
            filename = fileManager.getPath();
            yield return null;
        }

        if (filename == "cancel")
        {
            scenes.changeScene("MainMenu");
        }

        midiRecorder.openMidiFile(filename);

        saveData = SaveController.LoadData();

        if (saveData == null)
        {
            saveData = new Data(0, 0, 1, 1, new bool[25], new List<Data.LevelData>(), "", 1, 1,false, true);
        }
        try
        {
            saveData.alreadyRecorded = true;
            SaveController.SaverData(saveData);


            menuButton.SetActive(true);
        }
        catch (Exception e) { scenes.changeScene("MainMenu"); }




    }

    //Al pulsar el botón de exportar
    public void Export()
    {
        blupAudio.Play();
        restartButton.SetActive(false);
        Debug.Log("Grabado");
        for (int i = 0; i < recordedMidiEvents.Count; i++)
        {
            midiRecorder.addMidiNote(recordedMidiEvents[i]);
        }
        StartCoroutine(saveFile());
    }

    //Añadir un evento a la lista de eventos. Se llama desde Note Indicator
    public void addEventToPool(MidiFile.MidiEvent.Type type, byte noteNumber, byte vel)
    {
        recordedMidiEvents.Add(new MidiFile.MidiEvent(type, (byte)(noteNumber), vel, deltaTicks));
        deltaTicks = 0;
    }
    //Boton de restart
    public void restartLevel()
    {
        recordedMidiEvents.Clear();
        midiRecorder.clearList();
        finishedTimer = false;
        finished = false;

        deltaTicks = 0;
        setStartTimer();
    }
    //Teclas del teclado auxiliares a los botones normales
    void handleInput()
    {
        //Finalizar la grabación
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            finishRecord();

        }
        //Reempezar la grabación
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartLevel();

        }
    }

    public void closeAvisoCanvas()
    {

        avisoFrame.SetActive(false);
        closeAvisoButton.SetActive(false);
        menuButton_1.SetActive(true);
        recButton.SetActive(true);
        chooseOct();

        tickAudio.Play();
    }

    public void openAvisoCanvas()
    {
        avisoFrame.SetActive(true);
        closeAvisoButton.SetActive(true);
        menuButton_1.SetActive(false);
        recButton.SetActive(false);
    }

    public void playBlupAuido()
    {
        tickAudio.Play();
    }

    public void createNote(float posX, int nNote)
    {

        GameObject note  = Instantiate(notePrefab);
        note.transform.position = new Vector3(posX, gameObject.transform.position.y,0);
      
        note.GetComponent<Note>().setInverseMovement();

        if (noteIndicatorGroup.getNumEscalas() >= 4)
            note.transform.localScale = new Vector3(30f / (noteIndicatorGroup.getNumEscalas()), 0, 0);
        else
            note.transform.localScale = new Vector3(20f, 0, 0);

        note.GetComponent<Note>().setNote(nNote, 100, 0, 1);
        note.GetComponent<SpriteRenderer>().color = new Color(note.GetComponent<SpriteRenderer>().color.r, note.GetComponent<SpriteRenderer>().color.g, note.GetComponent<SpriteRenderer>().color.b, .25f);

    }

}
