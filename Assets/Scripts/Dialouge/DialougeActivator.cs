using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialougeActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialougeObject dialougeObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PlayerController playerController))
        {
            playerController.Interactable = this; 
        }
    }

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
        if (TryGetComponent(out DialougeResponseEvents responseEvents))
        {
            playerController.DialougeManager.AddResponseEvents(responseEvents.Events);
        }

        playerController.DialougeManager.ShowDialouge(dialougeObject);
    }
}
