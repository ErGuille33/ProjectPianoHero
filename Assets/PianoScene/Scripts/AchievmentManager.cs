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


    void Start()
    {
        try
        {
            data = SaveController.LoadData();

            if (data != null)
            {

                setElements();

            }
        }
        catch(Exception e)
        {
            print(e);
        }
    }

    void setElements()
    {
 
        rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, 110 * data.levelsData.Count);


        for (int i = 0; i < data.levelsData.Count; i++)
        {
            GameObject aux = Instantiate(gameObject);
            if (data.levelsData[i].score > 70000)
            {
                aux.GetComponent<Image>().color = new Color(0.3023318f, 0.8113208f, 0.3607404f);
            }
           

            aux.transform.SetParent(transform);

            aux.transform.Find("Level").GetComponent<Text>().text = "Nivel : " + data.levelsData[i].levelName ;

            aux.transform.Find("Score").GetComponent<Text>().text = "Score : " +(int) data.levelsData[i].score + " |";

            aux.transform.Find("NumBads").GetComponent<Text>().text = "Bad : " + data.levelsData[i].numBad + " |";

            aux.transform.Find("NumOk").GetComponent<Text>().text = "Ok : " + data.levelsData[i].numOk + " |";

            aux.transform.Find("NumGood").GetComponent<Text>().text = "Good : " + data.levelsData[i].numGood + " |";

            aux.transform.Find("NumPerf").GetComponent<Text>().text = "Perfect : " + data.levelsData[i].numPerf + " |";

            aux.transform.Find("NumMiss").GetComponent<Text>().text = "Miss : " + data.levelsData[i].numNan + " |";

            aux.transform.Find("Vel").GetComponent<Text>().text = "Vel : " + data.levelsData[i].vel + " |";

        }

    }

}
