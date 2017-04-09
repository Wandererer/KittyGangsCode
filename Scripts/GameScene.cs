using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameScene : MonoBehaviour {

	public GameState gameState;
	private WinState winState;

	private GameObject SplashImage; //splash image
	private GameObject PlayImages;
	private GameObject TutorialImage; //tutorial image
	private GameObject GameStartImage; //game start image
	private GameObject WaitText; //waiting text
	private GameObject PlayWaitTimeForGame; //play time when playerlist length==2
	private GameObject[] Icon; //Image icon


	private UISprite[] iconHighlights; //icon highlights
	private UISprite[] endSprites;

	private UISlider gageSlider; //throw gage ui slider

	private GameObject wait3SecondsLabel;

	private float gageAmout=0f; //how many slider value is
	private float oneTime = 0f; //one second for update seconds
	private float splashTime=3f;
	private float WaitTime = 3.5f;

	private int prevSelectedCharacter=-1; //prev selected character number;
	private int selectedCharacter = 1; //선택 된 릭터 번호 
	private int oppSelectedCharacter = 1; //seleted opp character number
	private int sortingOrderNumber=3; 
	private int time=180; //game time 

	private bool isSpacePressed=false; //space pressed or not
	private bool isPlaying=false;

	private GameObject timeLabel; //time label for seconds

	[SerializeField]
	private Sprite[] oppEyeCatSprites; //eye cat sprite list

	[SerializeField]
	private Sprite[] oppFastCatSprites; //fast cat sprite list

	[SerializeField]
	private Sprite[] oppStrongCatSprites; // strong cat sprite list

	public float GageAmount{
		get { return gageAmout;}
	}

	public bool IsPlaying{
		get { return isPlaying;}
	}
	 
	public int SelectedCharacter {
		get { return selectedCharacter;}
	}

	public int SortingOrderNumber{
		get { return sortingOrderNumber;}
		set { sortingOrderNumber = value;}
	}

	public int OppSelectedCharacter{
		get { return oppSelectedCharacter;}
		set { oppSelectedCharacter = value;}
	}

	public bool SpacePressed{
		get { return isSpacePressed;}
		set { isSpacePressed = value;}
	}

	void Awake()
	{

		gameState = GameState.Start;//delete after 
		GameStartImage=GameObject.Find("GameStartImage");
		Icon = GameObject.FindGameObjectsWithTag ("ICON"); //TODO: Delete
		endSprites=GameObject.Find("WinLose").GetComponentsInChildren<UISprite>();
		//HighlightsSet(); //TODO :
		PlayImages = GameObject.Find ("PlayImage");
		//SplashImage = GameObject.Find ("SplashImage");
		PlayWaitTimeForGame=GameObject.Find("WaitingImage");
		TutorialImage = GameObject.Find ("TutorialImage");
		timeLabel = GameObject.Find ("Time");
		gageSlider = GameObject.Find ("Gage").GetComponent<UISlider> ();
		wait3SecondsLabel = GameObject.Find ("WaitTimeLabel");
		gageSlider.GetComponent<UISlider> ().enabled = true;
		timeLabel.SetActive (false);
		PlayWaitTimeForGame.SetActive (false);
		wait3SecondsLabel.SetActive (false);
		TutorialImage.SetActive (false);
		GameStartImage.SetActive(true);
		PlayImages.SetActive (false);
		//DeactiveAllImageInSplash ();
	}



	// Use this for initialization
	void Start () {
		Game.Instance.roomManager.Test();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log (OppSelectedCharacter);

		//PhotonNetwork.SendOutgoingCommands (); //prevent local latency 
		//GetComponent<PhotonView>().RefreshRpcMonoBehaviourCache();
//		Debug.Log(PhotonNetwork.GetPing());
		switch(gameState)
		{
		case GameState.Splash:
			//SetGageValue ();
			StartCoroutine (WaitFor3Seconds ());
			//DissapearSplashImageForStart ();
	
			break;



		case GameState.Start:
			SetGageValue ();
			break;

		case GameState.Wait:
			WaitForPlayer ();
			break;

		case GameState.Tutorial:

			break;

		case GameState.Play:
			CheckWaitTime ();
			if (isPlaying) {
				TimeLabelActiveTrue ();
				UpdateOneTime ();
				SetTimeLabel ();
				SetGageValue ();
				GageValueUp ();
			}
			break;

		case GameState.End:
			AppearWinOrLose ();
			WaitFor3Seconds ();
			DissapearWinOrLose ();
			ResetForReplay ();
			DissapearPlayImage ();
			DestroyObjectsForRestart ();
			Game.Instance.roomManager.LeftRoom ();
			gameState = GameState.Start;
			break;

		default:

			break;
		}
	}

	void OnGUI()
	{
		switch(gameState)
		{
		case GameState.Splash:

			break;

		case GameState.Start:
			AppearStartImage ();
			break;

		case GameState.Wait:

			break;

		case GameState.Tutorial:

			break;

		case GameState.Play:
			Game.Instance.roomManager.OnJoinedRoom (); //Join or create room for start game
			Game.Instance.roomManager.ChangeOpponentUserPos ();
			SetPlayWaitTimeText ();
			SetGagelayer ();

			if(isPlaying)
			{
			 //manually change opp pos
			OnChangeSelectedCharacterNumberByKeyBoard (); //when i click opp. program know what i choose
			}
			break;

		case GameState.End:

			break;

		default:

			break;
		}
	}

	private void DeactiveAllImageInSplash()
	{
		GameStartImage.SetActive (false);
		TutorialImage.SetActive (false);
		timeLabel.SetActive (false); //caching timelabel
	}


	void TimeLabelActiveTrue()
	{
		if(timeLabel.GetActive()==false)
		{
			timeLabel.SetActive (true);
		}
	}

	void HighlightsSet()
	{
		for(int i=0;i<Icon.Length;i++)
		{
			iconHighlights [i] = Icon [i].GetComponent<UISprite> ();
			iconHighlights [i].enabled = false;
		}
		iconHighlights [0].enabled = true;
	}
		
	[PunRPC]
	public void SendTimeToClient(int serverTime)
	{
		time = serverTime;
	}

	void GageValueUp()
	{
		if (SpacePressed)
			GageValueIncrease ();
		else
			SetGageValueZero ();
	}

	private void WaitForPlayer()
	{
		if(PhotonNetwork.playerList.Length==1 && !isPlaying)
		{
			//TODO: Appear Wait UI 

		}
		else if(PhotonNetwork.playerList.Length==2)
		{
			if (wait3SecondsLabel.GetActive() == false)
				wait3SecondsLabel.SetActive (true);
			DissapearWaitingImage ();
			gameState = GameState.Play;
		}
	}

	private void SetPlayWaitTimeText()
	{
		if(wait3SecondsLabel.GetActive()==true)
		wait3SecondsLabel.GetComponent<UILabel> ().text = WaitTime.ToString ("0");
	}

	private void CheckWaitTime()
	{
		WaitTime -= Time.deltaTime;
		if(WaitTime<1f && !isPlaying && !Game.Instance.roomManager.IsAlignment )
		{
			if (wait3SecondsLabel.GetActive () == true)
				wait3SecondsLabel.SetActive (false);
			WaitTime = 3.5f;
			isPlaying = true;

		}
	}
		
	private void SetFalseForRestartGame()
	{
		gageAmout=0f; //how many slider value is
		oneTime = 0f; //one second for update seconds
		splashTime=3f;
		WaitTime = 3.5f;
		prevSelectedCharacter=-1; //prev selected character number;
	 	selectedCharacter = 1; //선택 된 릭터 번호 
		oppSelectedCharacter = 1; //seleted opp character number
		sortingOrderNumber=3; 
		time=180; //game time 
		isSpacePressed=false; //space pressed or not
	 	isPlaying=false;
		winState = WinState.None;
		Game.Instance.roomManager.IsConnected = false;
		Game.Instance.roomManager.IsAlignment = false;
	}

	public void OnChangeSelectedCharacterNumberByKeyBoard()
	{
		//let other players know what i choose
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			prevSelectedCharacter = selectedCharacter;
			selectedCharacter = 1;
			//IconHighlightsOnForSelectedCharacter (selectedCharacter);
			SortingOrderNumber++;
			//if(!this.GetComponent<PhotonView>().isMine)
			this.GetComponent<PhotonView>().RPC("SettingOppSelectedCharacter", PhotonTargets.Others, selectedCharacter);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2) )
		{
			prevSelectedCharacter = selectedCharacter;
			selectedCharacter = 2;
			//IconHighlightsOnForSelectedCharacter (selectedCharacter);
			SortingOrderNumber++;
			this.GetComponent<PhotonView>().RPC("SettingOppSelectedCharacter", PhotonTargets.Others, selectedCharacter);
		}
		else if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			prevSelectedCharacter = selectedCharacter;
			selectedCharacter = 3;
			//IconHighlightsOnForSelectedCharacter (selectedCharacter);
			SortingOrderNumber++;
			//if(!this.GetComponent<PhotonView>().isMine)
			this.GetComponent<PhotonView>().RPC("SettingOppSelectedCharacter", PhotonTargets.Others, selectedCharacter);
		}
	}

	void UpdateOneTime()
	{
		if (PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length == 2) {
			oneTime += Time.deltaTime;
			if (oneTime >= 1.0f) {
				//if (PhotonNetwork.isMasterClient && PhotonNetwork.playerList.Length == 2) 
				oneTime = 0;
				time -= 1;
				//ChangeTime ();
				GetComponent<PhotonView> ().RPC ("SendTimeToClient", PhotonTargets.Others, time);
			}
		}
	}

	void CheckPlayTimeForWinner()
	{
		if(time<=0 || PhotonNetwork.playerList.Length==1)
		{
			gameState = GameState.End;
		}
	}

	void AppearWinOrLose()
	{
		switch(winState)
		{
		case WinState.Win:
			endSprites [(int)WinState.Win].enabled = true;
			break;

		case WinState.Lose:
			endSprites [(int)WinState.Win].enabled = true;
			break;

		default:

			break;
		}
	}

	void DissapearWinOrLose()
	{
		switch(winState)
		{
		case WinState.Win:
			endSprites [(int)WinState.Win].enabled = false;
			break;

		case WinState.Lose:
			endSprites [(int)WinState.Win].enabled = false;
			break;

		default:

			break;
		}
	}

	void ResetForReplay()
	{
		winState = WinState.None;
		selectedCharacter = 1;
	}





	IEnumerator WaitFor3Seconds()
	{
		yield return new WaitForSeconds (3);
	}

	private void DissapearSplashImageForStart()
	{
		if(SplashImage.GetActive()==true)
		SplashImage.SetActive (false);
		AppearStartImage ();
		gameState = GameState.Start;
	}
	public void AppearPlayImage()
	{
		if (PlayImages.GetActive () == false) 
		{
			PlayImages.SetActive (true);

		}
	}

	private void SetGagelayer()
	{
		gageSlider.gameObject.layer = 11;
		gageSlider.gameObject.GetComponentInChildren<UISprite> ().gameObject.layer = 11;
	}

	public void DissapearPlayImage()
	{
		if(PlayImages.GetActive()==true)
			PlayImages.SetActive (false);
	}

	public void AppearStartImage()
	{
		if(GameStartImage.GetActive()==false)
		GameStartImage.SetActive (true);
	}

	public void DissapearStartImage()
	{
		if(GameStartImage.GetActive()==true)
			GameStartImage.SetActive (false);
	}

	public void ApeearTutorialImage()
	{
		if(TutorialImage.GetActive()==false)	
		TutorialImage.SetActive (true);
	}

	public void DissapearTutorialImage()
	{
		if(TutorialImage.GetActive()==true)	
		TutorialImage.SetActive (false);
	}

	public void AppearWaitingImage()
	{
		if (PlayWaitTimeForGame.GetActive () == false)
			PlayWaitTimeForGame.SetActive (true);
	}

	public void DissapearWaitingImage()
	{
		if (PlayWaitTimeForGame.GetActive () == true) {
			PlayWaitTimeForGame.SetActive (false);
			AppearPlayImage ();
		}
	}
		
	public void SetTimeLabel()
	{
		timeLabel.GetComponent<UILabel> ().text = (time / 60).ToString ("00")+":"+(time%60).ToString("00");
	}


	[PunRPC]
	public void SettingOppSelectedCharacter(int number)
	{
		//change what i choose character number
			Game.Instance.gameScene.OppSelectedCharacter = number;
	}

	public void SetGageValueZero()
	{
		gageAmout = 0;
	}

	 public void GageValueIncrease()
	{
		if (gageAmout >= 1)
			gageAmout = 0;

		gageAmout += 0.025f;
	}

	private void SetGageValue()
	{
		gageSlider.value = gageAmout;
	}

	public Sprite[] getOppEyeCatSprites()
	{
		return oppEyeCatSprites;
	}

	public Sprite[] getOppFastCatSprites()
	{
		return oppFastCatSprites;
	}

	public Sprite[] getOppStrongCatSprites()
	{
		return oppStrongCatSprites;
	}

	private void IconHighlightsOnForSelectedCharacter(int sel)
	{
		iconHighlights [prevSelectedCharacter-1].enabled = false;
		iconHighlights [sel-1].enabled = true;
			
	}

	private void DestroyObjectsForRestart()
	{
		GameObject[] charaters = GameObject.FindGameObjectsWithTag ("Player");
		GameObject[] bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		GameObject[] fences = GameObject.FindGameObjectsWithTag ("Fence");

		try{
			DeleteInScene(charaters);
			DeleteInScene(bullets);
			DeleteInScene(fences);
			
		}catch(Exception e)
		{
			
		}
	}

	private void DeleteInScene(GameObject[] objects)
	{
		for(int i=0;i<objects.Length;i++)
		{
			PhotonNetwork.Destroy (objects [i]);
		}
	}
}
