using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public UpdateBattleHUD playerHUD;
    public UpdateBattleHUD enemyHUD;


    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());

        SetupBattle();
    }

    IEnumerator SetupBattle(){
        //check what class player is, create object based on class (sykepleier, datatek., bygg...)

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

        state = BattleState.PLAYERTURN;
        PlayerTurn();
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

        var attackInfo = playerUnit.UseAbilityAttack1();

        if(attackNr == 1){
        attackInfo = playerUnit.UseAbilityAttack1();
        }
        if(attackNr == 2){
        attackInfo = playerUnit.UseAbilityAttack2();
        }

        bool isDead = enemyUnit.TakeDamage(attackInfo.dmg);

        enemyHUD.EnemySetHP(enemyUnit.currentHP);
        dialogText.text = "Du traff " + enemyUnit.name + ".\nSkade: " + attackInfo.dmg;

        yield return new WaitForSeconds(3f);

        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } 
        else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn(){
        dialogText.text = enemyUnit.name + " angriper!";

        yield return new WaitForSeconds(3f);

        var attackInfo = enemyUnit.UseAbilityAttack1();


        bool isDead = playerUnit.TakeDamage(attackInfo.dmg);

        playerHUD.PlayerSetHP(playerUnit.currentHP);

        dialogText.text = enemyUnit.name+ " traff deg." + ".\nSkade: " + attackInfo.dmg;

        yield return new WaitForSeconds(3f);

        if(isDead){
            state = BattleState.LOST;
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
}
