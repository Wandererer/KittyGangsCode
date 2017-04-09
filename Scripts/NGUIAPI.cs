using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGUIAPI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickStartButton()
	{
		Game.Instance.gameScene.DissapearStartImage ();
		Game.Instance.gameScene.AppearWaitingImage ();
		Game.Instance.roomManager.OnJoinedLobby ();
		Game.Instance.gameScene.gameState = GameState.Wait;
	}

	public void OnClickTutorialButton()
	{
		Game.Instance.gameScene.DissapearStartImage ();
		Game.Instance.gameScene.ApeearTutorialImage ();
		Game.Instance.gameScene.gameState = GameState.Tutorial;
	}

	public void OnClickStartButtonInTutorial()
	{
		Game.Instance.gameScene.DissapearTutorialImage ();
		Game.Instance.gameScene.AppearWaitingImage();
		Game.Instance.roomManager.OnJoinedLobby ();
		Game.Instance.gameScene.gameState = GameState.Wait;
	}

}
