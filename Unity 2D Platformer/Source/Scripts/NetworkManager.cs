using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

// has most of the internet functions
public class NetworkManager : MonoBehaviour
{
	public ArrayList playerInfo = new ArrayList(new string[]{"","","",""});
	public Transform plrObj;
	public Transform worldPlrObj;
	public NetworkPlayer[] playerData = new NetworkPlayer[4];
	public bool gameReady;
	public bool lanReceive;
	
	private string plrName;
	private string localGameIPAddress;
	private string localGameLobby;
	private int localGamePort;
	private int clientsDisconnected;
	private int plrsRdy;
	private string gameName = "RiQQ's ServerTest";
	private bool isRefreshing = false;
	private bool inLobby = true;

	private UdpClient sender;
	public int remotePort = 19784;
	private UdpClient receiver;

	[RPC] // server sends this rpc to clients
	private void allRdyToPlayers()
	{
		// sets game ready so it can start
		gameReady = true;
		Network.SetSendingEnabled(0, true);
	}

	[RPC] // clients send this to server when they are ready
	private void sndRdyToServer()
	{
		plrsRdy++;
		Debug.Log("Clients Ready: "+plrsRdy.ToString());
		Debug.Log("Clients Total: "+Network.connections.Length.ToString());
	}

	[RPC] // removes player from lobby and from client list
	private void removePlayerFromList(int plrNum)
	{
		//plrNum -= clientsDisconnected;
		//if(Network.isClient)
			playerInfo.RemoveAt(plrNum);
		//else
		//	playerInfo.RemoveAt(plrNum);

		if(plrNum < publicNetworkData.playerID)
		{
			if(Network.isServer)
			{
				for(int i = plrNum; i < 3; i++)
				{
					playerData[i] = playerData[i+1]; 
					//playerData[i+1] = null;
				}
			}
			publicNetworkData.playerID--;
		}
		/*
		for(int i = plrNum+1; i < 4; i++)
		{

			networkView.RPC("decreasePlrNum", RPCMode);
		}
		*/
		//networkView.RPC("updateClientList", RPCMode.Others, plrNum);
	}
	/*
	[RPC]
	private void updateClientList(int plrNum)
	{
		playerInfo.RemoveAt(plrNum);
	}
	*/

	[RPC] // receive game mode/style from host
	private void sendGameInfo(bool CoOp)
	{
		publicNetworkData.cooperative = CoOp;
	}

	[RPC] // start game call, received from server
	private void startGame()
	{
		publicStorage.lvlLoaded = false;
		gameReady = false;
		//setSendingAndListening(0, true);
		Debug.Log ("wait for level");
		Application.LoadLevel("World1");
		StartCoroutine(waitForWorld(false));
	}

	[RPC] // starts level at inet play, sent by server
	private void startLevel(int index)
	{
		publicStorage.currentLevel = index;
		string levelToLoad = "Level"+index.ToString();
		publicStorage.lvlLoaded = false;
		gameReady = false;
		Debug.Log ("wait for level");
		Application.LoadLevel(levelToLoad);
		StartCoroutine(waitForLevel());
	}

	[RPC] // starts world at inet play, sent by server
	private void startWorld(int index, bool finishedLevel)
	{
		publicStorage.currentWorld = index;
		string levelToLoad = "World"+index.ToString();
		publicStorage.lvlLoaded = false;
		gameReady = false;
		Debug.Log ("wait for world");
		Application.LoadLevel(levelToLoad);
		StartCoroutine(waitForWorld(finishedLevel));
	}

	[RPC] // disconnects player from the server
	private void disconnectFromServer()
	{
		publicStorage.localGame = true;
		Debug.Log("Client Disconnected");
		Application.LoadLevel("Menu");
		if(Network.connections.Length > 0)
			Network.CloseConnection(Network.connections[0], true);
	}

	[RPC] // server receivers joining clients player name
	private void receivePlayerInfo(string playerName, NetworkMessageInfo info)
	{
		Debug.Log(playerInfo.Count);
		Debug.Log(playerInfo.Capacity);
		if(playerInfo.Count < playerInfo.Capacity)
			playerInfo.Add(playerName);
		else
			Debug.Log("Names Wont fit to this array!");

		playerData[playerInfo.Count-1] = info.sender;
		networkView.RPC("returnPlayerNum", info.sender, playerInfo.Count);
		passPlayerInfoToClients();
	}
	/*
	[RPC] 
	private void initCompetitivePlayer(Vector3 pos, string name)
	{
		Transform t = (Transform) Network.Instantiate(plrObj, pos, Quaternion.identity, 0);
		GameObject g = t.gameObject;
		g.name = name;
	}
	*/

