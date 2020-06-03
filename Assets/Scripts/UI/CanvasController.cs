using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnButtonClick();

public class CanvasController : MonoBehaviour
{
	public GamePanel gamePanel = null;
	public NewGamePanel newGamePanel = null;
	public EndGamePanel endGamePanel = null;

	public void SetActiveGamePanel(bool value)
	{
		gamePanel.gameObject.SetActive(value);
	}
	public void SetActiveNewGamePanel(bool value)
	{
		newGamePanel.gameObject.SetActive(value);
	}
	public void SetActiveEndGamePanel(bool value)
	{
		endGamePanel.gameObject.SetActive(value);
	}

	private void Awake()
	{
		GameStaticRef.canvasController = this;
	}
}
