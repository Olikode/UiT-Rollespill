using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    

    [SerializeField] bool isPlayer;
    [SerializeField] BattleHud hud;

    public bool IsPlayer{
        get{return isPlayer;}
    }

    public BattleHud Hud {
        get{return hud;}
    }

    public Unit Unit { get; set; }

    Image image;
    Color orginalColor;
    Vector3 orginalPos;

    private void Awake(){
        image = GetComponent<Image>();
        orginalColor = image.color;
        orginalPos = image.transform.localPosition;
    }

    public void Setup(Unit unit)
    {
        Unit = unit;
        image.sprite = Unit.Base.Sprite;
        hud.SetData(unit);

        image.color = orginalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation(){
        if(isPlayer)
            image.transform.localPosition = new Vector3(-500f, orginalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orginalPos.y);

        image.transform.DOLocalMoveX(orginalPos.x, 2f);
    }

    public void PlayAttackAnimation(){
        var sequence = DOTween.Sequence();
        if(isPlayer)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void PlayHitAnimation(){
        var sequence = DOTween.Sequence();

        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(orginalColor, 0.1f));
    }

    public void PlayDieAnimation(){
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
