using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGamePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_numberPlayerTextMP = null;
    [SerializeField] Slider m_playerNumberSlider = null;
    [SerializeField] TMP_Dropdown m_difficultyDropDown = null;
    [SerializeField] Button m_startGameButton = null;
    [SerializeField] TextMeshProUGUI m_startButtonTextMP = null;

    bool m_gameIsCreated = false;

    void OnNumberPlayerSliderValueChange(float value)
    {
        m_numberPlayerTextMP.text = "Number of Player : " + (int)value;
        GameStaticRef.gameController.nbPlayer = (int)value;
    }
    void OnDifficultyDropDownValueChange(int value)
    {
        GameStaticRef.gameController.difficulty = (Difficulty)value;
    }
    void OnStartGameButtonClick()
    {
        if (!m_gameIsCreated)
        {
            m_gameIsCreated = true;

            GameStaticRef.gameController.CreateGame();

            m_startButtonTextMP.text = "Start the Game";
        }
        else
        {
            GameStaticRef.gameController.StartGame();

            GameStaticRef.canvasController.SetActiveGamePanel(true);
            GameStaticRef.canvasController.SetActiveNewGamePanel(false);
        }
    }

    private void Start()
    {
        m_playerNumberSlider.onValueChanged.AddListener(OnNumberPlayerSliderValueChange);
        m_difficultyDropDown.onValueChanged.AddListener(OnDifficultyDropDownValueChange);
        m_startGameButton.onClick.AddListener(OnStartGameButtonClick);
    }
}
