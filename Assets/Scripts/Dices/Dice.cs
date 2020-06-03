using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Player ownerPlayer { get; set; }
    public int index { get; set; }
    public Room assignedRoom { get; set; }
    public bool isAssigned { get => assignedRoom != null; }
    public bool isUsed { get; set; }
    public bool isSelected { get; set; }

    DiceValue m_value = DiceValue.None;
    public DiceValue value { get => m_value; }

    void OnDiceButtonClick()
    {
        if (!GameStaticRef.gameController.IsMyPlayerTurn(ownerPlayer)) return;
        if (GameStaticRef.gameController.waitFirstRoll) return;
        if (!GameStaticRef.gameController.canRollDice) return;
        if (isAssigned) return;
        if (isUsed) return;

        isSelected = !isSelected;
    }

    public void Rool()
    {
        if (!isSelected) return;

        m_value = (DiceValue)Random.Range(0, (int)DiceValue.Count);

        isSelected = false;
    }
    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }
    public void MoveTo(Transform target)
    {
        var position = transform.position;

        transform.rotation = target.rotation;
        position = target.position;
        position.y += 0.5f;

        transform.position = position;
    }

    public void Init(int index)
    {
        this.index = index;
        GameStaticRef.canvasController.gamePanel.diceUI.onDiceButtonClicks[this.index] += OnDiceButtonClick;
    }
    private void OnDestroy()
    {
        GameStaticRef.canvasController.gamePanel.diceUI.onDiceButtonClicks[index] -= OnDiceButtonClick;
    }
}
