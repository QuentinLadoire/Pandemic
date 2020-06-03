using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicesReference
{
	public GameObject[][] dices = new GameObject[4][];
	public GameObject[] blueDice = new GameObject[6];
	public GameObject[] brownDice = new GameObject[6];
	public GameObject[] greenDice = new GameObject[6];
	public GameObject[] greyDice = new GameObject[6];
}
public class PawnsReference
{
	public GameObject[] pawns = new GameObject[4];
	public GameObject planePawn = null;
}
public class SupplysReference
{
	public GameObject[] energySupplys = new GameObject[4];
	public GameObject[] firstAidSupplys = new GameObject[4];
	public GameObject[] foodSupplys = new GameObject[4];
	public GameObject[] vaccineSupplys = new GameObject[4];
	public GameObject[] waterSupplys = new GameObject[4];
}
public class TokensReference
{
	public GameObject[] timeToken = new GameObject[9];
	public GameObject wasteToken = null;
}
public class CardsReference
{
	public GameObject[] characterCards = new GameObject[4];
	public GameObject[] townCards = new GameObject[24];
	public TownRegion townRegion = new TownRegion();
}
