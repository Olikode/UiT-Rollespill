using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    int currentSelection = 0;
    public bool waitingForResponse = false;
    Response[] allResponses;

    private DialougeManager dialougeManager;
    private ResponseEvent[] responseEvents; 

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialougeManager = GetComponent<DialougeManager>(); 
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents; 
    }

    public void ShowResponses(Response[] responses)
    {
        waitingForResponse = true;
        float responseBoxHeigth = 0;
        allResponses = responses;

        for (int i = 0; i < responses.Length; i++) 
        {
            Response response = responses[i];
            int responseIndex = i; 

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => waitingForResponse = false);
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));
            
            tempResponseButtons.Add(responseButton);

            responseBoxHeigth += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeigth);
        responseBox.gameObject.SetActive(true);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++ currentSelection;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            -- currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, tempResponseButtons.Count-1);

        UpdateResponseSelection(currentSelection);

        if(dialougeManager.state == DialogState.Ready)
        {
            if(Input.GetKeyDown(KeyCode.Return) && tempResponseButtons.Count > 0)
            {
                var response = allResponses[currentSelection];
                waitingForResponse = false;
                OnPickedResponse(response, currentSelection);
            }
        }
    }

    void UpdateResponseSelection(int selection)
    {
        for(int i = 0; i < tempResponseButtons.Count; i++)
        {
            if(i == selection)
            {
                tempResponseButtons[i].GetComponent<TMP_Text>().color = GlobalSettings.i.HighlightedColor;
            }
            else
            {
                tempResponseButtons[i].GetComponent<TMP_Text>().color = Color.black;
            }
        }
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null;

        if (response.DialougeObject)
        {
            dialougeManager.ShowDialouge(response.DialougeObject);
        }
        else 
        {
            dialougeManager.CloseDialougeBox(); 
        }

        dialougeManager.ShowDialouge(response.DialougeObject);
    }
}
