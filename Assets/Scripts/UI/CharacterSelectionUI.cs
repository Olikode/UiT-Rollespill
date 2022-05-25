using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelectionState
{
    ClassSelection,
    StudentInfoSelection,
    ConfirmCharacter,
};

public class CharacterSelectionUI : MonoBehaviour
{
    CharacterSelectionState state;
    [SerializeField] List<UnitBase> playableCharacters;
    [SerializeField] List<Sprite> maleCharacterImages;
    [SerializeField] List<Sprite> femaleCharactersImages;

    [SerializeField] GameObject back;
    [SerializeField] GameObject next;

    [Header("Game Objects (views)")]
    [SerializeField] GameObject classSelectionGO;
    [SerializeField] GameObject characterInfoGO;
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

    [Header("Confirm Character Elements")]
    [SerializeField] Text studentName;
    [SerializeField] Text studentClass;
    [SerializeField] Image studentIDPhoto;

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

                back.SetActive(true);
                next.SetActive(true);
                characterInfoGO.SetActive(false);
                confirmCharacterGO.SetActive(true);
                state = CharacterSelectionState.ConfirmCharacter;
                }
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
            studentClass.text = playerUnitBase.Type.ToString();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                confirmedCharacter = true;
                // create playerUnitBase asset
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
