using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class DicesPrefabReference
{
    public GameObject blueDice = null;
    public GameObject brownDice = null;
    public GameObject greenDice = null;
    public GameObject greyDice = null;
}
[System.Serializable]
class PawnsPrefabReference
{
    public GameObject bluePawn = null;
    public GameObject brownPawn = null;
    public GameObject greenPawn = null;
    public GameObject greyPawn = null;
    public GameObject planePawn = null;
}
[System.Serializable]
class SupplysPrefabReference
{
    public GameObject energySupply = null;
    public GameObject firstAidSupply = null;
    public GameObject foodSupply = null;
    public GameObject vaccineSupply = null;
    public GameObject waterSupply = null;
}
[System.Serializable]
class TokensPrefabReference
{
    public GameObject timeToken = null;
    public GameObject wasteToken = null;
}

[System.Serializable]
public class TownRegion
{
    public GameObject[] blackTownCards = new GameObject[6];
    public GameObject[] blueTownCards = new GameObject[6];
    public GameObject[] redTownCards = new GameObject[6];
    public GameObject[] yellowTownCards = new GameObject[6];
}
[System.Serializable]
class CardsPrefabReference
{
    public GameObject[] characterCards = new GameObject[4];
    public TownRegion townRegion = new TownRegion();
}