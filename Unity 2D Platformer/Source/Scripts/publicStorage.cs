using UnityEngine;
using System.Collections;

// public variables, have to be public static
// can be called from anywhere using 'publicStorage.variableName'
public class publicStorage : MonoBehaviour
{
	public static bool splitInstantiated = false;
	public static bool splitCamInstantiated = false;
	public static bool lvlLoaded = false;
	public static bool upToDate = false;
	public static bool hasAxis = false;
	public static bool Splitscreen = false;
	public static bool localGame = true;
	public static bool gamePaused = false;
	public static bool waitingInput = false;
	public static bool optionsOpen = false;
	public static bool isInGameMenu = false;
	public static bool resoSetOnStartup = false;

	public static int currentLevel = 1;
	public static int currentWorld = 1;
	public static int plrsInThisWorld = 0;

	public static string[] userCtrl1 = {"UpArrow", "DownArrow", "LeftArrow", "RightArrow", "K", "L"};
	public static string[] userCtrl2 = {"W", "S", "A", "D", "G", "H"};

	public static Vector3 currentWorldPos = Vector3.zero;

	public static Transform refToNetManager;
	public static Transform refToPauser;
	public static Transform[] refToPlayers = new Transform[4];

	public static ArrayList levelsFinished = new ArrayList();

	private static string resoX;
	private static string resoY;
	private static string fullscrn;

	// resets input keys in memory and saves them
	public static void resetUserSettings()
	{
		// saves them to registry
		PlayerPrefs.SetString("Player1_up", "UpArrow");
		PlayerPrefs.SetString("Player1_down", "DownArrow");
		PlayerPrefs.SetString("Player1_left", "LeftArrow");
		PlayerPrefs.SetString("Player1_right", "RightArrow");
		PlayerPrefs.SetString("Player1_run", "K");
		PlayerPrefs.SetString("Player1_jump", "L");
		
		PlayerPrefs.SetString("Player2_up", "W");
		PlayerPrefs.SetString("Player2_down", "S");
		PlayerPrefs.SetString("Player2_left", "A");
		PlayerPrefs.SetString("Player2_right", "D");
		PlayerPrefs.SetString("Player2_run", "G");
		PlayerPrefs.SetString("Player2_jump", "H");

		// actual saving
		PlayerPrefs.Save();

		// sets current controls
		userCtrl1 = new string[] {"UpArrow", "DownArrow", "LeftArrow", "RightArrow", "K", "L"};
		userCtrl2 = new string[] {"W", "S", "A", "D", "G", "H"};
	}

	// updates keys after new key initiated
	public static void UpdateKeys()
	{
		bool hasAxisKey = false;		// is there axis button on input
		upToDate = false;				// sets current keys to be outdated

		// loop new keys to see if there is axis button
		for(int i = 0; i < 6; i++)
		{
			if(userCtrl1[i].Length > 4)
			{
				if(userCtrl1[i].Substring(0, 4) == "JoyX" || userCtrl1[i].Substring(0, 4) == "JoyY")
				{
					hasAxisKey = true;
				}
			}
			if(userCtrl2[i].Length > 4)
			{
				if(userCtrl2[i].Substring(0, 4) == "JoyX" || userCtrl2[i].Substring(0, 4) == "JoyY")
				{
					hasAxisKey = true;
				}
			}
		}

		if(hasAxisKey)
			hasAxis = true;
		else
			hasAxis = false;


		// saves input keys to memory/registry
		PlayerPrefs.SetString("Player1_up", userCtrl1[0]);
		PlayerPrefs.SetString("Player1_down", userCtrl1[1]);
		PlayerPrefs.SetString("Player1_left", userCtrl1[2]);
		PlayerPrefs.SetString("Player1_right", userCtrl1[3]);
		PlayerPrefs.SetString("Player1_run", userCtrl1[4]);
		PlayerPrefs.SetString("Player1_jump", userCtrl1[5]);

		PlayerPrefs.SetString("Player2_up", userCtrl2[0]);
		PlayerPrefs.SetString("Player2_down", userCtrl2[1]);
		PlayerPrefs.SetString("Player2_left", userCtrl2[2]);
		PlayerPrefs.SetString("Player2_right", userCtrl2[3]);
		PlayerPrefs.SetString("Player2_run", userCtrl2[4]);
		PlayerPrefs.SetString("Player2_jump", userCtrl2[5]);

		PlayerPrefs.Save();
	}

