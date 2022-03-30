using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Unit> wildUnits;

    public Unit GetRandomWildUnit(){
        return wildUnits[Random.Range(0, wildUnits.Count)];
    }
}
