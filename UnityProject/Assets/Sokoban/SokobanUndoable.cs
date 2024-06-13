using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SokobanUndoable
{
    public void RecordMove(int pointer);
    public void JumpMove(int pointer);
    public void ResetMove();
}
