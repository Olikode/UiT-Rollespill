using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenger : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] string prefix;
    [SerializeField] Sprite sprite;

    public string Name{
        get {return name;}
    }

    public string Prefix{
        get {return prefix;}
    }

    public Sprite Sprite{
        get {return sprite;}
    }
}
