using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchConnection 
{
    public ObjectMatchingGame source;
    public ObjectMatchform target;
    public UnityEngine.LineRenderer line;
    public bool IsLocked; // ✅ NEW

    public MatchConnection(ObjectMatchingGame source, ObjectMatchform target, UnityEngine.LineRenderer line)
    {
        this.source = source;
        this.target = target;
        this.line = line;
        this.IsLocked = false;
    }

    public bool IsCorrect()
    {
        return source.GetMatchID() == target.Get_ID();
    }
}

