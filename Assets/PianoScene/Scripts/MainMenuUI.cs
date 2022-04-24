using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    // Start is called before the first frame update
    void Start()
    {
        data = SaveController.LoadData();
        userLevel = data.levelPlayer;
        actualXP = data.expPoints;

        progressPerc = actualXP / data.getMaxXp(userLevel);

        userLevelText.text = "" + userLevel;
        progressCircle.fillAmount = progressPerc;

    }

}
