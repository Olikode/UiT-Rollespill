using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBattleHUD : MonoBehaviour
{
    
    public Text enemyName;
    public Text playerName;

    public Text enemyLevel;
    public Text playerLevel;

    public HealthBar enemyHealthBar;
    public HealthBar playerHealthBar;

    public void EnemySetHUD(Unit unit){
        enemyName.text = unit.unitName;
        enemyLevel.text = "Lvl " + unit.unitLevel;
        enemyHealthBar.SetMaxHealth(unit.maxHP);
        enemyHealthBar.SetHealth(unit.currentHP);
    }

    /*public void EnemySetHP(int hp){
        enemyHealthBar.value = hp;
    }*/

     public void PlayerSetHUD(Unit unit){
        playerName.text = unit.unitName;
        playerLevel.text = "Lvl " + unit.unitLevel;
        playerHealthBar.SetMaxHealth(unit.maxHP);
        playerHealthBar.SetHealth(unit.currentHP);
    }

   /* public void PlayerSetHP(int hp){
        playerHealthBar.value = hp;
    }*/
}
