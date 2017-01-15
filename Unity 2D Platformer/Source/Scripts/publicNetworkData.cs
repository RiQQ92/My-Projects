using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class publicNetworkData : MonoBehaviour
{
	public static int playerID;
	public static int hostingPort;
	public static HostData[] hostData;
	public static bool LANGame = false;
	public static bool sameUniverse;
	public static bool cooperative;
	public static bool worldAlreadyInstantiated = false;
	public static bool collideNetPlayers;

	public struct LANGameInfo
	{
		public string hostName;
		public string gameName;
		public string ipAdress;
		public int port;
	}
	
	public static List<LANGameInfo> LANGames = new List<LANGameInfo>();
	
	public static void assembleReceivedData(string data)
	{
		Debug.Log("Assembling Data!");
		string[] dataSegments = data.Split('#');
		Debug.Log(dataSegments.Length);
		for(int u = 0; u < dataSegments.Length; u++)
			Debug.Log(dataSegments[u]);
		Debug.Log(data);

		Debug.Log("ArrayList elements: "+LANGames.Count.ToString());
		if(LANGames.Count >= 1)
			Debug.Log(LANGames[0].hostName);

		if(dataSegments.Length == 4)
		{
			LANGameInfo newInfo;
			newInfo.hostName = dataSegments[0];
			newInfo.gameName = dataSegments[1];
			newInfo.ipAdress = dataSegments[2];
			newInfo.port = int.Parse(dataSegments[3]);
			if(LANGames == null)
			{
				Debug.Log("Data Storage was empty!");
				LANGames.Clear();
				LANGames.Add(newInfo);
			}
			else
			{
				bool foundSame = false;
				for(int i = 0; i < LANGames.Count; i++)
				{
					if(LANGames[i].ipAdress == newInfo.ipAdress)
						foundSame = true;
				}
				if(!foundSame)
				{
					Debug.Log("NewIPFound And Stored!");
					LANGames.Add(newInfo);
				}
			}
		}
	}

	public static string getExternalIP()
	{
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName();
		
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
		
		IPAddress[] addr = ipEntry.AddressList;
		
		return addr[addr.Length-1].ToString();
	}

	public static string getLocalIP()
	{
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
				break;
			}
		}
		return localIP;
	}
}
