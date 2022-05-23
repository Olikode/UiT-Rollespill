using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialougeObject dialougeObject;
    [SerializeField] public SpriteRenderer nPCImage; 



    public void Start()
    {
        nPCImage = GetComponent<SpriteRenderer>();
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
            playerController.Interactable = this; 
        }
    }

    //Resets the dialouge interactable bool
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController playerController))
        {
            if (playerController.Interactable is DialougeActivator dialougeActivator && dialougeActivator == this)
            {
                playerController.Interactable = null; 
            }
        }
    }


    public void Interact(PlayerController playerController)
    {
        foreach (DialougeResponseEvents responseEvents in GetComponents<DialougeResponseEvents>())
        {
            if (responseEvents.DialougeObject == dialougeObject)
            {
                playerController.DialougeManager.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        playerController.DialougeManager.ShowDialouge(dialougeObject);
    }
}
