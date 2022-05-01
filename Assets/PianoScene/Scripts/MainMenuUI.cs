using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    enum menuStates
    {
        idl= 0,
        options = 1,
        logros = 2
    }

    Data data;
    float progressPerc;
    float actualXP;
    int userLevel;

    public Image progressCircle;
    public Text userLevelText;

    public GameObject menuCanvas;

    public Button playButton;
    public Button recButton;
    public Button optionsButton;
    public Button awardButton;

    public Slider fxSlider;
    public Slider pianoSlider;
    public AudioMixer mixer;

    public AudioSource blupAudio;
    public AudioSource tickAudio;

    public bool teacherMode;
    public GameObject teacherCanvas;
    public GameObject studentCanvas;

    public GameObject mensajeCanvas;
    public GameObject textoMensajeProfesor;
    public GameObject textoMensajeAlumno;

    // 

    protected void setMenuCanvas(bool on)
    {
        if (on)
        {
            if (teacherMode)
            {
                teacherCanvas.SetActive(true);
                studentCanvas.SetActive(false);
            }
            else
            {
                teacherCanvas.SetActive(false);
                studentCanvas.SetActive(true);

            }
        }
        else
        {
            teacherCanvas.SetActive(false);
            studentCanvas.SetActive(false);
        }
    }

    void Start()
    {
   

        data = SaveController.LoadData();

        if (data != null) {
            init();
        }
        else
        {
            List<Data.LevelData> auxList = new List<Data.LevelData>();
            data = new Data(0, 0, 1, 1, new bool[25], auxList, "",1,1,false,false);

            mensajeCanvas.SetActive(true);

            if (teacherMode)
            {
                textoMensajeAlumno.SetActive(false);
                textoMensajeProfesor.SetActive(true);
            }
            else
            {
                textoMensajeAlumno.SetActive(true);
                textoMensajeProfesor.SetActive(false);
            }
        }


    }

    private void init()
    {

        userLevel = data.levelPlayer;
        actualXP = data.expPoints;

        progressPerc = actualXP / data.getMaxXp(userLevel);

        userLevelText.text = "" + userLevel;
        progressCircle.fillAmount = progressPerc;

        pianoSlider.value = data.volumePiano;
        fxSlider.value = data.volumeFx;

        changeMixerPiano(data.volumePiano);
        changeMixerFX(data.volumeFx);

        setMenuCanvas(true);
    }

    public void closeMensaje()
    {
        mensajeCanvas.SetActive(false);
        tickAudio.Play();

        init();

    }

    public void openMenuCanvas()
    {
        blupAudio.Play();
        playButton.enabled = false;
        recButton.enabled = false;
        optionsButton.enabled = false;
        awardButton.enabled = false;

        menuCanvas.SetActive(true);
        setMenuCanvas(false);
    }

    public void closeMenuCanvas()
    {

        tickAudio.Play();
        playButton.enabled = true;
        recButton.enabled = true;
        optionsButton.enabled = true;
        awardButton.enabled = true;

        SaveController.SaverData(data);

        menuCanvas.SetActive(false);
        setMenuCanvas(true);

    }

    public void changeMixerPiano()
    {
        mixer.SetFloat("Piano", Mathf.Log10(pianoSlider.value)*20);
        data.volumePiano = pianoSlider.value;

    }
    public void changeMixerFX()
    {
        mixer.SetFloat("Fx", Mathf.Log10(fxSlider.value) * 20);
        data.volumeFx = fxSlider.value;

    }

    public void changeMixerPiano(float sliderValue)
    {
        mixer.SetFloat("Piano", Mathf.Log10(pianoSlider.value) * 20);
        data.volumePiano = pianoSlider.value;

    }
    public void changeMixerFX(float sliderValue)
    {
        mixer.SetFloat("Fx", Mathf.Log10(fxSlider.value) * 20);
        data.volumeFx = fxSlider.value;

    }

    public void closeApp()
    {
        tickAudio.Play();
        Application.Quit();
    }



}
