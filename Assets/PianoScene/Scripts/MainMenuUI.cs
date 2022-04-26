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
    public float progressPerc;
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

    
    // 

    void Start()
    {
        data = SaveController.LoadData();

        if (data != null) {

        }
        else
        {
            List<Data.LevelData> auxList = new List<Data.LevelData>();
            data = new Data(0, 0, 1, 1, new bool[25], auxList, "",1,1,false,false);
        }

        userLevel = data.levelPlayer;
        actualXP = data.expPoints;

        progressPerc = actualXP / data.getMaxXp(userLevel);

        userLevelText.text = "" + userLevel;
        progressCircle.fillAmount = progressPerc;

        pianoSlider.value = data.volumePiano;
        fxSlider.value = data.volumeFx;

        changeMixerPiano(data.volumePiano);
        changeMixerFX(data.volumeFx);

    }

    public void openMenuCanvas()
    {
        playButton.enabled = false;
        recButton.enabled = false;
        optionsButton.enabled = false;
        awardButton.enabled = false;

        menuCanvas.SetActive(true);
    }

    public void closeMenuCanvas()
    {
        playButton.enabled = true;
        recButton.enabled = true;
        optionsButton.enabled = true;
        awardButton.enabled = true;

        SaveController.SaverData(data);

        menuCanvas.SetActive(false);
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
        Application.Quit();
    }



}
