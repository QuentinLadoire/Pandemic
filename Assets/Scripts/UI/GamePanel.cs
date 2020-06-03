using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePanel : MonoBehaviour
{
    [SerializeField] Button m_characterCardButton = null;
    [SerializeField] Button m_movePawnButton = null;
    [SerializeField] Button m_movePlaneButton = null;
    [SerializeField] Button m_assignButton = null;
    [SerializeField] Button m_activateButton = null;
    [SerializeField] Button m_rollDiceButton = null;
    [SerializeField] Button m_skipTurnButton = null;

    [SerializeField] DiceUI m_diceUI = null;
    public DiceUI diceUI { get => m_diceUI; }

    [SerializeField] PlayerPawn m_playerPawn = null;
    public PlayerPawn playerPawn { get => m_playerPawn; }

    [SerializeField] TextMeshProUGUI m_actionText = null;

    public OnButtonClick onShowHideCharacterCardButtonClick = () => { };
    public OnButtonClick onMovePawnButtonClick = () => { };
    public OnButtonClick onMovePlaneButtonClick = () => { };
    public OnButtonClick onAssignButtonClick = () => { };
    public OnButtonClick onActivateButtonClick = () => { };
    public OnButtonClick onRollDiceButtonClick = () => { };
    public OnButtonClick onSkipTurnButtonClick = () => { };

    void OnStartNewTurn(Player currentPlayer)
    {
        m_actionText.text = "Roll the dices";
    }
    void OnRollDiceResult(Dice[] dices)
    {
        OnOtherAction();
    }
    void OnOtherAction()
    {
        m_actionText.text = "Choose a action";
    }
    void OnMovePawnStateEnter()
    {
        m_actionText.text = "Click on any neighboring Room";
    }
    void OnMovePlaneStateEnter()
    {
        m_actionText.text = "Click on any neighboring TownSlot";
    }

    private void Awake()
    {
        m_characterCardButton.onClick.AddListener(() => 
        {
            onShowHideCharacterCardButtonClick();
        });

        m_movePawnButton.onClick.AddListener(() =>
        {
            onMovePawnButtonClick();
        });

        m_movePlaneButton.onClick.AddListener(() =>
        {
            onMovePlaneButtonClick();
        });

        m_assignButton.onClick.AddListener(() =>
        {
            onAssignButtonClick();
        });

        m_activateButton.onClick.AddListener(() =>
        {
            onActivateButtonClick();
        });

        m_rollDiceButton.onClick.AddListener(() =>
        {
            onRollDiceButtonClick();
        });

        m_skipTurnButton.onClick.AddListener(() =>
        {
            onSkipTurnButtonClick();
        });
    }

    private void Start()
    {
        GameStaticRef.gameController.onStartNewTurn += OnStartNewTurn;
        GameStaticRef.gameController.onRollDiceResult += OnRollDiceResult;
        GameStaticRef.gameController.onMovePawnStateExit += OnOtherAction;
        GameStaticRef.gameController.onMovePlaneStateExit += OnOtherAction;
        GameStaticRef.gameController.onMovePawnStateEnter += OnMovePawnStateEnter;
        GameStaticRef.gameController.onMovePlaneStateEnter += OnMovePlaneStateEnter;

        m_diceUI.Init();
        m_playerPawn.Init();

        GameStaticRef.canvasController.SetActiveGamePanel(false);
    }
    private void OnDestroy()
    {
        GameStaticRef.gameController.onStartNewTurn -= OnStartNewTurn;
        GameStaticRef.gameController.onRollDiceResult -= OnRollDiceResult;
        GameStaticRef.gameController.onMovePawnStateExit -= OnOtherAction;
        GameStaticRef.gameController.onMovePlaneStateExit -= OnOtherAction;
        GameStaticRef.gameController.onMovePawnStateEnter -= OnMovePawnStateEnter;
        GameStaticRef.gameController.onMovePlaneStateEnter -= OnMovePlaneStateEnter;
    }
}
