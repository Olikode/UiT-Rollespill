using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelectionState
{
    ClassSelection,
    StudentInfoSelection,
    ProgressBar,
    ConfirmCharacter,
};

public class CharacterSelectionUI : MonoBehaviour
{
    CharacterSelectionState state;

    [Header("Lists")]
    [SerializeField] List<UnitBase> playableCharacters;
    [SerializeField] List<Sprite> maleCharacterImages;
    [SerializeField] List<Sprite> femaleCharactersImages;

    [Header("Game Objects (views)")]
    [SerializeField] GameObject classSelectionGO;
    [SerializeField] GameObject characterInfoGO;
    [SerializeField] GameObject sendApplicationGO;
    [SerializeField] GameObject confirmCharacterGO;

    [Header("Class Selection Elements")]
    [SerializeField] List<Text> classes;
    [SerializeField] Text classDesciption;

    [Header("Info Selection Elements")]
    
    [SerializeField] Text nameInput;
    [SerializeField] Text nameLable;
    [SerializeField] Image maleBG;
    [SerializeField] Image femaleBG;
    [SerializeField] List<Text> genders;
    [SerializeField] List<Image> genderImages;

    [Header("Progress bar")]

    [SerializeField] GameObject progressBar;
    [SerializeField] Text progress;
    [SerializeField] Text progressMsg;

    [Header("Confirm Character Elements")]
    [SerializeField] Text studentName;
    [SerializeField] Text studentClass;
    [SerializeField] Image studentIDPhoto;

    [Header("Navigation")]
    [SerializeField] GameObject back;
    [SerializeField] GameObject next;

    int selectedClass = 0;
    int selectedGender = 0;

    UnitBase playerUnitBase;

    public bool confirmedCharacter = false;

    private void Awake()
    {
        back.SetActive(false);
        for (int i = 0; i < playableCharacters.Count; i++)
            classes[i].text = playableCharacters[i].Type.ToString();
    }

    public void HandleUpdate()
    {
        if(state == CharacterSelectionState.ClassSelection)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedClass;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedClass;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                // shows sprites based on class
                genderImages[0].sprite = maleCharacterImages[selectedClass];
                genderImages[1].sprite = femaleCharactersImages[selectedClass];
                
                back.SetActive(true);
                next.SetActive(true);
                classSelectionGO.SetActive(false);
                characterInfoGO.SetActive(true);
                state = CharacterSelectionState.StudentInfoSelection;
            }
        }
        else if (state == CharacterSelectionState.StudentInfoSelection)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedGender;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedGender;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if(nameInput.text != "")
                {
                nameLable.color = Color.black;
                studentIDPhoto.sprite = genderImages[selectedGender].sprite;
                studentName.text = nameInput.text;

                // Shows progress bar for sending application
                state = CharacterSelectionState.ProgressBar;
                characterInfoGO.SetActive(false);
                sendApplicationGO.SetActive(true);
                StartCoroutine(ProgressLoad());
                }
                // if name field is not filled, alert player
                else
                {
                    nameLable.color = Color.red;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                back.SetActive(false);
                next.SetActive(true);
                characterInfoGO.SetActive(false);
                classSelectionGO.SetActive(true);
                state = CharacterSelectionState.ClassSelection;
            }
        }
        else if (state == CharacterSelectionState.ConfirmCharacter)
        {
            sendApplicationGO.SetActive(false);
            studentClass.text = playerUnitBase.Type.ToString();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                // player confirms selections
                confirmedCharacter = true;
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                confirmCharacterGO.SetActive(false);
                characterInfoGO.SetActive(true);
                state = CharacterSelectionState.StudentInfoSelection;
            }
        }

        selectedClass = Mathf.Clamp(selectedClass, 0, playableCharacters.Count-1);
        selectedGender = Mathf.Clamp(selectedGender, 0, 1);

        UpdateClassSelection();
        UpdateGenderSelection();
    }

    void UpdateClassSelection()
    {
        for (int i = 0; i < playableCharacters.Count; i++)
        {
            if (i == selectedClass)
            {
                classes[i].color = GlobalSettings.i.HighlightedColor;
                classDesciption.text = playableCharacters[i].Description;
                playerUnitBase = playableCharacters[i];
            }
            else
                classes[i].color = Color.black;
        }
    }
    void UpdateGenderSelection()
    {
        for (int i = 0; i < genders.Count; i++)
        {
            if (i == selectedGender)
            {
                genders[i].color = GlobalSettings.i.HighlightedColor;
            }
            else
                genders[i].color = Color.black;
        }
    }

    IEnumerator ProgressLoad()
    {
        float value = 0f;
        float endValue = 1f;
        progressMsg.text = "Sender din søknad";

        // simulates a loading bar
        while (value < endValue)
        {
            value = value + Random.Range(0.05f, 0.20f);
            value = Mathf.Clamp(value, 0, endValue);

            progressBar.transform.localScale = new Vector3(value, 0.5f);
            int percentage = (int)(value*100);
            progress.text =$"{percentage}%";
            yield return new WaitForSeconds(0.2f);
        }

        progressMsg.text = "Søknad sendt";
        yield return new WaitForSeconds(2f);

        // sends player to confirmation screen
        back.SetActive(true);
        next.SetActive(true);
        confirmCharacterGO.SetActive(true);
        state = CharacterSelectionState.ConfirmCharacter;
    }

    public UnitBase PlayerUnitBase{
        get {return playerUnitBase;}
    }

    public string PlayerName{
        get {return nameInput.text;}
    }

    public Image PlayerImage{
        get {return studentIDPhoto;}
    }

}
