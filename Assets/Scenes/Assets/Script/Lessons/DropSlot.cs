using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSlot : MonoBehaviour
{
    public bool isTerminalSlot = true; // only true for right-side slots

    public LessonBlock currentBlock;

    public bool IsOccupied => currentBlock != null && currentBlock.gameObject != null;

    public void AssignBlock(LessonBlock block)
    {
        currentBlock = block;
    }

    public void ClearBlock()
    {
        currentBlock = null;
    }
}