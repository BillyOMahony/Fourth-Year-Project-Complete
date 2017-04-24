using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, ISelectHandler, IPointerEnterHandler, ISubmitHandler
{
    public AudioSource Audio;
    
    // Use this for initialization
    void Start () {
        Audio = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Audio != null)
        {
            Audio.Play();
        }
    }

    public void OnSelect(BaseEventData EventData)
    {
        if (Audio != null)
        {
            Audio.Play();
        }
    }

    public void OnSubmit(BaseEventData EventData)
    {
        GameObject.Find("SubmitAudio(Clone)").GetComponent<AudioSource>().Play();
    }
}
