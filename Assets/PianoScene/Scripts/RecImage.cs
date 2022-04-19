using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecImage : MonoBehaviour
{
    public float timeHide;
    public float timeShowing;

    private float elapsedTime;

    private bool hide = false;

    Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!hide)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > timeShowing)
            {
                hide = true;
                image.enabled = false;
                elapsedTime = 0;
            }
        }
        else if (hide)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > timeHide)
            {
                hide = false;
                image.enabled = true;
                elapsedTime = 0;
            }
        }
    }
}
