using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    void Start()
    {
        
    }


    public void SetupBattle(){
        playerUnit.Setup();
        playerHud.SetData(playerUnit.unit);
    }
}
