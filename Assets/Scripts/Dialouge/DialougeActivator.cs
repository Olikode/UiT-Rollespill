using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialougeActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialougeObject dialougeObject;
    public static Sprite nPCImage; 

    public void Start()
    {
        nPCImage = gameObject.GetComponent<SpriteRenderer>().sprite;
    }
    //Updates the dialougeObject
    public void UpdateDialogueObject(DialougeObject dialougeObject)
    {
        this.dialougeObject = dialougeObject;
    }

    //Checks if NPC is colliding with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController playerController))
        {
            nPCImage = gameObject.GetComponent<SpriteRenderer>().sprite;
            GameController.Interactable = this;
        }
    }

    //Resets the dialouge interactable bool
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController playerController))
        {
            if (GameController.Interactable is DialougeActivator dialougeActivator && dialougeActivator == this)
            {
                GameController.Interactable = null;
            }
        }
    }


    public void Interact(PlayerController playerController)
    {
        foreach (DialougeResponseEvents responseEvents in GetComponents<DialougeResponseEvents>())
        {
            if (responseEvents.DialougeObject == dialougeObject)
            {
                GameController.dialougeManager.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        GameController.dialougeManager.ShowDialouge(dialougeObject);
    }
}
