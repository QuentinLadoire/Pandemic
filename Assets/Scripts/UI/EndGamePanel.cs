using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] Button m_quitGameButton = null;
    [SerializeField] Button m_replayGameButton = null;

    void OnQuitGameButtonClick()
    {
        Application.Quit();
    }
    void OnReplayGameButtonClick()
    {
        SceneManager.LoadScene("Pandemic");
    }

    void OnEndGame()
    {
        GameStaticRef.canvasController.SetActiveGamePanel(false);
        GameStaticRef.canvasController.SetActiveEndGamePanel(true);
    }

    private void Start()
    {
        m_quitGameButton.onClick.AddListener(OnQuitGameButtonClick);
        m_replayGameButton.onClick.AddListener(OnReplayGameButtonClick);

        GameStaticRef.gameController.onEndGame += OnEndGame;

        GameStaticRef.canvasController.SetActiveEndGamePanel(false);
    }
    private void OnDestroy()
    {
        GameStaticRef.gameController.onEndGame -= OnEndGame;
    }
}
