using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class DialougeNPCPicture : MonoBehaviour
{
    //On collision sets the canvas npc Image to match the given sprite.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DialougeManager.talkingNPC = this.GetComponent<Image>();
        }
    }

    //ON exit, it resets the npc Image.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
        }
    }
}
