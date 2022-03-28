using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using System;

public enum BS
{
    START,
    PLAYERTURN,
    PLAYERTURN_ATTACK_LOCK,
    ENEMYTURN,
    WON,
    LOST
}

public class BattleSys : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject attackMenuButton;
    public GameObject inventoryMenuButton;
    public GameObject attackButton1;
    public GameObject attackButton2;
    public GameObject attackButton3;
    public GameObject attackButton4;

    Unit playerUnit;
    EnemyUnit enemyUnit;
    string attack1 = "";
    string attack2 = "";

    public Text dialogText;

    public UpdateBattleHUD HUD;

    public BS state;
    Random rnd = new Random();
    private bool successiveDodge;

    // Start is called before the first frame update
    void Start()
    {
        state = BS.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // gets referance to the player and enemy game objects
        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<EnemyUnit>();

        // writes text to the dialg box
        dialogText.text = "En vill " + enemyUnit.name + " dukket opp";

        // set information to HUD
        HUD.PlayerSetHUD(playerUnit);
        HUD.PlayerSetStress(playerUnit);
        HUD.EnemySetHUD(enemyUnit);
        HUD.EnemySetHP(enemyUnit);

        // finds names on the players attacks and labels buttons with them
        ClassAttack atk = new ClassAttack();
        var attackNames = atk.FindAttackName(playerUnit.classID);
        attack1 = attackNames.attackName1;
        attack2 = attackNames.attackName2;

        attackButton1.GetComponentInChildren<Text>().text = attack1;
        attackButton2.GetComponentInChildren<Text>().text = attack2;

        yield return new WaitForSeconds(3f);

        // checks who starts the battle
        if (PlayerStarts(playerUnit, enemyUnit))
        {
            dialogText.text = "Du angriper først";
            yield return new WaitForSeconds(1f);
            state = BS.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        else
        {
            dialogText.text = enemyUnit.name + " angriper først";
            yield return new WaitForSeconds(1f);
            state = BS.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack(int attackNr)
    {
        attackButton1.GetComponent<Button>().interactable = false;
        attackButton2.GetComponent<Button>().interactable = false;
        attackButton3.GetComponent<Button>().interactable = false;
        attackButton4.GetComponent<Button>().interactable = false;

        var diceRoll = DiceRoll();
        // contains info about the attack/move
        var attackInfo = playerUnit.UseClassAttack1(diceRoll.isCritical);

        if (attackNr == 1)
        {
            attackInfo = playerUnit.UseClassAttack1(diceRoll.isCritical);
            dialogText.text = "Du angriper " + enemyUnit.name + " med " + attack1;
        }
        if (attackNr == 2)
        {
            attackInfo = playerUnit.UseClassAttack2(diceRoll.isCritical);
            dialogText.text = "Du angriper " + enemyUnit.name + " med " + attack2;
        }

        yield return new WaitForSeconds(3f);

        // check if attacker hits or not
        if (Dodge(attackInfo.hitModifier))
        {
            dialogText.text = enemyUnit.name + " dukket unna angrepet";
            yield return new WaitForSeconds(3f);
        }
        else
        {
            float totalDamage = (float)Math.Round(attackInfo.dmg * diceRoll.modifier, 2);

            if (enemyUnit.protection > 0)
            {
                enemyUnit.protection -= (int)totalDamage;
                if (enemyUnit.protection < 0)
                {
                    enemyUnit.protection = 0;
                }

                // update dialog box with info about protection
            }
            else
            {
                enemyUnit.TakeDamage(totalDamage);
            }

            HUD.EnemySetHP(enemyUnit);

            if (diceRoll.isCritical)
            {
                dialogText.text = "Kritisk-treff på " + enemyUnit.name;
                yield return new WaitForSeconds(3f);
            }
            else
            {
                dialogText.text = "Du traff " + enemyUnit.name;
                yield return new WaitForSeconds(3f);

                if (totalDamage == 0)
                {
                    dialogText.text = "Men det gjorde ingen skade";
                    yield return new WaitForSeconds(3f);
                }
            }

            // if enemy is sleeping, a hit has a chance to wake them up
            if (enemyUnit.sleep > 0 && totalDamage > 3)
            {
                int random = rnd.Next(1, 11); // 1-10

                switch (enemyUnit.sleep)
                {
                    case 1:
                        // 50% chance to wake up
                        if (random % 2 == 0)
                        {
                            enemyUnit.sleep = 0;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = enemyUnit.name + " våknet";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    case 2:
                        // 40% chance to decrease sleep round by 1
                        if (random < 5)
                        {
                            enemyUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = enemyUnit.name + " vrir seg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    case 3:
                        // 30% chance to decrease sleep round by 1
                        if (random < 4)
                        {
                            enemyUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = enemyUnit.name + " vrir seg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    default:
                        // 20% chance to decrease sleep round by 1
                        if (random < 3)
                        {
                            enemyUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = enemyUnit.name + " vrir seg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;
                }
            }
            yield return new WaitForSeconds(3f);

            if (attackInfo.stun > enemyUnit.stun)
            {
                // if enemy already is stun
                if (enemyUnit.stun > 0)
                {
                    dialogText.text = enemyUnit.name + " er allerede paralysert";
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    enemyUnit.stun = attackInfo.stun;
                    dialogText.text = enemyUnit.name + " ble paralysert og kan ikke bevege seg";
                    yield return new WaitForSeconds(3f);
                }
            }
            if (attackInfo.sleep > enemyUnit.sleep)
            {
                enemyUnit.sleep = attackInfo.sleep;
                dialogText.text = enemyUnit.name + " sovnet";
                yield return new WaitForSeconds(3f);
            }
            if (attackInfo.poison > enemyUnit.poison)
            {
                enemyUnit.poison = attackInfo.poison;
                dialogText.text = enemyUnit.name + " ble forgiftet";
                yield return new WaitForSeconds(3f);
            }
            if (attackInfo.protection > playerUnit.protection)
            {
                playerUnit.protection = attackInfo.protection;
                dialogText.text = "Du beskyttet deg selv";
                yield return new WaitForSeconds(3f);
            }
        }

        // end off attack, check if enemy is dead
        if (enemyUnit.currentHP <= 0)
        {
            state = BS.WON;
            EndBattle();
        }
        else
        {
            state = BS.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyAttack(int attackNr)
    {
        EnemyAttack ea = new EnemyAttack();
        var names = ea.FindAttackName(enemyUnit.enemyID);

        var diceRoll = DiceRoll();
        // contains info about the attack/move
        var attackInfo = enemyUnit.UseEnemyAttack1(diceRoll.isCritical);

        // checks what attack the enemy uses
        if (attackNr == 1)
        {
            attackInfo = enemyUnit.UseEnemyAttack1(diceRoll.isCritical);
            dialogText.text = enemyUnit.name + " angriper deg med " + names.attackName1;
        }
        /* if (attackNr == 2)
        {
            attackInfo = enemyUnit.UseEnemyAttack2(diceRoll.isCritical);
            dialogText.text = enemyUnit.name + " angriper deg med " + attack2;
        }*/

        yield return new WaitForSeconds(3f);

        if (Dodge(attackInfo.hitModifier))
        {
            dialogText.text = enemyUnit.name + " dukket unna angrepet";
            yield return new WaitForSeconds(3f);
        }
        else
        {
            float totalDamage = (float)Math.Round(attackInfo.dmg * diceRoll.modifier, 2);
            if (playerUnit.protection > 0)
            {
                playerUnit.protection -= (int)totalDamage;
                if (playerUnit.protection < 0)
                {
                    playerUnit.protection = 0;
                }

                // TODO update dialog box with info about protection
            }
            else
            {
                playerUnit.TakeStress(totalDamage);
            }

            HUD.PlayerSetStress(playerUnit);

            if (diceRoll.isCritical)
            {
                dialogText.text = enemyUnit.name + " fikk et kritisk-treff på deg";
            }
            else
            {
                dialogText.text = enemyUnit.name + " traff deg";
            }

            // if enemy is sleeping, a hit has a chance to wake them up
            if (playerUnit.sleep > 0 && totalDamage < 3)
            {
                int random = rnd.Next(1, 11); // 1-10

                switch (playerUnit.sleep)
                {
                    case 1:
                        // 50% chance to wake up
                        if (random % 2 == 0)
                        {
                            playerUnit.sleep = 0;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = "Du våknet";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    case 2:
                        // 40% chance to decrease sleep round by 1
                        if (random < 5)
                        {
                            playerUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = "Du vrir deg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    case 3:
                        // 30% chance to decrease sleep round by 1
                        if (random < 4)
                        {
                            playerUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = "Du vrir deg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;

                    default:
                        // 20% chance to decrease sleep round by 1
                        if (random < 3)
                        {
                            playerUnit.sleep -= 1;
                            yield return new WaitForSeconds(3f);
                            dialogText.text = "Du vrir deg i søvne";
                            yield return new WaitForSeconds(3f);
                        }
                        break;
                }
            }
            yield return new WaitForSeconds(3f);

            if (attackInfo.stun > playerUnit.stun)
            {
                if (playerUnit.stun > 0)
                {
                    dialogText.text = "Du er allerede paralysert";
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    playerUnit.stun = attackInfo.stun;
                    dialogText.text = "Du ble paralysert og kan ikke bevege deg";
                    yield return new WaitForSeconds(3f);
                }
            }
            if (attackInfo.sleep > playerUnit.sleep)
            {
                playerUnit.sleep = attackInfo.sleep;
                dialogText.text = "Du sovnet";
                yield return new WaitForSeconds(3f);
            }
            if (attackInfo.poison > playerUnit.poison)
            {
                playerUnit.poison = attackInfo.poison;
                dialogText.text = playerUnit.name + " ble forgiftet";
                yield return new WaitForSeconds(3f);
            }
            if (attackInfo.protection > enemyUnit.protection)
            {
                enemyUnit.protection = attackInfo.protection;
                dialogText.text = enemyUnit.name + " beskyttet seg selv";
                yield return new WaitForSeconds(3f);
            }
        }

        // end off attack, check if player is dead
        if (playerUnit.currentStress >= playerUnit.maxStress)
        {
            state = BS.LOST;
            EndBattle();
        }
        else
        {
            state = BS.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        if (enemyUnit.poison > 0)
        {
            dialogText.text = enemyUnit.name + " er forgiftet";
            yield return new WaitForSeconds(3f);
            enemyUnit.TakeDamage(enemyUnit.maxHP / 16);

            HUD.EnemySetHP(enemyUnit);
            dialogText.text = enemyUnit.name + " ble skadet av giften";
            yield return new WaitForSeconds(3f);
            enemyUnit.poison -= 1;

            // check if enemy is dead by poison
            if (enemyUnit.currentHP <= 0)
            {
                state = BS.WON;
                EndBattle();
            }
        }

        if (enemyUnit.stun > 0)
        {
            enemyUnit.stun -= 1;

            if (enemyUnit.stun == 0)
            {
                dialogText.text = enemyUnit.name + " er ikke lengre paralysert";
                yield return new WaitForSeconds(3f);
            }
        }

        if (enemyUnit.sleep > 0)
        {
            enemyUnit.sleep -= 1;

            if (enemyUnit.sleep == 0)
            {
                dialogText.text = enemyUnit.name + " våknet";
                yield return new WaitForSeconds(3f);
            }
        }

        if (enemyUnit.stun > 0 || enemyUnit.sleep > 0)
        {
            if (enemyUnit.stun > 0)
            {
                dialogText.text = enemyUnit.name + " er paralysert og kan ikke angripe";
                yield return new WaitForSeconds(3f);

                state = BS.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
            if (enemyUnit.sleep > 0)
            {
                dialogText.text = enemyUnit.name + " sover og kan ikke angripe";
                yield return new WaitForSeconds(3f);

                state = BS.PLAYERTURN;
                StartCoroutine(PlayerTurn());
            }
        }
        else
        {
            // TODO make enemy choose attack
            StartCoroutine(EnemyAttack(1));
        }
    }

    IEnumerator PlayerTurn()
    {
        if (playerUnit.poison > 0)
        {
            dialogText.text = "Du er forgiftet";
            yield return new WaitForSeconds(3f);
            playerUnit.TakeStress(playerUnit.maxStress / 16);

            HUD.PlayerSetStress(playerUnit);
            playerUnit.poison -= 1;
        }

        if (playerUnit.stun > 0)
        {
            playerUnit.stun -= 1;

            if (playerUnit.stun == 0)
            {
                dialogText.text = "Du har ikke lengre hjernteppe";
                yield return new WaitForSeconds(3f);
            }
        }

        if (playerUnit.sleep > 0)
        {
            playerUnit.sleep -= 1;

            if (playerUnit.sleep == 0)
            {
                dialogText.text = "Du våknet";
                yield return new WaitForSeconds(3f);
            }
        }

        if (playerUnit.stun > 0 || playerUnit.sleep > 0)
        {
            inventoryMenuButton.SetActive(true);

            dialogText.text = "Velg hva du vil gjøre:";
            yield return new WaitForSeconds(3f);
        }
        else
        {
            dialogText.text = "Velg hva du vil gjøre:";
            attackMenuButton.SetActive(true);
            inventoryMenuButton.SetActive(true);

            attackButton1.GetComponent<Button>().interactable = true;
            attackButton2.GetComponent<Button>().interactable = true;

            //if(WeaponIsEquiped)
            attackButton3.GetComponent<Button>().interactable = true;
            attackButton4.GetComponent<Button>().interactable = true;
        }
    }

    public bool PlayerStarts(Unit player, EnemyUnit enemy)
    {
        Random rnd = new Random();
        int playerRoll = rnd.Next(21); // random number 0 - 20
        int enemyRoll = rnd.Next(21); // random number 0 - 20

        playerRoll += player.level;
        enemyRoll += enemy.level;

        Debug.Log("initiative: " + playerRoll + " " + enemyRoll);

        if (playerRoll >= enemyRoll)
        {
            return true;
        }
        return false;
    }

    public void onAttackButton1()
    {
        if (state != BS.PLAYERTURN)
            return;

        int attackNr = 1;

        StartCoroutine(PlayerAttack(attackNr));
    }

    public void onAttackButton2()
    {
        if (state != BS.PLAYERTURN)
            return;

        int attackNr = 2;

        StartCoroutine(PlayerAttack(attackNr));
    }

    public void onAttackButton3()
    {
        if (state != BS.PLAYERTURN)
            return;

        int attackNr = 2;

        StartCoroutine(PlayerAttack(attackNr));
    }

    public void onAttackButton4()
    {
        if (state != BS.PLAYERTURN)
            return;

        int attackNr = 2;

        StartCoroutine(PlayerAttack(attackNr));
    }

    public void onItemUse(){

    }

    private (float modifier, bool isCritical) DiceRoll()
    {
        float modifier = 1.0f;
        bool isCritical = false;

        // 1:16 chance at critical damage (50% more damage)
        // if not critical modifier is from 0.85 to 1.00
        int roll = rnd.Next(17);
        if (roll < 16)
        {
            float rollDecimal = roll / 100.0f;

            modifier -= rollDecimal;
        }
        else
        {
            Debug.Log("critical");
            modifier = 1.5f;
            isCritical = true;
        }
        // also returns bool isCritical
        // to be used to set base damage to max
        return (modifier, isCritical);
    }

    private bool Dodge(int hitModifier)
    {
        bool dodgeAttack = false;
        if (state == BS.PLAYERTURN)
        {
            playerUnit.CalculateHitScore();

            if (enemyUnit.stun > 0 || enemyUnit.sleep > 0)
            {
                dodgeAttack = false;
            }
            else if (playerUnit.hitScore + hitModifier < enemyUnit.dodgeScore + enemyUnit.level)
            {
                dodgeAttack = true;
            }
            else
            {
                dodgeAttack = false;
            }
        }
        if (state == BS.ENEMYTURN)
        {
            enemyUnit.CalculateHitScore();

            if (playerUnit.stun > 0 || playerUnit.sleep > 0)
            {
                dodgeAttack = false;
            }
            else if (enemyUnit.hitScore + hitModifier < playerUnit.dodgeScore + playerUnit.level)
            {
                dodgeAttack = true;
            }
            else
            {
                dodgeAttack = false;
            }
        }

        return dodgeAttack;
    }

    void EndBattle()
    {
        if (state == BS.WON)
        {
            dialogText.text = "Du vant!";
        }
        else if (state == BS.LOST)
        {
            dialogText.text = "Du tapte!";
        }
    }
}
