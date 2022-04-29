using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveInfoUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text ppText;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetInfo(string name, string pp)
    {
        nameText.text = name;
        ppText.text = pp;
    }

    public void ShowSelected(Color color)
    {
        nameText.color = color;
        ppText.color = color;
    }
}
