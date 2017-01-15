using UnityEngine;
using System.Collections;

public class GUI_GameLobby : MonoBehaviour
{
	/*
	[RPC]
	private void disconnectFromServer()
	{
		publicStorage.localGame = true;
		Debug.Log("Client Disconnected");
		Application.LoadLevel("Menu");
		if(Network.connections.Length > 0)
			Network.CloseConnection(Network.connections[0], true);
	}
   	*/
	private string localIP;
	private string externalIP;

	private float inputMenuScale;
	
	private GUIStyle centeredStyle;

	void Start()
	{
		inputMenuScale = 1.5f;
		localIP = publicNetworkData.getLocalIP();
		externalIP = publicNetworkData.getExternalIP();
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width/9.5f, Screen.height/7f, Screen.width -Screen.width/4.75f, Screen.height -Screen.height/3.5f));
		GUILayout.BeginVertical();

		if(Network.isServer)
			for(int i = 0; i < transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo.Count; i++)
			{
				if(transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo.Count > i)
					GUILayout.Box(transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo[i].ToString(), GUILayout.Height(Screen.height/20));
			}
		else
			for(int i = 0; i < transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo.Count -1; i++)
			{
				if(transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo.Count > i)
					GUILayout.Box(transform.Find ("/NetworkManager").GetComponent<NetworkManager>().playerInfo[i].ToString(), GUILayout.Height(Screen.height/20));
			}

		GUILayout.EndVertical();
		GUILayout.EndArea();

		if(Network.isServer)
		{
			centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginArea(new Rect(Screen.width/200, Screen.height/100, Screen.width/5f, Screen.height -Screen.height/7.6f -Screen.height/10));
			GUILayout.BeginVertical("box");

			GUILayout.Label(" External IP: "+externalIP, GUILayout.MaxWidth(Screen.width/5), GUILayout.MaxHeight((Screen.height/38)*inputMenuScale));
			GUILayout.Label(" Local     IP: "+localIP, GUILayout.MaxWidth(Screen.width/5), GUILayout.MaxHeight((Screen.height/38)*inputMenuScale));
			GUILayout.Label(" Port          : "+publicNetworkData.hostingPort.ToString(), GUILayout.MaxWidth(Screen.width/5), GUILayout.MaxHeight((Screen.height/38)*inputMenuScale));
			
			GUILayout.EndVertical();
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(Screen.width -Screen.width/10 +Screen.width/400, Screen.height -Screen.height/7.6f +Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
			GUILayout.BeginVertical();
			
			if(GUILayout.Button("Start", GUILayout.ExpandHeight(true)))
			{
				if(publicNetworkData.LANGame)
					transform.Find ("/NetworkManager").GetComponent<NetworkManager>().stopSending();
				//StartCoroutine(); to start with timer
				transform.Find ("/NetworkManager").GetComponent<NetworkManager>().passPlrData();
				transform.Find ("/NetworkManager").GetComponent<NetworkView>().RPC("sendGameInfo", RPCMode.Others, publicNetworkData.cooperative);
				transform.Find ("/NetworkManager").GetComponent<NetworkView>().RPC("startGame", RPCMode.All);
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		GUILayout.BeginArea(new Rect(Screen.width/400, Screen.height -Screen.height/7.6f +Screen.height/200, Screen.width/10 -Screen.width/200, Screen.height/7.6f -Screen.height/100));
		GUILayout.BeginVertical();
		
		if(GUILayout.Button("Exit", GUILayout.ExpandHeight(true)))
		{
			if(Network.isServer)
			{
				if(publicNetworkData.LANGame)
					transform.Find ("/NetworkManager").GetComponent<NetworkManager>().stopSending();

				transform.Find ("/NetworkManager").GetComponent<NetworkManager>().shutdownServer();
			}
			else
			{
				//transform.Find ("/NetworkManager").GetComponent<NetworkView>().RPC("removePlayerFromList", RPCMode.Others, (int.Parse(Network.player.ToString())));
				transform.Find ("/NetworkManager").GetComponent<NetworkView>().RPC("removePlayerFromList", RPCMode.Others, publicNetworkData.playerID-1);
				transform.Find ("/NetworkManager").GetComponent<NetworkManager>().closeConnection();
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
