using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Haciendo uso de rayCast, detectar la distancia de la nota más cercana al punto de detección
public class Detector : MonoBehaviour
{
    float offset = 0f;
    
    public float detectNoteDistance()
    {
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, Vector2.down);
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if (hit0.collider != null)
        {

            if (hit0.collider.transform.tag == "Note")
            {

                print(hit0.collider.name + hit0.collider.transform.position);

                float distance = Mathf.Abs(hit0.point.y + offset - transform.position.y);
                if (distance < 2)
                {
                    hit0.transform.gameObject.SetActive(false);
                    return distance;
                }
            }
        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        Debug.DrawRay(transform.position, Vector2.up , Color.green);
        if (hit.collider != null)
        {
 
            if(hit.collider.transform.tag == "Note")
            {

                print(hit.collider.name + hit.collider.transform.position);

                float distance = Mathf.Abs(hit.point.y + offset - transform.position.y);
                if (distance < 6)
                {
                    hit.transform.gameObject.SetActive(false);
                    return distance;
                }
                
            }
        }

        return -1;
    }

}
