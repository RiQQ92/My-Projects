using UnityEngine;
using System.Collections;


public class GUI_Script : MonoBehaviour
{
	public bool lanReceive = false;
	public Transform plrNetworkHandle;

	private GUIStyle textlineStyle;
	private GUIStyle centeredStyle;
	private GUIStyle toggleStyle;
	private GUIStyle buttonstyleCopy;
	private GUIStyle boxstyleCopy;
	private GUIStyle listButton;
	private GUIStyle listBox;

	public GUISkin listSkin;

	private int[] IPSects = new int[4];
	
	private const int defaultPort = 25000;
	private int customPort = 25000;
	private int scrResoX;
	private int scrResoY;

	private bool isFullscreen = false;
	private bool selNetwork = false;
	private bool networkMenuOpen = false;
	private bool netLocalMenuOpen = false;
	private bool netHostMenuOpen = false;

	private string customIP = "192.168.0.200";
	private string customGameName = "";
	private string playerName = "";
	private string splitOrSolo = "Singleplayer";

	private float inputMenuScale = 1.5f;
	private Vector2 inputMenuOffset = new Vector2(Screen.width/10, Screen.height/2);

	private IEnumerator attemptingConnect(float startTime)
	{
		while(Time.time < startTime+6)
		{
			if(Network.peerType == NetworkPeerType.Connecting)
			{
				Debug.Log("Connecting. . . . .");
			}
			else if(Network.peerType == NetworkPeerType.Client)
			{
				publicStorage.localGame = false;
				transform.Find("/NetworkManager").gameObject.GetComponent<NetworkView>().RPC("receivePlayerInfo", RPCMode.Server, playerName);
				Application.LoadLevel("GameLobby");
				yield return true;
			}
			else
			{
				Debug.Log("Connection Failed!");
				yield return true;
			}
		}
		Debug.Log("Connection timeout!");
		yield return true;
	}

	void Start()
	{
		if(publicStorage.Splitscreen)
			splitOrSolo = "Splitscreen";
		else
			splitOrSolo = "Singleplayer";

		customIP = publicNetworkData.getLocalIP();
		Debug.Log(customIP);
		string[] strArr = customIP.Split('.');
		
		for(int num = 0; num < 4; num++)
		{
			int.TryParse(strArr[num], out IPSects[num]);
			//= int.Parse (strArr[num]); 
			if(IPSects[num] < 0)
				IPSects[num] = 0;
			else if(IPSects[num] > 255)
				IPSects[num] = 255;
		}

		if(!publicStorage.resoSetOnStartup)
		{
			int count = 0;
			bool foundReso = false;
			Resolution[] resolutions = Screen.resolutions;
			
			foreach(Resolution res in resolutions)
			{
				print(res.width+"x"+res.height);
				count++;
			}
			
			string[] strArgs = publicStorage.getScreenSettings();
			foreach(string res in strArgs)
			{
				print(res);
			}
			scrResoX = int.Parse(strArgs[0]);
			scrResoY = int.Parse(strArgs[1]);
			isFullscreen = bool.Parse(strArgs[2]);
			
			print(scrResoX);
			print(scrResoY);
			print(isFullscreen);
			
			for(int i = 0; i < count; i++)
			{
				strArgs = (resolutions[i].width.ToString()+"x"+resolutions[i].height.ToString()).Split('x');
				if(strArgs[0] == scrResoX.ToString() && strArgs[1] == scrResoY.ToString())
				{
					print ("Found resolution match!");
					
					publicStorage.setScreenSettings(scrResoX, scrResoY, isFullscreen);
					foundReso = true;
				}
			}
			if(!foundReso)
			{
				print ("Didn't find resolution match!");
				strArgs = (resolutions[count-1].width.ToString()+"x"+resolutions[count-1].height.ToString()).Split('x');
				
				scrResoX = int.Parse(strArgs[0]);
				scrResoY = int.Parse(strArgs[1]);
				
				publicStorage.setScreenSettings(scrResoX, scrResoY, isFullscreen);
			}

			publicStorage.resoSetOnStartup = true;
		}
	}

