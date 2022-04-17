using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{
    [Header("count down")]
    public Count_down count_down;

    // Use this for initialization
    void Start()
    {
        //bingding over event
        this.count_down.handler += this.over;
    }

    public void over()
    {
        print("count down is over");
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 100), "start count down"))
        {
            this.count_down.start_count_down();
        }
    }
}
