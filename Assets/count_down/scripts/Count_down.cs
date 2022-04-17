using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Count_down : MonoBehaviour
{
    //event
    public event Handler handler;

    [Header("count down image")]
    public Sprite sprite_three;
    public Sprite sprite_two;
    public Sprite sprite_one;

    [Header("audio clip")]
    public AudioClip audio_clip_number;
    public AudioClip audio_clip_over;

    [Header("animation type")]
    public Animation_type animation_type;

    //image
    private Image image;

    //animator controller
    private Animator animator;

    //audio source
    private AudioSource audio_source;

    // Use this for initialization
    void Start()
    {
        //initialize animator and audioSource
        this.image = this.GetComponent<Image>();
        this.animator = this.GetComponent<Animator>();
        this.audio_source = this.GetComponent<AudioSource>();
    }

    //start count down
    public void start_count_down()
    {
        //this.image.color = new Color(255, 255, 255, 255);

        //this.animator.SetTrigger("zoom_triger");
        switch (this.animation_type)
        {
            case Animation_type.fade:
                this.animator.SetTrigger("fade_triger");
                break;
            case Animation_type.zoom:
                this.animator.SetTrigger("zoom_triger");
                break;
            default:
                break;
        }
    }

    //number 3 animation start event
    public void number_3_start()
    {
        //change the texture
        this.image.sprite = this.sprite_three;

        //play the audio
        this.audio_source.PlayOneShot(this.audio_clip_number);

    }

    //number 2 animation start event
    public void number_2_start()
    {
        //change the texture
        this.image.sprite = this.sprite_two;

        //play the audio
        this.audio_source.PlayOneShot(this.audio_clip_number);

    }

    //number 1 animation start event
    public void number_1_start()
    {
        //change the texture
        this.image.sprite = this.sprite_one;

        //play the audio
        this.audio_source.PlayOneShot(this.audio_clip_number);
    }


    //number 1 animation over event
    public void number_1_over()
    {
        //play the audio
        this.audio_source.PlayOneShot(this.audio_clip_over);

        //send the event
        this.handler();
    }
}

//declare delegate
public delegate void Handler();

//animation type enum
public enum Animation_type
{
    fade,
    zoom
}



