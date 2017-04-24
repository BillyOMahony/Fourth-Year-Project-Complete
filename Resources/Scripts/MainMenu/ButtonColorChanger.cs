using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonColorChanger : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {

    public Color highlightedColor;
    public Color standardColor;

    Text text;

	// Use this for initialization
	void Start () {
        text = transform.GetChild(0).GetComponent<Text>();
	}
	
    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightedColor;
    } 

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = standardColor;
    }

    public void OnSelect(BaseEventData EventData)
    {
        text.color = highlightedColor;
    }

    public void OnDeselect(BaseEventData EventData)
    {
        text.color = standardColor;
    }
}
