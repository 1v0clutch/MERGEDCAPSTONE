using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class ObjectMatchform : MonoBehaviour
{
	[SerializeField] private int matchId;
	public bool IsLocked { get; private set; } = false;

	public void Lock()
	{
		IsLocked = true;
	}
	public void Unlock()
	{
		IsLocked = false;
		// Optional: reset visuals
	}
	public int Get_ID()
	{
		return matchId;
	}
}

