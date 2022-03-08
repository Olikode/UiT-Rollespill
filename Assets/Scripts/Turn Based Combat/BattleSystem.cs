using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject attackButton;
    public GameObject inventoryButton;

    Unit playerUnit;
    Unit enemyUnit;

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
        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<Unit>();


        dialogText.text = "En vill " + enemyUnit.unitName + " dukket opp";

        playerHUD.PlayerSetHUD(playerUnit);
        enemyHUD.EnemySetHUD(enemyUnit);

        yield return new WaitForSeconds(3f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack(){

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.EnemySetHP(enemyUnit.currentHP);
        dialogText.text = "Du traff " + enemyUnit.unitName;

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
        dialogText.text = enemyUnit.unitName + " angriper!";

        yield return new WaitForSeconds(2f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.PlayerSetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(2f);

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

        attackButton.SetActive(true);
        inventoryButton.SetActive(true);
    }

    public void onAttackButton(){
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }
}
