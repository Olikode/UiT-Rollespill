using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public enum InventoryUIState { ItemSelection, UseSelection, Busy, ForgetMove}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    Inventory inventory;
    RectTransform itemListRect;
    [SerializeField] LearnMoveUI learnMoveUI;
    MoveBase moveToLearn;
    [SerializeField] SummaryUI summaryUI;
    [SerializeField] GameObject player;

    [Header("Dialog Box")]
    [SerializeField] GameObject UIDialogBox;
    [SerializeField] Text UIDialogText;

    int selectedItem = 0;
    int selectedCategory = 0;
    public bool inBattle = false;
    InventoryUIState state;
    Unit playerUnit;

    public string itemName;
    public string itemMessage;

    Action onItemUsed;

    // number of items shown before scrollbar appares
    const int itemsInViewport = 8;

    List<ItemSlotUI> slotUIList;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        playerUnit = player.gameObject.GetComponent<UnitList>().GetPlayerUnit();

        // clear existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }
    }

    public void HandleUpdate(Action onBack, Action onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;

        if(state == InventoryUIState.ItemSelection)
        {
            
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;

            if (selectedCategory > Inventory.ItemCategories.Count -1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count -1;



            selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);
            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            UpdateItemSlection();

            if(prevCategory != selectedCategory){
                // reset selected item for switching categories
                ResetSelection();
                // show new category list
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                // go to summary screen when using items
                OpenSummaryScreen();
            }
            else if(Input.GetKeyDown(KeyCode.Escape))
                onBack?.Invoke();
        }
        else if(state == InventoryUIState.UseSelection)
        {
            Action onSelected = () =>
            {
                var item = inventory.GetItem(selectedItem, selectedCategory);
                var usable = ItemUsable(item);
                StartCoroutine(UseItem(usable));
            };

            Action onBackSummary = () =>
            {
                // close summary screen when canceling using items
                UIDialogBox.gameObject.SetActive(false);
                CloseSummaryScreen();
            };

            summaryUI.HandleUpdate(onBackSummary, onSelected);
        }
        else if(state == InventoryUIState.ForgetMove)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(MoveToForgetSelected(moveIndex));
            };

            learnMoveUI.HandleMoveSelection(onMoveSelected);
        }
    }

    void UpdateItemSlection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        for (int i = 0; i <slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count);

        if(slots.Count > 0){
            // shows the player the selected item and description
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if(slotUIList.Count > 0){
            float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, selectedItem) * slotUIList[0].Height;
            itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

            bool showUpArrow = selectedItem > itemsInViewport/2;
            upArrow.gameObject.SetActive(showUpArrow);

            bool showDownArrow = selectedItem + itemsInViewport/2 < slotUIList.Count;
            downArrow.gameObject.SetActive(showDownArrow);
        }
    }

    void OpenSummaryScreen()
    {
        state = InventoryUIState.UseSelection;
        summaryUI.gameObject.SetActive(true);
    }
    void CloseSummaryScreen()
    {
        state = InventoryUIState.ItemSelection;
        summaryUI.gameObject.SetActive(false);
    }

    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    IEnumerator UseItem(bool canBeUsed)
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItem(selectedItem, selectedCategory);

        if(canBeUsed )
        {
            yield return HandleBookItems();
            yield break;
        }

        var usedItem = inventory.UseItem(selectedItem, selectedCategory, playerUnit);

        if (usedItem != null && canBeUsed)
        {
            
            if(usedItem is RecoveryItem)
            {
                // shows the player item used in battle dialog
                if(inBattle){
                    itemName = $"{item.Name}";
                    itemMessage = $"{item.UseMessage}";

                    CloseSummaryScreen();
                    onItemUsed?.Invoke();
                }
                // shows the player item used in dialog box overlaying UI
                else
                {
                    UIDialogBox.gameObject.SetActive(true);
                    UIDialogText.text = $"Du brukte {item.Name}";
                    yield return new WaitForSeconds(2f);
                    UIDialogBox.gameObject.SetActive(false);
                    CloseSummaryScreen();
                }
            }
        }
        // shows the player if item could not be used
        else
        {
            UIDialogBox.gameObject.SetActive(true);
            UIDialogText.text = $"Du kan ikke bruke {item.Name}";
            yield return new WaitForSeconds(2f);
            UIDialogBox.gameObject.SetActive(false);
            CloseSummaryScreen();
        }
    }

     IEnumerator ChooseMoveToForget(MoveBase newMove)
    {
        state = InventoryUIState.Busy;

        UIDialogBox.gameObject.SetActive(true);
        UIDialogText.text = $"Velg et angrep å glemme";
        yield return new WaitForSeconds(2f);
        UIDialogBox.gameObject.SetActive(false);
        
        learnMoveUI.gameObject.SetActive(true);
        learnMoveUI.SetMoveData(playerUnit.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.ForgetMove;
    }

    IEnumerator HandleBookItems()
    {
        var bookItem = inventory.GetItem(selectedItem, selectedCategory) as BookItem;
        if(bookItem == null)
            yield break;

        if(playerUnit.HasMove(bookItem.Move))
        {
            var text = $"Du kan allerede {bookItem.Move.Name}";
            yield return WriteDialog(text);

            state = InventoryUIState.ItemSelection;
            CloseSummaryScreen();

            yield break;
        }
        
        if(playerUnit.Moves.Count < UnitBase.MaxNumOfMoves)
        {
            playerUnit.LearnMove(bookItem.Move);

            // show player use dialog
            UIDialogBox.gameObject.SetActive(true);
            UIDialogText.text = $"Du lærte {bookItem.Move.Name}";
            yield return new WaitForSeconds(2f);
            UIDialogBox.gameObject.SetActive(false);
        }
        else
        {
            UIDialogBox.gameObject.SetActive(true);
            UIDialogText.text = $"Du prøver å lære {bookItem.Move.Name}";
            yield return new WaitForSeconds(2f);
            UIDialogText.text = $"Men du kan ikke lære mer...\nVil du glemme noe for å lære {bookItem.Move.Name}";
            yield return new WaitForSeconds(2f);
            UIDialogBox.gameObject.SetActive(false);

            yield return ChooseMoveToForget(bookItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.ForgetMove);
        }
    }

    IEnumerator WriteDialog(string text)
    {
        UIDialogBox.gameObject.SetActive(true);
        UIDialogText.text = text;
        yield return new WaitForSeconds(2f);
        UIDialogBox.gameObject.SetActive(false);
    }

    IEnumerator MoveToForgetSelected(int moveIndex)
    {
        learnMoveUI.gameObject.SetActive(false);
        if(moveIndex == UnitBase.MaxNumOfMoves)
        {
            var text = $"Du glemmer ingenting";
            yield return WriteDialog(text);
        }
        else
        {
            // forgets selected move, learns new move
            var selectedMove = playerUnit.Moves[moveIndex].Base;

            var text = $"Du glemte {selectedMove.Name} og lærte {moveToLearn.Name}";
            yield return WriteDialog(text);

            playerUnit.Moves[moveIndex] = new Move(moveToLearn);
        }

        moveToLearn = null;
        CloseSummaryScreen();
        state = InventoryUIState.ItemSelection;
    }

    bool ItemUsable(ItemBase item)
    {
        if (inBattle)
        {
            if(!item.UsableInBattle)
                return false;
        }
        else
        {
            if(!item.UsableOutsideBattle)
                return false;
        }

        return true;
    }
}
