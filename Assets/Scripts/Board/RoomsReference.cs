using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpacesGroup
{
	public GameObject[] spaces = null;
	public int supplyProduction = 0;

	[HideInInspector] public bool isAssigned = false;
	[HideInInspector] public List<Dice> assignedDices = null;
}

public class Room : MonoBehaviour
{
	[SerializeField] DiceValue m_diceValue = DiceValue.None;
	public DiceValue diceValue { get => m_diceValue; }

	[SerializeField] RoomType m_roomType = RoomType.None;
	public RoomType roomType { get => m_roomType; }

	public Collider hitBox = null;
	public GameObject[] pawnPoints = new GameObject[4];
	public SpacesGroup[] spaceGroup = null;
	public Room[] neighbourRooms = null;

	[HideInInspector] public int pawnPointIndex = 0;

	public bool CanActivateRoom()
	{
		foreach (var group in spaceGroup) if (group.isAssigned) return true;

		return false;
	}
	public bool IsNeighbour(Room room)
	{
		foreach (var neighbourRoom in neighbourRooms) if (neighbourRoom == room) return true;

		return false;
	}
	public void AssignDices(List<Dice> toAssign)
	{
		foreach (var group in spaceGroup)
		{
			if (!group.isAssigned)
			{
				if (toAssign.Count == 0) return;

				List<Dice> tmp = null;
				if (toAssign.Count > group.spaces.Length)
				{
					tmp = new List<Dice>();
					for (int i = 0; i < group.spaces.Length; i++)
					{
						tmp.Add(toAssign[0]);
						toAssign.RemoveAt(0);
					}
				}
				else if (toAssign.Count == group.spaces.Length)
				{
					tmp = new List<Dice>(toAssign);
					toAssign.Clear();
				}
				else if (toAssign.Count < group.spaces.Length)
				{
					//Debug.Log("Can't Assign Dices : Not enough Dice");
					return;
				}

				//assign the dices to the group
				group.isAssigned = true;
				group.assignedDices = tmp;

				for (int i = 0; i < group.spaces.Length; i++)
				{
					//Assign the room to the dice
					tmp[i].assignedRoom = this;

					//Set Dice to used
					tmp[i].isSelected = false;
					//tmp[i].isUsed = true;

					//Move the dice into the room
					tmp[i].MoveTo(group.spaces[i].transform);
				}
			}
		}
	}
	public int ActivateRoom()
	{
		int productionCount = 0;
		foreach (var group in spaceGroup)
		{
			if (group.isAssigned)
			{
				//UnAssign all dices and move to zero
				foreach (var dice in group.assignedDices)
				{
					dice.assignedRoom = null;
					dice.isUsed = true;
					dice.ResetPosition();
				}

				//UnAssign the group
				group.isAssigned = false;

				productionCount = group.supplyProduction;
			}
		}

		return productionCount;
	}
}

[System.Serializable]
public class CommandCenterRoom
{
	public GameObject townDeck = null;
	public GameObject[] timeTokenPoints = new GameObject[9];
}
