using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] Unit unit;

    public Unit GetEnemyUnit(){
        return unit;
    }

}