	// checks at start of program the user settings through
	public static void checkUserSettings()
	{
		// checks if player has before set settings already
		if(PlayerPrefs.HasKey("Player1_up") && PlayerPrefs.HasKey("Player1_down") && PlayerPrefs.HasKey("Player1_left") && PlayerPrefs.HasKey("Player1_right") && PlayerPrefs.HasKey("Player1_run") && PlayerPrefs.HasKey("Player1_jump") 
		&& PlayerPrefs.HasKey("Player2_up") && PlayerPrefs.HasKey("Player2_down") && PlayerPrefs.HasKey("Player2_left") && PlayerPrefs.HasKey("Player2_right") && PlayerPrefs.HasKey("Player2_run") && PlayerPrefs.HasKey("Player2_jump"))
		{
			userCtrl1[0] = PlayerPrefs.GetString("Player1_up");
			userCtrl1[1] = PlayerPrefs.GetString("Player1_down");
			userCtrl1[2] = PlayerPrefs.GetString("Player1_left");
			userCtrl1[3] = PlayerPrefs.GetString("Player1_right");
			userCtrl1[4] = PlayerPrefs.GetString("Player1_run");
			userCtrl1[5] = PlayerPrefs.GetString("Player1_jump");

			userCtrl2[0] = PlayerPrefs.GetString("Player2_up");
			userCtrl2[1] = PlayerPrefs.GetString("Player2_down");
			userCtrl2[2] = PlayerPrefs.GetString("Player2_left");
			userCtrl2[3] = PlayerPrefs.GetString("Player2_right");
			userCtrl2[4] = PlayerPrefs.GetString("Player2_run");
			userCtrl2[5] = PlayerPrefs.GetString("Player2_jump");
		}
		// othervice makes default layout for them
		else
		{
			resetUserSettings();
		}

		// checks if player already before set resolution settings
		if(PlayerPrefs.HasKey("ResolutionX") && PlayerPrefs.HasKey("ResolutionY") && PlayerPrefs.HasKey("Fullscreen"))
		{
			resoX = PlayerPrefs.GetString("ResolutionX");
			resoY = PlayerPrefs.GetString("ResolutionY");
			fullscrn = PlayerPrefs.GetString("Fullscreen");
		}
		// othervice make default layout for them
		else
		{
			PlayerPrefs.SetString("ResolutionX", Screen.currentResolution.width.ToString());
			PlayerPrefs.SetString("ResolutionY", Screen.currentResolution.height.ToString());
			PlayerPrefs.SetString("Fullscreen", Screen.fullScreen.ToString());

			resoX = Screen.currentResolution.width.ToString();
			resoY = Screen.currentResolution.height.ToString();
			fullscrn = Screen.fullScreen.ToString();
		}
	}

	// returns current screen settings
	public static string[] getScreenSettings()
	{
		print(resoX);
		print(resoY);
		print(fullscrn);
		string[] strArgs = {resoX, resoY, fullscrn};
		return(strArgs);
	}

	// sets screen resolution and fullscreen
	public static void setScreenSettings(int resWidth, int resHeight, bool isFullscreen)
	{
		Debug.Log("Save Screen Settings");

		Screen.SetResolution(resWidth, resHeight, isFullscreen);

		PlayerPrefs.SetString("ResolutionX", resWidth.ToString());
		PlayerPrefs.SetString("ResolutionY", resHeight.ToString());
		PlayerPrefs.SetString("Fullscreen", isFullscreen.ToString());
	}

	// vector adding and substracting functions
	public static Vector2 addVectors(Vector2 a, Vector2 b)
	{
		Vector2 ret = new Vector2(a.x + b.x, a.y + b.y);
		return(ret);
	}

	public static Vector3 addVectors(Vector3 a, Vector3 b)
	{
		Vector3 ret = new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		return(ret);
	}

	public static Vector4 addVectors(Vector4 a, Vector4 b)
	{
		Vector4 ret = new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		return(ret);
	}

	public static Vector2 substractVectors(Vector2 a, Vector2 b)
	{
		Vector2 ret = new Vector2(a.x - b.x, a.y - b.y);
		return(ret);
	}

	public static Vector3 substractVectors(Vector3 a, Vector3 b)
	{
		Vector3 ret = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		return(ret);
	}
	
	public static Vector4 substractVectors(Vector4 a, Vector4 b)
	{
		Vector4 ret = new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		return(ret);
	}
}
