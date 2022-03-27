using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpdateBattleHUD : MonoBehaviour
{
    
    public Text enemyName;
    public Text playerName;

    public Text enemyLevel;
    public Text playerLevel;

    public Text stressField;
    public Text healthField;

    public HealthBar enemyHealthBar;
    public HealthBar playerHealthBar;

    public void EnemySetHUD(EnemyUnit unit){
        enemyName.text = unit.name;
        enemyLevel.text = "Lvl " + unit.level;
        enemyHealthBar.SetMaxHealth(unit.maxHP);
        enemyHealthBar.SetHealth(unit.currentHP);
    }

    public void EnemySetHP(EnemyUnit unit){
        enemyHealthBar.SetHealth(unit.currentHP);

        float healthPercentage = ((unit.currentHP / unit.maxHP) * 100);
            if (healthPercentage < 0)
            {
                healthPercentage = 0;
            }
            healthField.text = Math.Ceiling(healthPercentage) + "%";
    }

     public void PlayerSetHUD(Unit unit){
        playerName.text = unit.name;
        playerLevel.text = "Lvl " + unit.level;
        playerHealthBar.SetMaxHealth(unit.maxStress);
        playerHealthBar.SetHealth(unit.currentStress);
    }

    public void PlayerSetStress(Unit unit){
        playerHealthBar.SetHealth(unit.currentStress);

        float healthPercentage = (((unit.currentStress / unit.maxStress) * 100));
            if (healthPercentage > 100)
            {
                healthPercentage = 100;
            }
            stressField.text = Math.Ceiling(healthPercentage) + "%";
    }
}