	void OnGUI()
	{
		inputMenuOffset = new Vector2(Screen.width/20, Screen.height/2 -Screen.height/6);

		listSkin.box.fixedWidth = Mathf.RoundToInt((Screen.width/22)*inputMenuScale);
		listSkin.box.fixedHeight = 0;
		listSkin.box.fontSize = Mathf.RoundToInt((Screen.width/135)*inputMenuScale);
		//listSkin.box.padding.left = 0;
		listSkin.box.padding.left = Mathf.RoundToInt((Screen.height/49)*inputMenuScale);

		listSkin.button.fixedWidth = Mathf.RoundToInt((Screen.width/22)*inputMenuScale);
		listSkin.button.fixedHeight = Mathf.RoundToInt((Screen.height/50)*inputMenuScale);
		listSkin.button.fontSize = Mathf.RoundToInt((Screen.width/120)*inputMenuScale);


		toggleStyle = GUI.skin.GetStyle("Toggle");
		toggleStyle.fixedWidth = Mathf.RoundToInt((Screen.height/50)*inputMenuScale);
		toggleStyle.fixedHeight = Mathf.RoundToInt((Screen.height/50)*inputMenuScale);
		toggleStyle.border = new RectOffset(0,0,0,0);
		toggleStyle.overflow = new RectOffset(0,0,0,0);
		toggleStyle.imagePosition = ImagePosition.ImageOnly;
		toggleStyle.padding = new RectOffset(Mathf.RoundToInt((Screen.height/50)*inputMenuScale),0,Mathf.RoundToInt((Screen.height/50)*inputMenuScale),0);

		centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		centeredStyle.wordWrap = false;
		centeredStyle.fontSize = Mathf.RoundToInt((Screen.width/100)*inputMenuScale);
		centeredStyle.fixedHeight = Mathf.RoundToInt((Screen.height/34)*inputMenuScale);
		centeredStyle.contentOffset = new Vector2(0,0);

		textlineStyle = GUI.skin.textField;
		textlineStyle.alignment = TextAnchor.MiddleLeft;
		textlineStyle.wordWrap = false;
		textlineStyle.fontSize = Mathf.RoundToInt((Screen.width/100)*inputMenuScale);
		textlineStyle.fixedHeight = Mathf.RoundToInt((Screen.height/34)*inputMenuScale);
		textlineStyle.contentOffset = new Vector2(0,0);

		buttonstyleCopy = new GUIStyle( GUI.skin.button );
		buttonstyleCopy.alignment = TextAnchor.MiddleCenter;
		buttonstyleCopy.wordWrap = false;
		buttonstyleCopy.fixedWidth = Mathf.RoundToInt((Screen.width/16)*inputMenuScale);
		buttonstyleCopy.fixedHeight = Mathf.RoundToInt((Screen.height/34)*inputMenuScale);
		buttonstyleCopy.margin = new RectOffset(Mathf.RoundToInt((Screen.width/70)),0, Mathf.RoundToInt((Screen.height/480)*inputMenuScale), Mathf.RoundToInt((Screen.height/(Screen.height/4))));
		buttonstyleCopy.padding = new RectOffset(3,3,0,0);
		buttonstyleCopy.fontSize = Mathf.RoundToInt((Screen.width/100)*inputMenuScale);

		GUIStyle originButton = GUI.skin.GetStyle("Button");
		originButton.fontSize = Mathf.RoundToInt((Screen.width/100)*inputMenuScale);

		GUIStyle originBox = GUI.skin.GetStyle("Box");
		originBox.fontSize = Mathf.RoundToInt((Screen.width/120)*inputMenuScale);

		/*
		boxstyleCopy = new GUIStyle( GUI.skin.box );
		boxstyleCopy.alignment = TextAnchor.MiddleCenter;
		boxstyleCopy.wordWrap = false;
		boxstyleCopy.fixedWidth = 80;
		boxstyleCopy.margin = new RectOffset(24,0,4,4);
		*/
		if(!networkMenuOpen && !netLocalMenuOpen && !netHostMenuOpen)
		{
			if(!publicStorage.waitingInput && !publicStorage.optionsOpen)
			{
				GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
				// middle menu
				GUILayout.BeginArea(new Rect(Mathf.RoundToInt(Screen.width/2 -(Screen.width/18)*inputMenuScale),inputMenuOffset.y, Mathf.RoundToInt((Screen.width/9)*inputMenuScale), Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
				GUILayout.BeginVertical();
		
				if(GUILayout.Button("Play"))
				{
					transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalWorld(publicStorage.currentWorld);
				}
				if(GUILayout.Button("Change Splitscreen"))
				{
					if(publicStorage.Splitscreen)
					{
						splitOrSolo = "Singleplayer";
						publicStorage.Splitscreen = false;
					}
					else
					{
						splitOrSolo = "Splitscreen";
						publicStorage.Splitscreen = true;
					}
				}
				GUILayout.Box(splitOrSolo);
				if(GUILayout.Button("Network"))
				{
					selNetwork = !selNetwork;
				}
				if(GUILayout.Button("Options"))
				{
					// enable options menu
					publicStorage.optionsOpen = true;
					transform.GetComponent<GUI_OptionsMenu>().enabled = true;
				}
				if(GUILayout.Button("Exit"))
				{
					Application.Quit();
				}
		
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}


			if(!publicStorage.waitingInput && !publicStorage.optionsOpen && selNetwork)
			{
				GUILayout.BeginArea(new Rect(Screen.width/2 +(Screen.width/17.8f)*inputMenuScale, inputMenuOffset.y +Screen.height/12, Mathf.RoundToInt((Screen.width/9)*inputMenuScale), Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Global"))
				{
					transform.Find ("/NetworkManager").GetComponent<NetworkManager>().refreshGameList();
					networkMenuOpen = true;
					selNetwork = false;
				}
				if(GUILayout.Button("Local"))
				{
					transform.Find("/NetworkManager").GetComponent<NetworkManager>().lanReceive = true;
					transform.Find("/NetworkManager").GetComponent<NetworkManager>().StartReceivingIP();
					netLocalMenuOpen = true;
					selNetwork = false;
					InvokeRepeating("clearLANGames",0,5f);
				}
				if(GUILayout.Button("Host"))
				{
					netHostMenuOpen = true;
					selNetwork = false;
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
		else
		{
			GUI.Box(new Rect(0,Screen.height/7.6f,Screen.width, Screen.height -Screen.height/7.6f), "");
			GUI.Box(new Rect(Screen.width/10,0,Screen.width -Screen.width/10, Screen.height), "");

			if(networkMenuOpen)
			{
				GUILayout.BeginArea(new Rect(Screen.width/7.5f, Screen.height/100, Screen.width/5, Screen.height/10));
				GUILayout.BeginVertical();

				GUILayout.Box("", GUILayout.MaxWidth(Screen.width/10), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				//GUI.Label (new Rect(0,Screen.height/34*3.8f, (Screen.width/10), (Screen.height/34)*inputMenuScale), "Game Name");
				GUI.Label (GUILayoutUtility.GetLastRect(), "Player Name");
				
				if(playerName != "" && playerName != "Player Name")
					playerName = GUILayout.TextField(playerName , 25, textlineStyle);
				else
					playerName = GUILayout.TextField("Player Name", textlineStyle);
				
				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/7f+ Screen.width/5, Screen.height/100, Screen.width/5, Screen.height/9f));
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				int iTemp = 0;
				string strTemp = customIP;
				
				/*for(int i = 0; i< 4; i++)
					if(IPSects[i] == null)
						IPSects[i] = 0;*/
				
				GUILayout.Label("IP", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				customIP = GUILayout.TextField(IPSects[0].ToString ()+"."+IPSects[1].ToString ()+"."+IPSects[2].ToString ()+"."+IPSects[3].ToString (), textlineStyle);
				
				foreach(char check in customIP.ToCharArray())
				{
					if(check == '.')
						iTemp++;
				}
				if(iTemp < 3)
					customIP = strTemp;
				
				string[] strArr = customIP.Split('.');
				
				for(int num = 0; num < 4; num++)
				{
					int.TryParse(strArr[num], out IPSects[num]);
					//= int.Parse (strArr[num]); 
					if(IPSects[num] < 0)
						IPSects[num] = 0;
					else if(IPSects[num] > 255)
						IPSects[num] = 255;
				}
				
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				GUILayout.Label("Port", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				int.TryParse(GUILayout.TextField(customPort.ToString() , 6, textlineStyle), out customPort);
				
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.EndArea();
				
				GUILayout.BeginArea(new Rect(Screen.width/7f+ Screen.width/5 + Screen.width/4.8f, Screen.height/100, Screen.width/7, Screen.height/9.5f));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Direct Connect", GUILayout.ExpandHeight(true)))
				{
					Network.Connect(customIP, customPort);
					float startTime = Time.time;
					StartCoroutine(attemptingConnect(startTime));
				}
				
				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/400, Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
				GUILayout.BeginVertical();

				if(GUILayout.Button("Exit", GUILayout.ExpandHeight(true)))
				{
					networkMenuOpen = false;
				}
				if(GUILayout.Button("Refresh", GUILayout.ExpandHeight(true)))
				{
					transform.Find ("/NetworkManager").GetComponent<NetworkManager>().refreshGameList();
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/9.5f, Screen.height/7f, Screen.width -Screen.width/9.5f, Screen.height -Screen.height/7f));
				GUILayout.BeginVertical();
				if(publicNetworkData.hostData != null)
				{
					for(int i = 0; i < publicNetworkData.hostData.Length; i++)
					{
						if(GUILayout.Button(publicNetworkData.hostData[i].gameName, GUILayout.Height(Screen.height/20)) && Network.peerType == NetworkPeerType.Disconnected)
						{
							Network.Connect(publicNetworkData.hostData[i]);
							float startTime = Time.time;
							StartCoroutine(attemptingConnect(startTime));
						}
					}
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
			/*
			 * 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; LAN-MENU; 
			 * 
			 */
			else if(netLocalMenuOpen)
			{
				GUILayout.BeginArea(new Rect(Screen.width/7.5f, Screen.height/100, Screen.width/5, Screen.height/10));
				GUILayout.BeginVertical();
				
				GUILayout.Box("", GUILayout.MaxWidth(Screen.width/10), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				//GUI.Label (new Rect(0,Screen.height/34*3.8f, (Screen.width/10), (Screen.height/34)*inputMenuScale), "Game Name");
				GUI.Label (GUILayoutUtility.GetLastRect(), "Player Name");
				
				if(playerName != "" && playerName != "Player Name")
					playerName = GUILayout.TextField(playerName , 25, textlineStyle);
				else
					playerName = GUILayout.TextField("Player Name", textlineStyle);
				
				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/7f+ Screen.width/5, Screen.height/100, Screen.width/5, Screen.height/9f));
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				int iTemp = 0;
				string strTemp = customIP;
				
				/*for(int i = 0; i< 4; i++)
					if(IPSects[i] == null)
						IPSects[i] = 0;*/
				
				GUILayout.Label("IP", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				customIP = GUILayout.TextField(IPSects[0].ToString ()+"."+IPSects[1].ToString ()+"."+IPSects[2].ToString ()+"."+IPSects[3].ToString (), textlineStyle);
				
				foreach(char check in customIP.ToCharArray())
				{
					if(check == '.')
						iTemp++;
				}
				if(iTemp < 3)
					customIP = strTemp;
				
				string[] strArr = customIP.Split('.');
				
				for(int num = 0; num < 4; num++)
				{
					int.TryParse(strArr[num], out IPSects[num]);
					//= int.Parse (strArr[num]); 
					if(IPSects[num] < 0)
						IPSects[num] = 0;
					else if(IPSects[num] > 255)
						IPSects[num] = 255;
				}
				
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				GUILayout.Label("Port", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				int.TryParse(GUILayout.TextField(customPort.ToString() , 6, textlineStyle), out customPort);
				
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/7f+ Screen.width/5 + Screen.width/4.8f, Screen.height/100, Screen.width/7, Screen.height/9.5f));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Direct Connect", GUILayout.ExpandHeight(true)))
				{
					Network.Connect(customIP, customPort);
					float startTime = Time.time;
					StartCoroutine(attemptingConnect(startTime));
				}
				
				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/400, Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Exit", GUILayout.ExpandHeight(true)))
				{
					netLocalMenuOpen = false;
					transform.Find("/NetworkManager").GetComponent<NetworkManager>().lanReceive = false;
					CancelInvoke("clearLANGames");
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();

				GUILayout.BeginArea(new Rect(Screen.width/9.5f, Screen.height/7f, Screen.width -Screen.width/9.5f, Screen.height -Screen.height/7f));
				GUILayout.BeginVertical();
				if(publicNetworkData.LANGames != null)
				{
					Debug.Log("public connection to LANGames and its amount: "+publicNetworkData.LANGames.Count.ToString());
					for(int i = 0; i < publicNetworkData.LANGames.Count; i++)
					{
						if(GUILayout.Button(publicNetworkData.LANGames[i].gameName, GUILayout.Height(Screen.height/20))  && Network.peerType == NetworkPeerType.Disconnected)
						{
							Network.Connect(publicNetworkData.LANGames[i].ipAdress, publicNetworkData.LANGames[i].port);
							float startTime = Time.time;
							StartCoroutine(attemptingConnect(startTime));
						}
					}
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
			/*
			 * 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; HOST-MENU; 
			 * 
			 */
			else
			{
				GUILayout.BeginArea(new Rect(Screen.width/400, Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Exit", GUILayout.ExpandHeight(true)))
				{
					netHostMenuOpen = false;
				}
				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				
				GUILayout.BeginArea(new Rect(Screen.width -Screen.width/10 +Screen.width/400, Screen.height -Screen.height/7.6f +Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
				GUILayout.BeginVertical();
				
				if(GUILayout.Button("Host Game", GUILayout.ExpandHeight(true)))
				{
					publicStorage.localGame = false;
					transform.Find("/NetworkManager").GetComponent<NetworkManager>().startServer(customPort, customGameName, customIP);
					transform.Find("/NetworkManager").GetComponent<NetworkManager>().setHostName(playerName);
					Application.LoadLevel("GameLobby");
				}
				
				GUILayout.EndVertical();
				GUILayout.EndArea();
				
				GUILayout.BeginArea(new Rect(Screen.width/7.5f, Screen.height/7.6f +Screen.height/100, Screen.width/5, Screen.height -Screen.height/10));
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));

				int iTemp = 0;
				string strTemp = customIP;

				/*for(int i = 0; i< 4; i++)
					if(IPSects[i] == null)
						IPSects[i] = 0;*/

				GUILayout.Label("IP", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				customIP = GUILayout.TextField(IPSects[0].ToString ()+"."+IPSects[1].ToString ()+"."+IPSects[2].ToString ()+"."+IPSects[3].ToString (), textlineStyle);

				foreach(char check in customIP.ToCharArray())
				{
					if(check == '.')
						iTemp++;
				}
				if(iTemp < 3)
					customIP = strTemp;

				string[] strArr = customIP.Split('.');

				for(int num = 0; num < 4; num++)
				{
					int.TryParse(strArr[num], out IPSects[num]);
						 //= int.Parse (strArr[num]); 
					if(IPSects[num] < 0)
						IPSects[num] = 0;
					else if(IPSects[num] > 255)
						IPSects[num] = 255;
				}

				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box", GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				GUILayout.Label("Port", GUILayout.MaxWidth(Screen.width/25), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				int.TryParse(GUILayout.TextField(customPort.ToString() , 6, textlineStyle), out customPort);
				
				GUILayout.EndHorizontal();

				GUILayout.Box("", GUILayout.MaxWidth(Screen.width/10), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				//GUI.Label (new Rect(0,Screen.height/34*3.8f, (Screen.width/10), (Screen.height/34)*inputMenuScale), "Game Name");
				GUI.Label (GUILayoutUtility.GetLastRect(), "Game Name");

				if(customGameName != "" && customGameName != "Your game room's name")
					customGameName = GUILayout.TextField(customGameName , 25, textlineStyle);
				else
					customGameName = GUILayout.TextField("Your game room's name", textlineStyle);

				
				GUILayout.Box("", GUILayout.MaxWidth(Screen.width/10), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				//GUI.Label (new Rect(0,Screen.height/34*3.8f, (Screen.width/10), (Screen.height/34)*inputMenuScale), "Game Name");
				GUI.Label (GUILayoutUtility.GetLastRect(), "Player Name");
				
				if(playerName != "" && playerName != "Player Name")
					playerName = GUILayout.TextField(playerName , 25, textlineStyle);
				else
					playerName = GUILayout.TextField("Player Name", textlineStyle);

				GUILayout.BeginHorizontal("box", GUILayout.MaxWidth(Screen.width/9), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));

				GUILayout.Label("LAN-Game", GUILayout.MaxWidth(Screen.width/11), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				publicNetworkData.LANGame = GUILayout.Toggle(publicNetworkData.LANGame, "");

				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal("box", GUILayout.MaxWidth(Screen.width/9), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				
				GUILayout.Label("Cooperative", GUILayout.MaxWidth(Screen.width/11), GUILayout.MaxHeight((Screen.height/34)*inputMenuScale));
				publicNetworkData.cooperative = GUILayout.Toggle(publicNetworkData.cooperative, "");
				
				GUILayout.EndHorizontal();

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
	}
}
