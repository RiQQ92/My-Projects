using UnityEngine;
using System.Collections;

public class GUI_PauseMenu : MonoBehaviour
{
	private bool pauseKeyPressed = false;

	void Awake()
	{
		if(publicStorage.refToPauser == null)
			publicStorage.refToPauser = transform;
		else
			Destroy(gameObject);
		
		gameObject.SetActive(false);
		DontDestroyOnLoad(gameObject);
	}

	void OnGUI()
	{
		if(!publicStorage.waitingInput && !publicStorage.optionsOpen)
		{
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

			GUILayout.BeginArea(new Rect(Mathf.RoundToInt(Screen.width/2 -(Screen.width/18)*1.5f),Screen.height/2 - Screen.height/16, Mathf.RoundToInt((Screen.width/9)*1.5f), Mathf.RoundToInt(Screen.height/8)*1.5f));
			GUILayout.BeginVertical("box");
			
			if(GUILayout.Button("Resume"))
			{
				// unpause the game
				pauseKeyPressed = true;
			}

			if(GUILayout.Button("Options"))
			{
				// open inGame options
				publicStorage.isInGameMenu = true;
				publicStorage.optionsOpen = true;
				gameObject.GetComponent<GUI_OptionsMenu>().enabled = true;
			}

			if(publicStorage.localGame)
			{
				if(GUILayout.Button("Exit"))
				{
					// exit to menu
					publicStorage.lvlLoaded = false;
					publicStorage.splitInstantiated = false;
					publicStorage.splitCamInstantiated = false;
					Application.LoadLevel("Menu");
					pauseKeyPressed = true;
				}
			}
			else
			{
				if(Network.isServer)
				{
					if(GUILayout.Button("Close Server"))
					{
						// shutdown the server
						transform.Find ("/NetworkManager").GetComponent<NetworkManager>().shutdownServer();
						pauseKeyPressed = true;
					}
				}
				else
				{
					if(GUILayout.Button("Disconnect"))
					{
						// disconnect client
						transform.Find ("/NetworkManager").GetComponent<NetworkManager>().closeConnection();
						pauseKeyPressed = true;
					}
				}
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	void Update()
	{
		if(pauseKeyPressed)
		{
			pauseKeyPressed = false;
			if(publicStorage.localGame)
			{
				Time.timeScale = 1;
			}

			publicStorage.gamePaused = false;
			gameObject.SetActive (false);
		}
	}
}
