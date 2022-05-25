using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSelectionState
{
    ClassSelection,
    GenderSelection,
    ConfirmCharacter,
};

public class CharacterSelectionUI : MonoBehaviour
{
    CharacterSelectionState state;
    [SerializeField] List<UnitBase> playableCharacters;
    [SerializeField] List<Sprite> maleCharacterImages;
    [SerializeField] List<Sprite> femaleCharactersImages;

    [Header("Game Objects (views)")]
    [SerializeField] GameObject classSelectionGO;
    [SerializeField] GameObject characterInfoGO;
    [SerializeField] GameObject confirmCharacterGO;

    [Header("Class Selection Elements")]
    [SerializeField] List<Text> classes;
    [SerializeField] Text desciption;

    [Header("Gender Selection Elements")]
    // name form


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
    string playerName;
    Image playerImage;

    public bool confirmedCharacter = false;

    private void Awake()
    {
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

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                genderImages[0].sprite = maleCharacterImages[selectedClass];
                genderImages[1].sprite = femaleCharactersImages[selectedClass];
                
                classSelectionGO.SetActive(false);
                characterInfoGO.SetActive(true);
                state = CharacterSelectionState.GenderSelection;
            }
        }
        else if (state == CharacterSelectionState.GenderSelection)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedGender;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedGender;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                studentIDPhoto.sprite = genderImages[selectedGender].sprite;

                characterInfoGO.SetActive(false);
                confirmCharacterGO.SetActive(true);
                state = CharacterSelectionState.ConfirmCharacter;
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                characterInfoGO.SetActive(false);
                classSelectionGO.SetActive(true);
                state = CharacterSelectionState.ClassSelection;
            }
        }
        else if (state == CharacterSelectionState.ConfirmCharacter)
        {
            studentClass.text = playerUnitBase.Type.ToString();
            //studentName.text = 

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                playerName = "Roger";
                confirmedCharacter = true;
                // create playerUnitBase asset
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
            {
                confirmCharacterGO.SetActive(false);
                characterInfoGO.SetActive(true);
                state = CharacterSelectionState.GenderSelection;
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
        get {return playerName;}
    }

    public Image PlayerImage{
        get {return studentIDPhoto;}
    }

}