	[RPC] // returns players ID number
	private void returnPlayerNum(int plrID)
	{
		publicNetworkData.playerID = plrID;
	}

	[RPC] // receives other players info at current server
	private void clientReceivePlrInfo(string toCatch)
	{
		string[]plrInfo = toCatch.Split('#');
		playerInfo.Clear();
		for(int i = 0; i < plrInfo.Length; i++)
			playerInfo.Add(plrInfo[i]);
	}

	[RPC] // received from server, excact player data with IP's and everything
	private void sharePlrData(NetworkPlayer dat1, NetworkPlayer dat2, NetworkPlayer dat3, NetworkPlayer dat4)
	{
		playerData[0] = dat1;
		playerData[1] = dat2;
		playerData[2] = dat3;
		playerData[3] = dat4;
	}

	// restarts sending and receiving info after a while
	public IEnumerator returnCommunicationsAfter(float seconds)
	{
		yield return new WaitForSeconds(seconds);

		setSendingAndListening(0, true);

		yield return true;
	}

	// waits for player to get disconnected and then returns to menu
	private IEnumerator waitForDisconnect()
	{
		while(Network.peerType != NetworkPeerType.Disconnected)
		{
			yield return new WaitForEndOfFrame();
		}
		Application.LoadLevel("Menu");
		yield return true;
	}

