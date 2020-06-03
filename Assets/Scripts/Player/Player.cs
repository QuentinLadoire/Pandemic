using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public string playerName = "PlayerName";
    [HideInInspector] public CharacterCard characterCard = null;
    [HideInInspector] public Pawn pawn = null;
    [HideInInspector] public Dice[] dices = new Dice[6];

    public int nbSelectedDice
    {
        get
        {
            int result = 0;
            foreach (var dice in dices) if (dice.isSelected) result++;

            return result;
        }
    }
    public int nbUsedDice
    {
        get
        {
            int result = 0;
            foreach (var dice in dices) if (dice.isUsed) result++;

            return result;
        }
    }
    public int nbDiceToRoll { get => nbSelectedDice; }
    public int nbDiceToMovePawn { get => nbSelectedDice; }
    public int nbDiceToMovePlane
    {
        get
        {
            int result = 0;
            foreach (var dice in dices) if (dice.isSelected && dice.value == DiceValue.Plane) result++;

            return result;
        }
    }
    public int nbDiceAssignable
    {
        get
        {
            int result = 0;
            foreach (var dice in dices) if (dice.isSelected && dice.value == pawn.currentRoom.diceValue) result++;

            return result;
        }
    }

    public bool canActivateRoom
    {
        get => pawn.currentRoom.CanActivateRoom();
    }

    public List<Dice> selectedDices
    {
        get
        {
            List<Dice> tmp = new List<Dice>();
            foreach (var dice in dices) if (dice.isSelected) tmp.Add(dice);

            return tmp;
        }
    }
    public List<Dice> assignedDices
    {
        get
        {
            List<Dice> tmp = new List<Dice>();
            foreach (var dice in dices) if (dice.isAssigned) tmp.Add(dice);

            return tmp;
        }
    }
    public List<Dice> assignableDices
    {
        get
        {
            List<Dice> tmp = new List<Dice>();
            foreach (var dice in dices) if (dice.isSelected && dice.value == pawn.currentRoom.diceValue) tmp.Add(dice);

            return tmp;
        }
    }

    void OnStartNewTurn(Player currentPlayer)
    {
        foreach (var dice in dices)
        {
            dice.isUsed = false;

            if (!dice.isAssigned) dice.isSelected = true;
        }
    }

    void UseDiceForMovePawn()
    {
        foreach (var dice in dices)
        {
            if (dice.isSelected && !dice.isUsed)
            {
                dice.isSelected = false;
                dice.isUsed = true;

                return;
            }
        }
    }

    public void UseDiceForMovePlane()
    {
        foreach (var dice in dices)
        {
            if (dice.isSelected && !dice.isUsed && dice.value == DiceValue.Plane)
            {
                dice.isSelected = false;
                dice.isUsed = true;

                return;
            }
        }
    }
    public void MovePawn(Room room)
    {
        if (pawn.currentRoom.IsNeighbour(room) && nbDiceToMovePawn != 0)
        {
            pawn.PlaceInRoom(room);
            UseDiceForMovePawn();
        }
    }

    public void AssignDices()
    {
        pawn.currentRoom.AssignDices(assignableDices);
    }
    public int ActivateRoom()
    {
        return pawn.currentRoom.ActivateRoom();
    }
    public RoomType GetRoomType()
    {
        return pawn.currentRoom.roomType;
    }
    public Room GetRoom()
    {
        return pawn.currentRoom;
    }

    private void Start()
    {
        GameStaticRef.gameController.onStartNewTurn += OnStartNewTurn;
    }
    private void OnDestroy()
    {
        GameStaticRef.gameController.onStartNewTurn -= OnStartNewTurn;
    }
}
