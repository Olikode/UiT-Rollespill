using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHud : MonoBehaviour
{
    [SerializeField]
    Text nameText;

    [SerializeField]
    Text lvlText;

    [SerializeField]
    Image conditionImage;

    [SerializeField]
    Sprite sleepIcon;

    [SerializeField]
    Sprite poisonIcon;

    [SerializeField]
    Sprite stunIcon;

    [SerializeField]
    Color iconColor;

    [SerializeField]
    Color noIconColor;

    [SerializeField]
    HPBar hpBar;

    [SerializeField]
    GameObject expBar;

    Unit _unit;

    public void SetData(Unit unit)
    {
        _unit = unit;

        nameText.text = unit.Base.Name;
        SetLevel();
        hpBar.SetHP((float)unit.HP / unit.MaxHP);
        SetExp();

        SetStatusIcon();
        unit.OnStatusChanged += SetStatusIcon;
        unit.OnHPChanged += UpdateHP;
    }

    void SetStatusIcon(){

        if(_unit.Status == null){
            conditionImage.sprite = null;
            conditionImage.color = noIconColor;
        }
        else if(_unit.Status.Name == "Gift"){
            conditionImage.sprite = poisonIcon;
            conditionImage.color = iconColor;
        }
        else if(_unit.Status.Name == "Søvn"){
            conditionImage.sprite = sleepIcon;
            conditionImage.color = iconColor;
        }
        else if(_unit.Status.Name == "Hjerneteppe"){
            conditionImage.sprite = stunIcon;
            conditionImage.color = iconColor;
        }
    }

    public void SetLevel(){
        lvlText.text = "Lvl " + _unit.Level;
    }

    public void SetExp()
    {
        if(expBar == null){
            return;
        }

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

        public IEnumerator SetExpSmooth(bool reset=false)
    {
        if(expBar == null)
            yield break;

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);


        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    float GetNormalizedExp(){
        int currentLevelExp = _unit.Base.GetExpForLevel(_unit.Level);
        int nextLevelExp = _unit.Base.GetExpForLevel(_unit.Level + 1);

        float normalizedExp = (float)(_unit.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_unit.HP / _unit.MaxHP);
    }

    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.isUpdating == false);
    }
}
