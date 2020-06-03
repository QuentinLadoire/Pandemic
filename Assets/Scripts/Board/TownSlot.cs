using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSlot : MonoBehaviour
{
	public string townName = "TownName";
	public GameObject planePoint = null;
	public Collider hitBox = null;

	[HideInInspector] public int index = 0;

	public TownCard townCard { get; set; }

	public bool IsNeighbour(TownSlot slot)
	{
		int townMin = slot.index - 1;
		if (townMin < 0) townMin = 23;

		int townMax = slot.index + 1;
		townMax %= 24;

		return (townMin == index || townMax == index);
	}
}
