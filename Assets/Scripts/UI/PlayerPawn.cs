using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerPawn : MonoBehaviour
{
    [SerializeField] Image m_pawnColorImage = null;
    [SerializeField] TextMeshProUGUI m_pawnColorTextMP = null;

    void OnStartNewTurn(Player currentPlayer)
    {
        m_pawnColorImage.color = currentPlayer.pawn.color;
        m_pawnColorTextMP.text = currentPlayer.playerName;
    }

    public void Init()
    {
        GameStaticRef.gameController.onStartNewTurn += OnStartNewTurn;
    }
    private void OnDestroy()
    {
        GameStaticRef.gameController.onStartNewTurn -= OnStartNewTurn;
    }
}
