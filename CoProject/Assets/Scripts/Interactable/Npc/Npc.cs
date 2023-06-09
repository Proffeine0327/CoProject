using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class Npc : Interactable
{
    public override void DisplayUI()
    {
        if(situation != null && canTalk)
            base.DisplayUI();
    }

    public override void Interact()
    {
        Debug.Log(situation.currentSentence?.Type);
        if(situation != null && canTalk)
            SequenceManager.StartSequence(situation);
    }
}
