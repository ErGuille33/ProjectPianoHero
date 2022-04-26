using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Haciendo uso de rayCast, detectar la distancia de la nota más cercana al punto de detección
public class Detector : MonoBehaviour
{
    float offset = 0f;
    Note actualNote;

    //Detectar nota al pulsar la tecla
    public bool detectNotePushDistance()
    {
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, Vector2.down);
        
        //Miramos notas por abajo del detector por si queda rezagada
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if (hit0.collider != null)
        {

            if (hit0.collider.transform.tag == "Note")
            {

                float distance = Mathf.Abs(hit0.point.y + offset - transform.position.y);
                if (distance < 1)
                {
                    actualNote = hit0.collider.transform.GetComponent<Note>();

                    if (actualNote != null)
                    {
                        actualNote.setPushPointHit(transform.position.x, transform.position.y,distance);
                        return true;
                    }
                 
                }
            }
        }

        //Miramos por arriba
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        Debug.DrawRay(transform.position, Vector2.up , Color.green);

        if (hit.collider != null)
        {
 
            if(hit.collider.transform.tag == "Note")
            {

                float distance = Mathf.Abs(hit.point.y + offset - transform.position.y);
                if (distance < 2)
                {
                    actualNote = hit.collider.transform.GetComponent<Note>();

                    if (actualNote != null)
                    {
                        actualNote.setPushPointHit(transform.position.x, transform.position.y, distance);
                        return true;
                    }
                }  
            }
        }
        return false;
    }
    //Detectamos nota al liberar la nota, utilizado para las notas largas
    public bool detectNoteReleaseDistance()
    {
        RaycastHit2D hit0 = Physics2D.Raycast(transform.position, Vector2.down);
        //Por abajo
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if (hit0.collider != null)
        {
           
            if (hit0.collider.transform.tag == "Note")
            {

                float distance = Mathf.Abs(hit0.point.y + offset - transform.position.y);
                if (distance < 1)
                {
                    actualNote = hit0.collider.transform.GetComponent<Note>();

                    if (actualNote != null)
                    {
                        actualNote.setReleasePointHit(hit0.point.x, hit0.point.y);
                        return true;
                    }

                }
            }
        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        Debug.DrawRay(transform.position, Vector2.up, Color.green);
        //Por arriba
        if (hit.collider != null)
        {

            if (hit.collider.transform.tag == "Note")
            {

                float distance = Mathf.Abs(hit.point.y + offset - transform.position.y);
                if (distance < 2)
                {
                    actualNote = hit.collider.transform.GetComponent<Note>();

                    if (actualNote != null)
                    {
                        actualNote.setReleasePointHit(hit0.point.x, hit0.point.y);
                        return true;
                    }
                }
            }
        }
        return false;
    }



}