	// level player initialization
	private IEnumerator waitForLevel()
	{

		float start = Time.time;
		while(Application.isLoadingLevel && Time.time <= start + 10)
		{
			yield return new WaitForFixedUpdate();
			Debug.Log("Loading. . . . ");
			if(Time.time >= start+10)
			{
				Debug.LogError("Loading TimedOut, Shutting Down!...");
				Network.Disconnect();
				yield return new WaitForSeconds(3);
				Application.Quit();
			}
		}
		Vector3 spwnPoint = transform.Find ("/Spawn").position;
		if(!publicStorage.localGame)
		{
			if(publicNetworkData.cooperative)
			{
				//Transform t = (Transform) Network.Instantiate(plrObj, new Vector3(spwnPoint.x + 1* int.Parse(Network.player.ToString()), spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
				Transform t = (Transform) Network.Instantiate(plrObj, new Vector3(spwnPoint.x + 1* publicNetworkData.playerID, spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
				GameObject g = t.gameObject;
				//g.name = "Player"+(int.Parse(Network.player.ToString())+1).ToString();
				g.name = "Player"+publicNetworkData.playerID.ToString();
			}
			else
			{
				Transform t = (Transform) Network.Instantiate(plrObj, new Vector3(spwnPoint.x + 1, spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
				GameObject g = t.gameObject;
				//g.name = "Player"+(int.Parse(Network.player.ToString())+1).ToString();
				g.name = "Player"+publicNetworkData.playerID.ToString();
				g.GetComponent<NetworkView>().RPC("setWorld", RPCMode.All, publicStorage.currentWorld);
				g.GetComponent<NetworkView>().RPC("setLevel", RPCMode.All, publicStorage.currentLevel);
				g.GetComponent<NetworkView>().RPC("checkNetWorldLevel", RPCMode.All, publicNetworkData.playerID);
			}
		}
		else
		{
			Transform t = (Transform) Instantiate(plrObj, new Vector3(spwnPoint.x, spwnPoint.y, spwnPoint.z), Quaternion.identity);
			GameObject g = t.gameObject;
			g.name = "Player1";
		}

		inLobby = false;

		if(Network.isServer)
		{
			// yield return new WaitForSeconds(1);
			float time = Time.time;
			while(!gameReady && Time.time < time + 10)
			{
				yield return new WaitForEndOfFrame();
				Debug.Log("Am I Endless?");
			}
			while(transform.Find("/Player(Clone)"))
			{
				int idNum = 2;
				while(transform.Find("/Player(Clone)").networkView.owner != playerData[idNum-1])
				{
					idNum++;
					if(idNum > 4)
						break;
				}
				Transform obj = transform.Find("/Player(Clone)");
				//obj.name = "Player" + (int.Parse(obj.GetComponent<NetworkView>().owner.ToString())+1).ToString();
				obj.name = "Player" + idNum.ToString();
			}
		}
		else if(Network.isClient)
			networkView.RPC("sndRdyToServer", RPCMode.Server);

		publicStorage.lvlLoaded = true;

		yield return true;
	}

	// world player initialization
	private IEnumerator waitForWorld(bool lvlFinished)
	{
		publicStorage.currentLevel = 0;
		float start = Time.time;
		while(Application.isLoadingLevel && Time.time <= start + 10)
		{
			yield return new WaitForFixedUpdate();
			Debug.Log("Loading. . . . ");
			if(Time.time >= start+10)
			{
				Debug.LogError("Loading TimedOut, Shutting Down!...");
				Network.Disconnect();
				yield return new WaitForSeconds(3);
				Application.Quit();
			}
		}
		Vector3 spwnPoint;
		if(publicStorage.currentWorldPos == Vector3.zero)
			spwnPoint = transform.Find ("/Spawn").position;
		else
			spwnPoint = publicStorage.currentWorldPos;

		if(!publicStorage.localGame)
		{
			if(publicNetworkData.cooperative)
			{
				if(Network.isServer)
				{
					if(!publicNetworkData.worldAlreadyInstantiated)
					{
						//Transform t = (Transform) Network.Instantiate(plrObj, new Vector3(spwnPoint.x + 1* int.Parse(Network.player.ToString()), spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
						Transform t = (Transform) Network.Instantiate(worldPlrObj, new Vector3(spwnPoint.x, spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
						GameObject g = t.gameObject;
						//g.name = "Player"+(int.Parse(Network.player.ToString())+1).ToString();
						g.name = "plrWorld"+publicNetworkData.playerID.ToString();
						publicNetworkData.worldAlreadyInstantiated = true;
					}
					else
					{
						// activate this worlds players
						foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
						{
							if(go.name.Contains("plrWorld"))
							{
								go.renderer.enabled = true;
								for( int i = 0; i < go.transform.childCount; ++i )
								{
									go.transform.GetChild(i).gameObject.SetActive(true);
								} 
								go.GetComponent<selectorControls>().enabled = true;
								
								if(go.networkView.isMine)
									if(lvlFinished)
										go.GetComponent<selectorControls>().setLvlFinished();
							}
						}
					}
				}
			}
			else
			{
				if(!publicNetworkData.worldAlreadyInstantiated)
				{
					//Transform t = (Transform) Network.Instantiate(plrObj, new Vector3(spwnPoint.x + 1* int.Parse(Network.player.ToString()), spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
					Transform t = (Transform) Network.Instantiate(worldPlrObj, new Vector3(spwnPoint.x, spwnPoint.y, spwnPoint.z), Quaternion.identity, 0);
					GameObject g = t.gameObject;
					//g.name = "Player"+(int.Parse(Network.player.ToString())+1).ToString();
					g.name = "plrWorld"+publicNetworkData.playerID.ToString();
					publicNetworkData.worldAlreadyInstantiated = true;
				}
				else
				{
					// activate this worlds players
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
					{
						if(go.name.Contains("plrWorld"))
						{
							go.renderer.enabled = true;
							for( int i = 0; i < go.transform.childCount; ++i )
							{
								go.transform.GetChild(i).gameObject.SetActive(true);
							} 
							go.GetComponent<selectorControls>().enabled = true;
							
							if(go.networkView.isMine)
								if(lvlFinished)
									go.GetComponent<selectorControls>().setLvlFinished();
						}
					}

					// disable level players
					foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
					{
						if(go.name.Contains("Player"))
						{
							go.renderer.enabled = false;
							go.collider2D.enabled = false;
							go.GetComponent<Animator>().enabled = false;
							go.rigidbody2D.isKinematic = true;
							for( int i = 0; i < go.transform.childCount; ++i )
							{
								go.transform.GetChild(i).gameObject.SetActive(false);
							} 
						}
					}
				}
			}
		}
		else
		{
			if(!publicNetworkData.worldAlreadyInstantiated)
			{
				Transform t = (Transform) Instantiate(worldPlrObj, new Vector3(spwnPoint.x, spwnPoint.y, spwnPoint.z), Quaternion.identity);
				GameObject g = t.gameObject;
				g.name = "plrWorld1";
				publicNetworkData.worldAlreadyInstantiated = true;
			}
			else
			{
				// activate this worlds players
				foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
				{
					if(go.name.Contains("plrWorld"))
					{
						go.renderer.enabled = true;
						for( int i = 0; i < go.transform.childCount; ++i )
						{
							go.transform.GetChild(i).gameObject.SetActive(true);
						} 
						go.GetComponent<selectorControls>().enabled = true;

						if(lvlFinished)
							go.GetComponent<selectorControls>().setLvlFinished();
					}
				}
			}
		}
		
		inLobby = false;
		
		if(Network.isServer)
		{
			// yield return new WaitForSeconds(1);
			float time = Time.time;
			while(!gameReady && Time.time < time + 10)
			{
				yield return new WaitForEndOfFrame();
				Debug.Log("Am I Endless?");
			}
			while(transform.Find("/plrWorld(Clone)"))
			{
				int idNum = 2;
				while(transform.Find("/plrWorld(Clone)").networkView.owner != playerData[idNum-1])
				{
					idNum++;
					if(idNum > 4)
						break;
				}
				Transform obj = transform.Find("/plrWorld(Clone)");
				//obj.name = "Player" + (int.Parse(obj.GetComponent<NetworkView>().owner.ToString())+1).ToString();
				obj.name = "plrWorld" + idNum.ToString();
			}
		}
		else if(Network.isClient)
			networkView.RPC("sndRdyToServer", RPCMode.Server);
		
		yield return true;
	}

	// server function, sends player info to clients
	private void passPlayerInfoToClients()
	{
		string toSend = "";
		for(int i = 0; i < playerInfo.Count; i++)
			toSend += playerInfo[i].ToString()+"#";
		
		toSend.TrimEnd("#"[0]);
		networkView.RPC("clientReceivePlrInfo", RPCMode.Others, toSend);
	}

	// server function passes player data to clients
	public void passPlrData()
	{
		networkView.RPC("sharePlrData", RPCMode.Others, playerData[0], playerData[1], playerData[2], playerData[3]);
	}

	// allaround function, stops sending and listening of network messages
	public void setSendingAndListening(int group, bool onOrOff)
	{
		Network.SetReceivingEnabled(Network.player, group, onOrOff);
		Network.SetSendingEnabled(group, onOrOff);
	}

	// local play start world
	public void startLocalWorld(int index)
	{
		publicStorage.currentWorld = index;
		string levelToLoad = "World"+index.ToString();
		Debug.Log ("wait for world");
		Application.LoadLevel(levelToLoad);
		StartCoroutine(waitForWorld(false));
	}

	// local play start world
	public void startLocalWorld(int index, bool finishedLevel)
	{
		publicStorage.currentWorld = index;
		string levelToLoad = "World"+index.ToString();
		Debug.Log ("wait for world");
		Application.LoadLevel(levelToLoad);
		StartCoroutine(waitForWorld(finishedLevel));
	}

	// local play start level
	public void startLocalLevel(int index)
	{
		publicStorage.currentLevel = index;
		string levelToLoad = "Level"+index.ToString();
		Debug.Log ("wait for level");
		Application.LoadLevel(levelToLoad);
		StartCoroutine(waitForLevel());
	}

	// sets hosters name
	public void setHostName(string name)
	{
		playerData[0] = Network.player;
		publicNetworkData.playerID = 1;
		playerInfo.Clear();
		playerInfo.Add (name);
	}

	// closes connection to server
	public void closeConnection()
	{
		if(Network.connections.Length > 0)
		{
			publicStorage.localGame = true;
			Network.CloseConnection(Network.connections[0], true);
		}
		Network.Disconnect();
		StartCoroutine(waitForDisconnect());
	}

	// shutsdown the server and disconnects clients
	public void shutdownServer()
	{
		plrsRdy = 0;
		clientsDisconnected = 0;
		publicStorage.localGame = true;
		MasterServer.UnregisterHost();
		networkView.RPC("disconnectFromServer", RPCMode.OthersBuffered);
		Network.Disconnect();
		StartCoroutine(waitForDisconnect());
	}

	// creates server
	public void startServer(int port, string lobbyName, string localIP)
	{
		Debug.Log("Starting server");
		Network.InitializeServer(3, port, !Network.HavePublicAddress());
		publicNetworkData.hostingPort = port;

		if(!publicNetworkData.LANGame)
		{
			MasterServer.RegisterHost(gameName, lobbyName, "Testing Server");
		}
		else
		{
			localGameIPAddress = localIP;
			localGamePort = port;
			localGameLobby = lobbyName;

			sender = new UdpClient (remotePort, AddressFamily.InterNetwork);
			IPEndPoint groupEP = new IPEndPoint (IPAddress.Broadcast, remotePort);
			sender.Connect (groupEP);

			Debug.Log("GUID: "+Network.player.guid);
			//SendData ();
			InvokeRepeating("SendData",0,0.5f);
		}
	}

	// actually sets the player names on current level
	public void getPlayerNames()
	{
		while(transform.Find("/Player(Clone)"))
		{
			int idNum = 2;
			while(transform.Find("/Player(Clone)").networkView.owner != playerData[idNum-1])
			{
				idNum++;
				if(idNum > 4)
					break;
			}
			Transform obj = transform.Find("/Player(Clone)");
			//obj.name = "Player" + (int.Parse(obj.GetComponent<NetworkView>().owner.ToString())+1).ToString();
			obj.name = "Player" + idNum.ToString();
		}
	}

	// refreshes master server gamelist
	public void refreshGameList()
	{
		publicNetworkData.hostData = null;
		MasterServer.RequestHostList(gameName);
		isRefreshing = true;
	}

	// stops LAN UDP-Broadcasting
	public void stopSending()
	{
		sender.Close();
		CancelInvoke("SendData");
	}

	// starts listening to LAN UDP-Broadcasting
	public void StartReceivingIP ()
	{
		Debug.Log("Started Receiving IP's");
		try
		{
			if (receiver == null)
			{
				receiver = new UdpClient (remotePort);
				receiver.BeginReceive(new System.AsyncCallback (ReceiveData), null);
			}
		}
		catch (SocketException e)
		{
			Debug.Log (e.Message);
		}
	}

	// when receives data through LAN UDP-Broadcasting -> parse it through
	private void ReceiveData (System.IAsyncResult result)
	{
		IPEndPoint receiveIPGroup = new IPEndPoint (IPAddress.Any, remotePort);
		byte[] received;
		if (receiver != null)
		{
			Debug.Log("SomeData Received!");
			received = receiver.EndReceive (result, ref receiveIPGroup);
			if(!lanReceive)
			{
				Debug.Log("LAN CLOSED");
				return;
			}
		}
		else
		{
			Debug.Log("NoData Received!");
			return;
		}
		Debug.Log("SendData Forward!");
		receiver.BeginReceive (new System.AsyncCallback (ReceiveData), null);
		string receivedString = Encoding.ASCII.GetString (received);
		publicNetworkData.assembleReceivedData(receivedString);
	}

	// sends data through LAN UDP-Broadcasting
	private void SendData ()
	{
		string customMessage = playerInfo[0].ToString()+"#"+localGameLobby+"#"+localGameIPAddress+"#"+localGamePort.ToString();
		
		if (customMessage != "")
		{
			sender.Send (Encoding.ASCII.GetBytes (customMessage), customMessage.Length);
		}
	}

	// when application quits, make sure the network games are disconnected first
	void OnApplicationQuit()
	{
		if(!publicStorage.localGame)
		{
			Application.CancelQuit();
			if(Network.isServer)
			{
				shutdownServer();
			}
			else
			{
				closeConnection();
			}
			Application.Quit();
		}
	}

	// when player gets disconnected, remove all leftover data from him
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		clientsDisconnected++;
		Debug.Log("playerDisconnected");
		if(Network.isServer)
			Debug.Log("By Server");
		else
			Debug.Log("By Client");
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	// when server has been Initialized
	void OnServerInitialized()
	{
		Debug.Log("Server Initialized!");
	}

	// when server is being connected by client
	void OnServerConnected()
	{
		// send player name
		//set boolean connected to true to make sure we have connection
	}

	// when master server does something (event)
	void OnMasterServerEvent(MasterServerEvent mse)
	{
		if(mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registeration was Succesful!");
		}
	}

	// NetworkManager Initialization
	void Start ()
	{
		Network.minimumAllocatableViewIDs = 1000;
		clientsDisconnected = 0;
		DontDestroyOnLoad(gameObject);
		playerInfo.Capacity = 4;

		if(publicStorage.refToNetManager == null)
			publicStorage.refToNetManager = transform;
		else
			Destroy(gameObject);
	}

	// every frame Update function
	void Update ()
	{
		if(isRefreshing)
		{
			if(MasterServer.PollHostList().Length > 0)
			{
				Debug.Log (MasterServer.PollHostList().Length);
				isRefreshing = false;
				publicNetworkData.hostData = MasterServer.PollHostList();
			}
		}
		if(!publicStorage.localGame && Network.isServer)
		{
			if(plrsRdy >= Network.connections.Length && !gameReady && !inLobby)
			{
				Debug.Log("Game Ready");
				networkView.RPC("allRdyToPlayers", RPCMode.Others);
				plrsRdy = 0;
				gameReady = true;
				inLobby = true;
			}
		}
	}
}
