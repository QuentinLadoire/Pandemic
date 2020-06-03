using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyRoom : Room
{
	public GameObject[] supplyBar = new GameObject[4];

	public int supplyCountMax { get => supplyBar.Length; }
	public int emptySupplyCount { get => supplyCountMax - assignedSupplys.Count; }

	[HideInInspector] public int supplyBarIndex = 0;
	[HideInInspector] public List<Supply> assignedSupplys = new List<Supply>();

	public int GetCountOf(SupplyType supplyType)
	{
		int result = 0;
		foreach (var supply in assignedSupplys) if (supply.type == supplyType) result++;

		return result;
	}

	public void RePositionSupplys()
	{
		for (int i = 0; i < assignedSupplys.Count; i++) assignedSupplys[i].PlaceTo(supplyBar[i].transform);
	}
	public Transform GetEmptySlotPosition()
	{
		return supplyBar[assignedSupplys.Count].transform;
	}

	public void MoveSupplysTo(int nbSupplys, SupplyRoom toRoom)
	{
		if (nbSupplys == 0 || nbSupplys < 0) return;
		if (assignedSupplys.Count == 0) return;

		//if the current room doesnt have enough supply, set nbSupply to the count of assignedSupply into this room
		if (nbSupplys > assignedSupplys.Count) nbSupplys = assignedSupplys.Count;

		//if the other room doesnt have enough empty supply slot, set nbSupply to the count of empty slot in the other room
		if (nbSupplys > toRoom.emptySupplyCount)
		{
			int tmp = nbSupplys - toRoom.emptySupplyCount;
			for (int i = 0; i < tmp; i++)
			{
				toRoom.assignedSupplys[0].ReturnToStartRoom();
			}

			toRoom.RePositionSupplys();
		}

		//move the supply to the other room
		for (int i = 0; i < nbSupplys; i++)
		{
			assignedSupplys[assignedSupplys.Count - 1].PlaceInRoom(toRoom);
		}
	}
}
