using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color orginalColor;

    public Color HighlightedColor => highlightedColor;
    public Color OrginalColor => orginalColor;

    public static GlobalSettings i {get; private set;}

    private void Awake()
    {
        i = this;
    }
}
