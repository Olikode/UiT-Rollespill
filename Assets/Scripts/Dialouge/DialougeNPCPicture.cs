using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeNPCPicture : MonoBehaviour
{
    [SerializeField] private SpriteRenderer nPCPlaceHolder;
    [SerializeField] private DialougeActivator dialougeActivator;
    void Start()
    {
        nPCPlaceHolder = GetComponent<SpriteRenderer>();
    }
}
