using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Este script consiste en la "coreografía" de las animaciones de los distintos elementos al enseñar la pantalla de puntuación tras completar un nivel

public class ScoreCoreo : MonoBehaviour
{
    Data data;

    int totalScore;
    int actualScore = 0;
    int numLevel;

    int userLevel;
    int previousUserLevel;

    public Text scoreText;
    public Text userLevelText;
    public Text menuText;

    public Image progressCircle;
    public Button menuButton;
    public Image menuImage;

     float progressPerc;
     float prevXP;
     float actualXP;


    string scoreString;


    //Cargamos los datos de el archivo de guardado y comenzamos
    void Start()
    {
        data = SaveController.LoadData();

        scoreString = "Score\n" + actualScore;
        scoreText.text = scoreString;
        int i = 0;

        foreach (Data.LevelData levelData in data.levelsData)
        {
            if (levelData.levelName == data.previousLevel)
            {
                break;
            }
            i++;
        }

        numLevel = i;

        totalScore = (int)data.levelsData[numLevel].lastScore;

        userLevel = data.levelPlayer;
        previousUserLevel = data.previousLevelPlayer;
        userLevelText.text = "" + data.previousLevelPlayer;

        prevXP = data.previousExpPoints;
        actualXP = data.expPoints;

        progressPerc = prevXP / data.getMaxXp(previousUserLevel);
        progressCircle.fillAmount = progressPerc;

        if (totalScore == 0)
        {
            StartCoroutine(menuButtonAdd());
        }
        else { 
        //Comenzamos las corrutinas
        StartCoroutine(growScore());
        }

    }
    //Enseña la puntuación creciendo desde cero
    IEnumerator growScore()
    {
        for (int i = 0; i <= totalScore; i = i + totalScore / 600)
        {
            actualScore = i;
            scoreString = "Score\n" + actualScore;
            scoreText.text = scoreString;
            yield return null;
        }
        actualScore = totalScore;
        scoreString = "Score\n" + actualScore;
        scoreText.text = scoreString;


        StartCoroutine(growXP());
    }
    //Sube la barra de experiencia
    IEnumerator growXP()
    {

        if (userLevel == previousUserLevel)
        {
            for (float i = progressPerc; i <= actualXP / data.getMaxXp(userLevel); i = i + .005f)
            {
                progressPerc = i;
                progressCircle.fillAmount = progressPerc;

                yield return null;
            }
        }

        else if (userLevel != previousUserLevel)
        {

            for (float i = progressPerc; i <= 1; i = i + .005f)
            {
                progressPerc = i;
                progressCircle.fillAmount = progressPerc;

                yield return null;
            }

            StartCoroutine(changeLevel());

            progressPerc = 0;

            for (float i = progressPerc; i <= actualXP / data.getMaxXp(userLevel); i = i + .005f)
            {
                progressPerc = i;
                progressCircle.fillAmount = progressPerc;

                yield return null;
            }

        }

        StartCoroutine(menuButtonAdd());

    }
    //Animación boton de menu
    IEnumerator menuButtonAdd()
    {
        Color color = new Color(menuImage.color.r, menuImage.color.g, menuImage.color.b, 0);
        Color color1 = new Color(menuText.color.r, menuText.color.g, menuText.color.b, 0);

        for (float i = 0; i <= 1; i = i + .005f)
        {
            color.a += .005f;
            menuImage.color = color;

            color1.a += .005f;
            menuText.color = color1;
            yield return null;
        }

        menuButton.enabled = true;
    }
    //Animación de subir nivel 1
    IEnumerator changeLevel()
    {
        Color color = userLevelText.color;

        for (float i = 0; i <= 1; i = i + .005f)
        {
            color.a -= .005f;
            userLevelText.color = color;
            yield return null;

        }

        userLevelText.text = "" + data.levelPlayer;


        StartCoroutine(changeLevelUp());

    }
    //Animación de subir nivel 2
    IEnumerator changeLevelUp()
    {
        Color color = new Color(userLevelText.color.r, userLevelText.color.g, userLevelText.color.b,0);
        for (float i = 0; i <= 1; i = i + .0005f)
        {
            color.a += .005f;
            userLevelText.color = color;
            yield return null;
        }
    }

}
