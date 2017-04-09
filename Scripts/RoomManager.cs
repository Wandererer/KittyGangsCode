using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Photon.MonoBehaviour {

	private string verNum = "1.0"; //버젼 정보
    private string roomName = "room01"; //방 이름
	private bool isConnected=false; //연결 상태 
	private bool isAllignment=false; //정렬 상태 

	private string[] roomNames = {"room01", "room02", "room03", "room04", "room05", "room06", "room07", "room08", "room09", "room10",
		"room11", "room12", "room13", "room14", "room15", "room16", "room17", "room18", "room19", "room20"
	}; //방 정보 

	private string[] katNames = {"FastCat" , "EyeCat", "MusleCat" }; //각 캐릭터 이름 생성용
	private Vector3[] instantPos = { new Vector3 (0.963f,-0.169f,0), new Vector3 (0.461f,-0.482f,0), new Vector3 (-0.095f,-0.733f,0) }; //내 캐릭터 위치
	private Vector3[] oppPos = { new Vector3 (-0.963f,0.169f,0), new Vector3 (-0.461f,0.482f,0), new Vector3 (0.095f,0.735f,0) }; //상대 캐릭터 위치
	private Vector3[] myTrashAreaPos={new Vector3(0.951f,-0.789f),new Vector3(1.066f,-0.654f)};
	private Vector3[] oppTrashAreaPos={new Vector3(0.941f,0.788f),new Vector3(1.066f,-0.654f)};

	public bool IsConnected{
		get {return isConnected;}
		set {isConnected = value;}
	}

	public bool IsAlignment{
		get {return isAllignment;}
		set {isAllignment = value;}
	}

	RoomOptions roomOptions; //방 옵션 

	// Use this for initialization
	 public RoomManager () {
        //첫 시작 세
		roomOptions= new RoomOptions();
        PhotonNetwork.ConnectUsingSettings (verNum);//버젼 정보가 다르면 접근 불가
		roomOptions.MaxPlayers=2; //방 옵션으로 최대 플레이어 2명으로 지정
		PhotonNetwork.sendRate=30;
        Debug.Log ("Starting Connection!");
    }

	public void Test()
	{
		
	}

    public void OnJoinedLobby()
    {
		PhotonNetwork.offlineMode = false;
       PhotonNetwork.JoinOrCreateRoom(roomName,roomOptions,TypedLobby.Default);
		//PhotonNetwork.automaticallySyncScene = true;
		//PhotonNetwork.UseRpcMonoBehaviourCache = true;
		//방을 만들거나 참가
        Debug.Log ("Starting Server!");
    }

    public void OnJoinedRoom()
	{

		if(PhotonNetwork.playerList.Length == 2 && !isConnected)
        {
			//Debug.Log ("TEST");
			isConnected = true;
			//2명이 다 들어 왔을 경우 시작 

			for (int i = 0; i < katNames.Length; i++) {
				GameObject obj=PhotonNetwork.Instantiate(katNames[i],instantPos[i] ,Quaternion.identity ,0) as GameObject; //네트워크 상 생성

				if(obj.GetComponent<Transform>().parent!=null)
				{
					Debug.Log ("parent");
					obj.GetComponent<Transform>().parent = null;
				}

				obj.GetComponent<CharacterController> ().enabled = true; //각 플레이어 마다 컨트롤 하기 위해 다시 켬 안 할 경우 동시에 움직 임
				obj.GetComponent<Character>().enabled=true;
			}

			isAllignment=true;

		}

		/*
			if(PhotonNetwork.isMasterClient)
				~~~
			else
				~~~
				마스터 냐 아니냐에 따라 생성 위치 다르게 .....
		*/
    }

	public void ChangeOpponentUserPos()
	{
		if (PhotonNetwork.playerList.Length == 2 && isAllignment) {

			GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");//캐릭터 개수 알아보기 위해

			if (objects.Length == 6) 
			{
				//TODO: 개수가 2개 일 경우에만 .. 이후 에 바꿔야 함 

				foreach(GameObject obj in objects)
				{

					if(!obj.GetComponent<PhotonView>().isMine)
					{
						//내 뷰가 아니면 위치 변경 
						int charNum=obj.GetComponent<CharacterController>().CharacterNumber;
						ChangeOppPosByCharNumber (charNum, obj);
					}
					else 
					{
						//if this is my view change pos for mine
						int charNum=obj.GetComponent<CharacterController>().CharacterNumber;
						ChangeMyCharPosByNumber (charNum, obj);
					}
				}
				CharacterController[] players = GameObject.Find ("UI Root").GetComponentsInChildren<CharacterController>(); //if character is still in uiroot 

				if (players.Length != 0)
					isAllignment = true;
				else
					isAllignment = false; //다음에는 이거 실행 안되도록 정렬 완료 
			}
		}
	}

	public void LeftRoom()
	{
		PhotonNetwork.LeaveRoom ();
	}

	private void ChangeMyCharPosByNumber(int number,GameObject obj)
	{
		switch(number)
		{
		case 1:
			//obj.GetComponent<Transform> ().GetChild (0).GetComponent<Transform> ().localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			obj.GetComponent<Transform> ().GetChild (0).GetComponent<Transform> ().localPosition = new Vector3 (-0.2f, 0.23f, 0);
			obj.GetComponent<Character> ().Type = SideType.Mine;
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().localPosition = instantPos [0];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
			//obj.GetComponent<Transform>().GetChild(0).
			break;

		case 2:
			//obj.GetComponent<Transform> ().GetChild (1).GetComponent<Transform> ().localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			obj.GetComponent<Transform> ().GetChild (1).GetComponent<Transform> ().localPosition=new Vector3(-0.2f,0.23f,0);
			obj.GetComponent<Character> ().Type = SideType.Mine;
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().position = instantPos [1];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);

			break;

		case 3:
			//obj.GetComponent<Transform> ().GetChild (0).GetComponent<Transform> ().localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			obj.GetComponent<Transform> ().GetChild (0).GetComponent<Transform> ().localPosition=new Vector3(-0.2f,0.23f,0);
			obj.GetComponent<Character> ().Type = SideType.Mine;
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().position = instantPos [2];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
			break;

		default:

			break;
		}
	}
		
	private void ChangeOppPosByCharNumber(int number, GameObject obj)
	{
		//change opp pos by character number
		switch(number)
		{
		case 1:
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().localPosition = oppPos [0];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
			obj.GetComponent<Transform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			obj.GetComponent<Character> ().Type = SideType.Opp;
			obj.GetComponent<Character> ().SetCharacterSprites(Game.Instance.gameScene.getOppFastCatSprites());
			break;

		case 2:
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().position = oppPos [1];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
			obj.GetComponent<Transform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			obj.GetComponent<Character> ().Type = SideType.Opp;
			obj.GetComponent<Character> ().SetCharacterSprites(Game.Instance.gameScene.getOppEyeCatSprites());
			break;

		case 3:
			obj.GetComponent<Transform> ().parent = null;
			obj.GetComponent<Transform> ().position = oppPos [2];
			obj.GetComponent<Transform> ().localScale = new Vector3 (1, 1, 1);
			obj.GetComponent<Transform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			obj.GetComponent<Character> ().Type = SideType.Opp;
			obj.GetComponent<Character> ().SetCharacterSprites (Game.Instance.gameScene.getOppStrongCatSprites());
			break;

		default:

			break;
		}
	}



}
