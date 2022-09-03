using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //private Button btn;
    private Text text;
    public Color normalColor;
    public Color hoverColor;

    void Awake()
    {
        //btn = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        text.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }

}
