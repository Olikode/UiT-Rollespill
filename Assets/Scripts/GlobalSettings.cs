using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColor;
    [SerializeField] Color orginalColor;
    [SerializeField] MoveBase noPpMove;

    public Color HighlightedColor => highlightedColor;
    public Color OrginalColor => orginalColor;
    public MoveBase NoPpMove => noPpMove;

    public static GlobalSettings i {get; private set;}

    private void Awake()
    {
        i = this;
    }
}
