using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random=System.Random;
using System;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject attackMenuButton;
    public GameObject inventoryMenuButton;
    public GameObject attackButton1;
    public GameObject attackButton2;
    public GameObject attackButton3;
    public GameObject attackButton4;

    TestUnit playerUnit;
    TestUnit enemyUnit;
    string attack1 = "";
    string attack2 = "";    

    public Text dialogText;
    public Text playerHealth;
    public Text enemyHealth;
    public UpdateBattleHUD playerHUD;
    public UpdateBattleHUD enemyHUD;


    public BattleState state;
    Random rnd = new Random();
    private bool successiveDodge;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());

        SetupBattle();
    }

    IEnumerator SetupBattle(){
        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<TestUnit>();

        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<TestUnit>();


        dialogText.text = "En vill " + enemyUnit.name + " dukket opp";

        playerHUD.PlayerSetHUD(playerUnit);
        enemyHUD.EnemySetHUD(enemyUnit);

        var attackNames = playerUnit.FindAttackName(playerUnit.classID);
        attack1 = attackNames.attackName1;
        attack2 = attackNames.attackName2;

        attackButton1.GetComponentInChildren<Text>().text = attack1;
        attackButton2.GetComponentInChildren<Text>().text = attack2;

        yield return new WaitForSeconds(3f);

        if(PlayerStarts(playerUnit, enemyUnit)){
            dialogText.text = "Du angriper først";
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else{
            dialogText.text = enemyUnit.name + " angriper først";
            yield return new WaitForSeconds(1f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack(int attackNr){
        attackButton1.GetComponent<Button>().interactable = false;
        attackButton2.GetComponent<Button>().interactable = false;
        attackButton3.GetComponent<Button>().interactable = false;
        attackButton4.GetComponent<Button>().interactable = false;

        if(attackNr == 1){
        dialogText.text = "Du angriper " + enemyUnit.name + " med " + attack1;
        }
        if(attackNr == 2){
        dialogText.text = "Du angriper " + enemyUnit.name + " med " + attack2;
        }

        yield return new WaitForSeconds(3f);

        var diceRoll = DiceRoll();
        var attackInfo = playerUnit.UseAbilityAttack1(diceRoll.isCritical);
        

        if(attackNr == 1){
        attackInfo = playerUnit.UseAbilityAttack1(diceRoll.isCritical);
        }
        if(attackNr == 2){
        attackInfo = playerUnit.UseAbilityAttack2(diceRoll.isCritical);
        }

        /*successiveDodge = Dodge(playerUnit, enemyUnit);
        if(successiveDodge){
            Debug.Log("dodgeAttack");
        }
        if(!successiveDodge){
            Debug.Log("not dodge");
        }*/

        if(Dodge(playerUnit, enemyUnit, attackInfo.hitModifier)){
            dialogText.text =enemyUnit.name + " dukket unna angrepet";
            yield return new WaitForSeconds(3f);
        }
        else{

            float totalDamage = (float)Math.Round(attackInfo.dmg*diceRoll.modifier, 2);

            bool isDead = enemyUnit.TakeDamage(totalDamage);

            enemyHUD.EnemySetHP(enemyUnit.currentHP);

        if(diceRoll.isCritical){
            dialogText.text = "Kritisk-treff på " + enemyUnit.name;
        }

        else{
            dialogText.text = "Du traff " + enemyUnit.name;
        }

        int healthPercentage = (int)((enemyUnit.currentHP / enemyUnit.maxHP)*100);
        if(healthPercentage < 0){
            healthPercentage = 0;
        }
        enemyHealth.text = healthPercentage + "%";

        if(enemyUnit.sleep > 0 && enemyUnit.sleep < 2){
            int wakeUpChance = rnd.Next(2); //0-1
            if(wakeUpChance > 0){
                yield return new WaitForSeconds(3f);
                dialogText.text = enemyUnit.name + "";
            }
        }

        yield return new WaitForSeconds(3f);

        if(attackInfo.stun > enemyUnit.stun){
            if(enemyUnit.stun > 0){
                dialogText.text = enemyUnit.name + " er allerede paralysert";
                yield return new WaitForSeconds(3f);
            }
            else{
                enemyUnit.stun = attackInfo.stun;
                dialogText.text = enemyUnit.name + " ble paralysert og kan ikke bevege seg";
                yield return new WaitForSeconds(3f);
            }
        }
        if(attackInfo.sleep > enemyUnit.sleep){
            enemyUnit.sleep = attackInfo.sleep;
            dialogText.text = enemyUnit.name + " sovnet";
            yield return new WaitForSeconds(3f);
        }
        if(attackInfo.poison > enemyUnit.poison){
            enemyUnit.poison = attackInfo.poison;
            dialogText.text = enemyUnit.name + " ble forgiftet";
            yield return new WaitForSeconds(3f);
        }
        if(attackInfo.protection > playerUnit.protection){
            playerUnit.protection = attackInfo.protection;
            dialogText.text = "Du beskyttet deg selv";
            yield return new WaitForSeconds(3f);
        }

        }

        if(enemyUnit.currentHP <= 0){
            state = BattleState.WON;
            EndBattle();
        } 
        else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn(){
        if(enemyUnit.stun == 0 && enemyUnit.sleep == 0 && enemyUnit.poison == 0){

            dialogText.text = enemyUnit.name + " angriper!";

            yield return new WaitForSeconds(3f);

            var diceRoll = DiceRoll();
            var attackInfo = enemyUnit.UseAbilityAttack1(diceRoll.isCritical);

            bool isDead = playerUnit.TakeDamage(attackInfo.dmg);

            playerHUD.PlayerSetHP(playerUnit.currentHP);

            dialogText.text = enemyUnit.name+ " traff deg.";

            int healthPercentage = (int)((playerUnit.currentHP / playerUnit.maxHP)*100);
            if(healthPercentage < 0){
                healthPercentage = 0;
            }
            playerHealth.text = healthPercentage + "%";

            yield return new WaitForSeconds(3f);
        }

        if(enemyUnit.stun > 0){
            dialogText.text = enemyUnit.name + " er paralysert";
            enemyUnit.stun -= 1;
            yield return new WaitForSeconds(3f);
        }
        if(enemyUnit.sleep > 0){
            dialogText.text = enemyUnit.name + " sover";
            enemyUnit.sleep -= 1;
            yield return new WaitForSeconds(3f);
        }
        if(enemyUnit.poison > 0){
            dialogText.text = enemyUnit.name + " er forgiftet";
            enemyUnit.TakeDamage(enemyUnit.maxHP/16);
            yield return new WaitForSeconds(3f);
            
            playerHUD.EnemySetHP(enemyUnit.currentHP);
            dialogText.text = enemyUnit.name + " ble skadet av giften";
            yield return new WaitForSeconds(3f);
        }

        

        if(playerUnit.currentHP == 0){
            state = BattleState.LOST;
            EndBattle();
        }
        if(enemyUnit.currentHP == 0){
            state = BattleState.WON;
            EndBattle();
        }
        else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle() {
        if(state == BattleState.WON){
            dialogText.text = "Du vant!";
        }
        else if(state == BattleState.LOST){
            dialogText.text = "Du tapte!";
        }
    }

    void PlayerTurn(){
        dialogText.text = "Velg hva du vil gjøre:";

        attackMenuButton.SetActive(true);
        inventoryMenuButton.SetActive(true);

        attackButton1.GetComponent<Button>().interactable = true;
        attackButton2.GetComponent<Button>().interactable = true;
        attackButton3.GetComponent<Button>().interactable = true;
        attackButton4.GetComponent<Button>().interactable = true;
    }

    public void onAttackButton1(){
        if (state != BattleState.PLAYERTURN)
            return;
        
        int attackNr = 1;

        StartCoroutine(PlayerAttack(attackNr));
    }

        public void onAttackButton2(){
        if (state != BattleState.PLAYERTURN)
            return;
        
        int attackNr = 2;

        StartCoroutine(PlayerAttack(attackNr));
    }

    private (float modifier, bool isCritical) DiceRoll(){
        
        float modifier = 1.0f;
        bool isCritical = false;

        // 1:16 chance at critical damage (50% more damage)
        // if not critical modifier is from 0.85 to 1.00
        int roll = rnd.Next(17);
        if(roll < 16){
            float rollDecimal = roll / 100.0f;

            modifier -= rollDecimal;
        }
        else{
            Debug.Log("critical");
            modifier = 1.5f;
            isCritical = true;
        }
        return (modifier, isCritical);
    }

    private bool Dodge(TestUnit attacker, TestUnit defender, int hitModifier){

        bool dodgeAttack;
        attacker.CalculateHitScore();
        //defender.CalculateDodgeScore();

        if(attacker.hitScore + hitModifier < defender.dodgeScore){
            dodgeAttack = true;
        }
        else{
            dodgeAttack = false;
        }
        Debug.Log("Dodge: " + (attacker.hitScore+hitModifier) + " " + defender.dodgeScore );
        return dodgeAttack;
    }

    
    public bool PlayerStarts(TestUnit player, TestUnit enemy){
        Random rnd = new Random();
        int playerRoll = rnd.Next(21); // random number 0 - 20
        int enemyRoll = rnd.Next(21); // random number 0 - 20

        playerRoll += player.level /* + modifier */;
        enemyRoll += enemy.level /* + modifier */;

        Debug.Log("initiative: " + playerRoll + " " + enemyRoll );

        if(playerRoll >= enemyRoll){
            return true;
        }
        return false;
    }

}
