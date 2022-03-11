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

    public void EnemySetHUD(TestUnit unit){
        enemyName.text = unit.name;
        enemyLevel.text = "Lvl " + unit.level;
        enemyHealthBar.SetMaxHealth(unit.maxHP);
        enemyHealthBar.SetHealth(unit.currentHP);
    }

    public void EnemySetHP(float hp){
        enemyHealthBar.SetHealth(hp);
    }

     public void PlayerSetHUD(TestUnit unit){
        playerName.text = unit.name;
        playerLevel.text = "Lvl " + unit.level;
        playerHealthBar.SetMaxHealth(unit.maxHP);
        playerHealthBar.SetHealth(unit.currentHP);
    }

    public void PlayerSetHP(float hp){
        playerHealthBar.SetHealth(hp);
    }
}
