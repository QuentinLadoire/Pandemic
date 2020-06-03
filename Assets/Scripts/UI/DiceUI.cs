using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    [SerializeField] Color[] m_diceValueColors = new Color[6];

    [SerializeField] Image[] m_diceImages = new Image[6];
    [SerializeField] Button[] m_diceButtons = new Button[6];

    public OnButtonClick[] onDiceButtonClicks = new OnButtonClick[6];

    void OnRollDiceResult(Dice[] dices)
    {
        for (int i = 0; i < dices.Length; i++)
            if (dices[i].value != DiceValue.None) m_diceImages[i].color = m_diceValueColors[(int)dices[i].value];
            else m_diceImages[i].color = Color.white;
    }

    void OnStartNewTurn(Player currentPlayer)
    {
        OnRollDiceResult(currentPlayer.dices);
    }

    private void Awake()
    {
        for (int i = 0; i < onDiceButtonClicks.Length; i++)
        {
            onDiceButtonClicks[i] = () => { };

            int tmp = i;
            m_diceButtons[i].onClick.AddListener(() =>
            {
                onDiceButtonClicks[tmp]();
            });
        }
    }

    public void Init()
    {
        GameStaticRef.gameController.onRollDiceResult += OnRollDiceResult;
        GameStaticRef.gameController.onStartNewTurn += OnStartNewTurn;
    }

    private void Update()
    {
        var dices = GameStaticRef.gameController.currentPlayer.dices;
        for (int i = 0; i < dices.Length; i++)
        {
            var position = m_diceImages[i].transform.localPosition;

            if (dices[i].isSelected) position.y = 20.0f;
            else if (dices[i].isUsed || dices[i].isAssigned) position.y = -20.0f;
            else position.y = 0.0f;

            m_diceImages[i].transform.localPosition = position;
        }
    }

    private void OnDestroy()
    {
        GameStaticRef.gameController.onRollDiceResult -= OnRollDiceResult;
        GameStaticRef.gameController.onStartNewTurn -= OnStartNewTurn;
    }
}
