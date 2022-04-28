using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Este script se encarga de los avisos en la pantalla de que se acerca la nota
public class NoteAlert : MonoBehaviour
{
    float detectDistance = 5;
    float offset = 0f;

    SpriteRenderer sprite;

    int countFrames = 0;
    int maxCountFrames = 2;

    // Inicializamos
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Comprobamos cada dos frames si viene la nota. De la misma manera activamos y desactivamos el sprite
    void Update()
    {
        countFrames++;

        if(countFrames >= maxCountFrames)
        {
            countFrames = 0;
            if (!detectNoteAlert())
            {
                sprite.enabled = false;
            }
            else
            {
                sprite.enabled = true;
            }
        }
    }
    //Si detectamos la nota y está a cierta distancia devuelve verdadero
    public bool detectNoteAlert() {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);
        Debug.DrawRay(transform.position, Vector2.up, Color.green);

        if (hit.collider != null)
        {

            if (hit.collider.transform.tag == "Note")
            {

                float distance = Mathf.Abs(hit.point.y + offset - transform.position.y);
                if (distance < detectDistance)
                {
                    return true;         
                }
            }
        }
        return false;

    }

}
