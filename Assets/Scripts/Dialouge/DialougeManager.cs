using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum DialogState
{
    Ready,
    Busy,
};

public class DialougeManager : MonoBehaviour
{
    public DialogState state;
    public event Action onDialogClosed;
    protected bool dialougeStarted;

    [SerializeField]
    TMP_Text textLabel;

    [SerializeField]
    GameObject dialogBox;

    private DialougetypingEffect dialougetypingEffect;
    public ResponseHandler responseHandler; 

    public bool IsOpen { get; private set; }


    private void Start()
    {
        dialougetypingEffect = GetComponent<DialougetypingEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialougeBox();
    }

    public void ShowDialouge(DialougeObject dialougeObject)
    {
        IsOpen = true; 
        dialogBox.SetActive(true);
        StartCoroutine(StepThroughDialouge(dialougeObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialouge(DialougeObject dialougeObject)
    {
        for (int i = 0; i < dialougeObject.Dialouge.Length; i++)
        {
            string dialouge = dialougeObject.Dialouge[i];

            yield return RunTypingEffect(dialouge);

            textLabel.text = dialouge; 

            if (i == dialougeObject.Dialouge.Length - 1 && dialougeObject.HasResponses) break;

            yield return null; 
            yield return new WaitUntil(() => !responseHandler.waitingForResponse && state == DialogState.Ready);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        if (dialougeObject.HasResponses)
        {
            responseHandler.ShowResponses(dialougeObject.Responses);
        }
        else 
        {
            CloseDialougeBox(); 
        }
    }

    private IEnumerator RunTypingEffect(string dialouge)
    {
        dialougetypingEffect.Run(dialouge, textLabel);
        state = DialogState.Busy;
        while (dialougetypingEffect.IsRunning)
        {
            yield return null;
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                dialougetypingEffect.Stop();
            }
        }
        state = DialogState.Ready;
    }

    public void CloseDialougeBox()
    {
        onDialogClosed?.Invoke();
        IsOpen = false; 
        dialogBox.SetActive(false);
        textLabel.text = string.Empty;
    }


    /*
    void Update()
    {
        if (dialougeStarted == true && Input.GetKeyDown(KeyCode.Z))
        {                 
            Debug.Log("Z virke sjef");
            dialougeStarted = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dialougeStarted = true;
        Debug.Log("kollisjon virke sjef");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        dialougeStarted = false;
        Debug.Log("BACKSTEP virke sjef");
    }*/
}
