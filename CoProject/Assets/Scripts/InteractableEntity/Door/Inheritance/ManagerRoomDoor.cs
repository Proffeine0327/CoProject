using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class ManagerRoomDoor : Door
{
    public override void Interact()
    {
        if(situation != null)
            SequenceManager.StartSequence(situation);
    }
}
