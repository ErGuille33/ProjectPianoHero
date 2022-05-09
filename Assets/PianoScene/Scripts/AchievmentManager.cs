using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievmentManager : MonoBehaviour
{
    Data data;

    public GameObject gameObject;

    public RectTransform rectTransform;

    public float progressPerc;
    float actualXP;
    int userLevel;

    public Image progressCircle;
    public Text userLevelText;


    void Start()
    {
        try
        {
            data = SaveController.LoadData();

            if (data != null)
            {

                setElements();
                userLevel = data.levelPlayer;
                actualXP = data.expPoints;

                progressPerc = actualXP / data.getMaxXp(userLevel);

                userLevelText.text = "" + userLevel;
                progressCircle.fillAmount = progressPerc;


            }
            else if(data == null)
            {
                progressPerc = 0;
                userLevelText.text = "" + 1;
                progressCircle.fillAmount = progressPerc;
            }
        }
        catch (Exception e)
        {
            print(e);
        }
    }

    void setElements()
    {

        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 110 * data.levelsData.Count);

        this.rectTransform.position =new Vector2(this.rectTransform.position.x, (-110 * data.levelsData.Count));

        for (int i = data.levelsData.Count - 1; i >= 0; i--)
        {
            if (data.levelsData[i].levelName == data.previousLevel)
            {
                GameObject aux = Instantiate(gameObject);
                aux.transform.GetComponent<Image>().color = Color.blue;

                aux.transform.SetParent(transform);

                aux.transform.Find("Level").GetComponent<Text>().text = "Nivel : " + data.levelsData[i].levelName;

                aux.transform.Find("Score").GetComponent<Text>().text = "Score : " + (int)data.levelsData[i].score;

                aux.transform.Find("NumBads").GetComponent<Text>().text = "Bad : " + data.levelsData[i].numBad;

                aux.transform.Find("NumOk").GetComponent<Text>().text = "Ok : " + data.levelsData[i].numOk;

                aux.transform.Find("NumGood").GetComponent<Text>().text = "Good : " + data.levelsData[i].numGood;

                aux.transform.Find("NumPerf").GetComponent<Text>().text = "Perfect : " + data.levelsData[i].numPerf;

                aux.transform.Find("NumMiss").GetComponent<Text>().text = "Miss : " + data.levelsData[i].numNan;

                aux.transform.Find("Vel").GetComponent<Text>().text = "Vel : " + data.levelsData[i].vel;

                if (data.levelsData[i].score > 70000)
                {
                    aux.transform.Find("Tick").gameObject.SetActive(true);
                }
            }
        }
        for (int i = data.levelsData.Count - 1; i >= 0; i--)
        {
            if (data.levelsData[i].levelName != data.previousLevel)
            {
                GameObject aux = Instantiate(gameObject);

                if (data.levelsData[i].score > 70000)
                {
                    aux.transform.Find("Tick").gameObject.SetActive(true);
                }

                aux.transform.SetParent(transform);

                aux.transform.Find("Level").GetComponent<Text>().text = "Nivel : " + data.levelsData[i].levelName;

                aux.transform.Find("Score").GetComponent<Text>().text = "Score : " + (int)data.levelsData[i].score;

                aux.transform.Find("NumBads").GetComponent<Text>().text = "Bad : " + data.levelsData[i].numBad;

                aux.transform.Find("NumOk").GetComponent<Text>().text = "Ok : " + data.levelsData[i].numOk;

                aux.transform.Find("NumGood").GetComponent<Text>().text = "Good : " + data.levelsData[i].numGood;

                aux.transform.Find("NumPerf").GetComponent<Text>().text = "Perfect : " + data.levelsData[i].numPerf;

                aux.transform.Find("NumMiss").GetComponent<Text>().text = "Miss : " + data.levelsData[i].numNan;

                aux.transform.Find("Vel").GetComponent<Text>().text = "Vel : " + data.levelsData[i].vel;
            }

        }

    }

}
