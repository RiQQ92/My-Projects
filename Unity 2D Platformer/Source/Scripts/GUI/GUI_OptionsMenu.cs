using UnityEngine;
using System.Collections;

public class GUI_OptionsMenu : MonoBehaviour
{
	
	private GUIContent[] comboBoxList;
	private ComboBox comboBoxControl;// = new ComboBox();
	private GUIStyle listStyle = new GUIStyle();
	
	private GUIStyle textlineStyle;
	private GUIStyle centeredStyle;
	private GUIStyle toggleStyle;
	private GUIStyle buttonstyleCopy;
	private GUIStyle boxstyleCopy;
	private GUIStyle listButton;
	private GUIStyle listBox;
	
	public GUISkin listSkin;
	
	private string[] Axes = {"JoyX 1", "JoyY 1", "JoyX 2", "JoyY 2", "JoyX 3", "JoyY 3"};
	private string[] plr1Ctrl;
	private string[] plr2Ctrl;

	private KeyCode[] validKeyCodes;

	private int enumSize = System.Enum.GetNames(typeof(KeyCode)).Length;
	private int scrResoX;
	private int scrResoY;
	private int plrToSet = 0;
	private int btnToSet = 0;
	
	private bool audioMenuOpen = false;
	private bool graphicsMenuOpen = false;
	private bool inputMenuOpen = false; 
	private bool isFullscreen = false;

	private string keyBefore = "";
	private string keyToReturn;
	
	private float inputMenuScale = 1.5f;
	private Vector2 inputMenuOffset = new Vector2(Screen.width/10, Screen.height/2);

	private void getKeys()
	{
		plr1Ctrl = publicStorage.userCtrl1;
		plr2Ctrl = publicStorage.userCtrl2;
		
		publicStorage.UpdateKeys();
	}
	
	private void setKeys(string[] strArgs)
	{
		string keyName = strArgs[0];
		int plrNum = int.Parse(strArgs[1]);
		int btnNum = int.Parse(strArgs[2]);
		
		if(plrNum == 1)
		{
			publicStorage.userCtrl1[btnNum] = keyName;
		}
		else
		{
			publicStorage.userCtrl2[btnNum] = keyName;
		}
		
		getKeys();
	}
	
	private void waitInput(string strKey, string plrNum, string btnNum)
	{
		publicStorage.waitingInput = true;
		plrToSet = int.Parse(plrNum);
		btnToSet = int.Parse(btnNum);
		keyBefore = strKey;
	}
	
	private void clearLANGames()
	{
		publicNetworkData.LANGames.Clear();
	}

	void Awake()
	{
		publicStorage.checkUserSettings();
		getKeys();

		int count = 0;
		bool foundReso = false;
		Resolution[] resolutions = Screen.resolutions;

		count = resolutions.Length;

		string[] strArgs = publicStorage.getScreenSettings();
		scrResoX = int.Parse(strArgs[0]);
		scrResoY = int.Parse(strArgs[1]);
		isFullscreen = bool.Parse(strArgs[2]);
		
		comboBoxList = new GUIContent[count];
		for(int i = 0; i < count; i++)
		{
			comboBoxList[i] = new GUIContent(resolutions[i].width+"x"+resolutions[i].height);
		}
		listStyle.normal.textColor = Color.white; 
		listStyle.alignment = TextAnchor.MiddleCenter;
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.fontSize = Mathf.RoundToInt((Screen.width/150)*inputMenuScale);
		//listStyle.padding.left = ;
		//listStyle.padding.right =
		//listStyle.padding.top = ;
		//listStyle.padding.bottom = ;
		
		comboBoxControl = new ComboBox(new Rect(50, 100, Mathf.RoundToInt((Screen.width/22)*inputMenuScale), Mathf.RoundToInt((Screen.height/50)*inputMenuScale)), comboBoxList[0], comboBoxList, listSkin.button, listSkin.box, listStyle);
		
		//	buttonstyleCopy.fixedWidth = Mathf.RoundToInt((Screen.width/16)*inputMenuScale);
		//	buttonstyleCopy.fixedHeight = Mathf.RoundToInt((Screen.height/34)*inputMenuScale);
		for(int i = 0; i < count; i++)
		{
			strArgs = comboBoxList[i].text.Split('x');
			if(strArgs[0] == scrResoX.ToString() && strArgs[1] == scrResoY.ToString())
			{
				comboBoxControl.SelectedItemIndex = i;

				foundReso = true;
			}
		}
		if(!foundReso)
		{
			strArgs = comboBoxList[count-1].text.Split('x');
			
			comboBoxControl.SelectedItemIndex = count-1;
			
			scrResoX = int.Parse(strArgs[0]);
			scrResoY = int.Parse(strArgs[1]);
		}
		
		comboBoxControl.updateSize(new Rect(50, 100, Mathf.RoundToInt((scrResoX/22)*inputMenuScale), Mathf.RoundToInt((scrResoY/50)*inputMenuScale)));
		comboBoxControl.updateButtonContent();
	}

	void OnGUI()
	{
		inputMenuOffset = new Vector2(Screen.width/20, Screen.height/2 -Screen.height/6);
		
		listStyle.normal.textColor = Color.white; 
		listStyle.alignment = TextAnchor.MiddleCenter;
		listStyle.hover.background = new Texture2D(2, 2);
		listStyle.fontSize = Mathf.RoundToInt((Screen.width/140)*inputMenuScale);
		listStyle.fixedHeight = Mathf.RoundToInt((Screen.height/50)*inputMenuScale);
		listStyle.fixedWidth = Mathf.RoundToInt((Screen.width/22)*inputMenuScale);
		
		listSkin.box.fixedWidth = Mathf.RoundToInt((Screen.width/22)*inputMenuScale);
		listSkin.box.fixedHeight = 0;
		listSkin.box.fontSize = Mathf.RoundToInt((Screen.width/135)*inputMenuScale);
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

		GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

		if(audioMenuOpen)
		{
			
		}
		
		// if menu
		if(graphicsMenuOpen)
		{
			if(publicStorage.isInGameMenu)
				GUI.Box(new Rect(50, 100, (Mathf.RoundToInt((Screen.width/22)*inputMenuScale)) + (Mathf.RoundToInt((Screen.width/22)*inputMenuScale)), Mathf.RoundToInt((Screen.height/50)*inputMenuScale)), "");

			GUILayout.BeginArea(new Rect(50, 100 - Mathf.RoundToInt((Screen.height/50)*inputMenuScale), Mathf.RoundToInt((Screen.width/22)*inputMenuScale), Mathf.RoundToInt((Screen.height/50)*inputMenuScale)));
			GUILayout.BeginVertical();
			
			GUILayout.Box("Resolution");
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(50 + Mathf.RoundToInt((Screen.width/22)*inputMenuScale), 100, Mathf.RoundToInt((Screen.width/22)*inputMenuScale), Mathf.RoundToInt((Screen.height/50)*inputMenuScale)));
			GUILayout.BeginVertical();
			
			GUI.Box(new Rect(0, 0, Mathf.RoundToInt((Screen.width/22)*inputMenuScale), Mathf.RoundToInt((Screen.height/50)*inputMenuScale)), "Fullscreen", listSkin.box);
			isFullscreen = GUILayout.Toggle(isFullscreen, "");
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			
			comboBoxControl.Show();
		}
		
		if(!publicStorage.waitingInput)
		{
			GUILayout.BeginArea(new Rect(Mathf.RoundToInt(Screen.width/2 -(Screen.width/24)*inputMenuScale),inputMenuOffset.y, Mathf.RoundToInt((Screen.width/12)*inputMenuScale), Mathf.RoundToInt(Screen.height/6)*inputMenuScale));
			if(publicStorage.isInGameMenu)
				GUILayout.BeginVertical("box");
			else
				GUILayout.BeginVertical();

			if(GUILayout.Button("Input Settings"))
			{
				inputMenuOpen = !inputMenuOpen;
				audioMenuOpen = false;
				graphicsMenuOpen = false;
			}
			if(GUILayout.Button("Audio"))
			{
				audioMenuOpen = !audioMenuOpen;
				inputMenuOpen = false;
				graphicsMenuOpen = false;
			}
			if(GUILayout.Button("Graphics"))
			{
				graphicsMenuOpen = !graphicsMenuOpen;
				audioMenuOpen = false;
				inputMenuOpen = false;
			}
			if(GUILayout.Button("Back"))
			{
				publicStorage.isInGameMenu = false;
				publicStorage.optionsOpen = false;
				this.enabled = false;
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		
		// input menu
		if(inputMenuOpen)
		{
			if(publicStorage.isInGameMenu)
				GUI.Box(new Rect(inputMenuOffset.x, inputMenuOffset.y, Mathf.RoundToInt(Screen.width/4.55f)*inputMenuScale, (Mathf.RoundToInt(Screen.height/4)*inputMenuScale) + (Mathf.RoundToInt(Screen.height/34)*inputMenuScale)), "");

			////////////////////////////Player1
			
			// begins an area
			GUILayout.BeginArea(new Rect(inputMenuOffset.x, inputMenuOffset.y, Mathf.RoundToInt(Screen.width/27)*inputMenuScale, Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
			//begins pillar form
			GUILayout.BeginVertical();
			
			// creates text boxes
			GUILayout.Box("Player 1", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Up", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Down", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Left", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Right", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Run", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Jump", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			//GUILayout.Space(20);
			
			
			GUILayout.EndVertical();
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(inputMenuOffset.x, inputMenuOffset.y +Mathf.RoundToInt(Screen.height/4)*inputMenuScale, Mathf.RoundToInt(Screen.width/4.55f)*inputMenuScale, Mathf.RoundToInt(Screen.height/34)*inputMenuScale));
			GUILayout.BeginVertical();
			
			if(GUILayout.Button("Reset"))
			{
				publicStorage.waitingInput = false;
				publicStorage.resetUserSettings();
				getKeys();
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(inputMenuOffset.x +Mathf.RoundToInt((Screen.width/27)*inputMenuScale) -(Mathf.RoundToInt(Screen.width/16/6)), inputMenuOffset.y, (Mathf.RoundToInt(Screen.width/16)+ (Mathf.RoundToInt(Screen.width/16/3)))*inputMenuScale, Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
			GUILayout.BeginVertical();
			
			//GUILayout.BeginArea(new Rect(Screen.width/4 , Screen.height/2 -Mathf.RoundToInt(Screen.height/10), Mathf.RoundToInt(Screen.width/16)+ (Mathf.RoundToInt(Screen.width/16/3)), Mathf.RoundToInt(Screen.height/5)));
			
			GUI.Box(new Rect(Mathf.RoundToInt(Screen.width/16/6), 0, Mathf.RoundToInt(Screen.width/16)*inputMenuScale, Mathf.RoundToInt(Screen.height/34)*inputMenuScale),"Key");
			
			//GUILayout.Space(2);
			GUILayout.Label("");
			
			//creates buttons with 'if pressed - do this'
			if(GUILayout.Button(new GUIContent(plr1Ctrl[0], plr1Ctrl[0]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[0], "1", "0");
				plr1Ctrl[0] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			if(GUILayout.Button(new GUIContent(plr1Ctrl[1], plr1Ctrl[1]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[1], "1", "1");
				plr1Ctrl[1] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			if(GUILayout.Button(new GUIContent(plr1Ctrl[2], plr1Ctrl[2]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[2], "1", "2");
				plr1Ctrl[2] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			if(GUILayout.Button(new GUIContent(plr1Ctrl[3], plr1Ctrl[3]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[3], "1", "3");
				plr1Ctrl[3] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			if(GUILayout.Button(new GUIContent(plr1Ctrl[4], plr1Ctrl[4]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[4], "1", "4");
				plr1Ctrl[4] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			if(GUILayout.Button(new GUIContent(plr1Ctrl[5], plr1Ctrl[5]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr1Ctrl[5], "1", "5");
				plr1Ctrl[5] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			/////////////////////////Player2
			
			GUILayout.BeginArea(new Rect(inputMenuOffset.x +((Mathf.RoundToInt(Screen.width/10.04f)+ (Mathf.RoundToInt(Screen.width/16/3)))*inputMenuScale), inputMenuOffset.y, Mathf.RoundToInt(Screen.width/27)*inputMenuScale, Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
			
			GUILayout.BeginVertical();
			
			GUILayout.Box("Player 2", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Up", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Down", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Left", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Right", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Run", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			GUILayout.Box("Jump", GUILayout.Height(Mathf.RoundToInt((Screen.height/34)*inputMenuScale)));
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			GUILayout.BeginArea(new Rect(inputMenuOffset.x +Mathf.RoundToInt((Screen.width/7.322f)*inputMenuScale)+ (Mathf.RoundToInt(Screen.width/16/3)), inputMenuOffset.y, (Mathf.RoundToInt(Screen.width/16)+ (Mathf.RoundToInt(Screen.width/16/3)))*inputMenuScale, Mathf.RoundToInt(Screen.height/4)*inputMenuScale));
			GUILayout.BeginVertical();
			
			GUI.Box(new Rect(Mathf.RoundToInt(Screen.width/16/6), 0, Mathf.RoundToInt(Screen.width/16)*inputMenuScale, Mathf.RoundToInt(Screen.height/34)*inputMenuScale),"Key");
			
			//GUILayout.Space(Mathf.RoundToInt(Screen.height/540));
			GUILayout.Label("");
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[0], plr2Ctrl[0]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[0], "2", "0");
				plr2Ctrl[0] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[1], plr2Ctrl[1]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[1], "2", "1");
				plr2Ctrl[1] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[2], plr2Ctrl[2]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[2], "2", "2");
				plr2Ctrl[2] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[3], plr2Ctrl[3]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[3], "2", "3");
				plr2Ctrl[3] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[4], plr2Ctrl[4]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[4], "2", "4");
				plr2Ctrl[4] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			if(GUILayout.Button(new GUIContent(plr2Ctrl[5], plr2Ctrl[5]), buttonstyleCopy))
			{
				if(publicStorage.waitingInput)
				{
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				waitInput(plr2Ctrl[5], "2", "5");
				plr2Ctrl[5] = "Set key";
			}
			if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
				GUI.Label(new Rect(GUILayoutUtility.GetLastRect().position.x -Mathf.RoundToInt(Screen.width/16/6), GUILayoutUtility.GetLastRect().position.y, GUILayoutUtility.GetLastRect().size.x +Mathf.RoundToInt(Screen.width/16/3), GUILayoutUtility.GetLastRect().size.y), GUI.tooltip, centeredStyle);
			
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	void Update()
	{
		string[] strArgs = comboBoxList[comboBoxControl.SelectedItemIndex].text.Split('x');
		if(strArgs[0] != scrResoX.ToString() || strArgs[1] != scrResoY.ToString())
		{
			Debug.Log("Change Resolution");
			
			scrResoX = int.Parse(strArgs[0]);
			scrResoY = int.Parse(strArgs[1]);
			
			comboBoxControl.updateSize(new Rect(50, 100, Mathf.RoundToInt((scrResoX/22)*inputMenuScale), Mathf.RoundToInt((scrResoY/50)*inputMenuScale)));
			
			publicStorage.setScreenSettings(scrResoX, scrResoY, isFullscreen);
		}
		
		if(isFullscreen != Screen.fullScreen)
		{
			Debug.Log("Toggle Fullscreen");
			Screen.fullScreen = isFullscreen;
			publicStorage.setScreenSettings(scrResoX, scrResoY, isFullscreen);
		}
		
		if(publicStorage.waitingInput)
		{
			//int btnNum = int.Parse(strArgs[3]);
			//string key = strArgs[0];
			string key = keyBefore;
			bool isKeySet = false;
			validKeyCodes =(KeyCode[])System.Enum.GetValues(typeof(KeyCode));
			
			if(key == keyBefore && !isKeySet && publicStorage.waitingInput)
			{
				if(Input.GetKey(KeyCode.Escape))
				{
					Debug.Log("Setting Canceled");
					publicStorage.waitingInput = false;
					
					if(plrToSet == 1)
						plr1Ctrl[btnToSet] = keyBefore;
					else
						plr2Ctrl[btnToSet] = keyBefore;
				}
				
				if(publicStorage.waitingInput)
					for(int i = 0; i < enumSize; i++)
				{
					if(Input.GetKey(validKeyCodes[i]))
					{
						key = validKeyCodes[i].ToString();
						isKeySet = true;
						if(btnToSet == 0 || btnToSet == 2)
						{
							if(plr1Ctrl[btnToSet+1].Length > 4)
							{
								if(plrToSet == 1 && plr1Ctrl[btnToSet+1].Substring(0, 4) == "JoyX" || plrToSet == 1 && plr1Ctrl[btnToSet+1].Substring(0, 4) == "JoyY")
								{
									publicStorage.userCtrl1[btnToSet+1] = "None";
								}
							}
							if(plr2Ctrl[btnToSet+1].Length > 4)
							{
								if(plrToSet == 1 && plr2Ctrl[btnToSet+1].Substring(0, 4) == "JoyX" || plrToSet == 1 && plr2Ctrl[btnToSet+1].Substring(0, 4) == "JoyY")
								{
									publicStorage.userCtrl2[btnToSet+1] = "None";
								}
							}
						}
						else if(btnToSet == 1 || btnToSet == 3)
						{
							if(plr1Ctrl[btnToSet-1].Length > 4)
							{
								if(plrToSet == 1 && plr1Ctrl[btnToSet-1].Substring(0, 4) == "JoyX" || plrToSet == 1 && plr1Ctrl[btnToSet-1].Substring(0, 4) == "JoyY")
								{
									publicStorage.userCtrl1[btnToSet-1] = "None";
								}
							}
							if(plr2Ctrl[btnToSet-1].Length > 4)
							{
								if(plrToSet == 1 && plr2Ctrl[btnToSet-1].Substring(0, 4) == "JoyX" || plrToSet == 1 && plr2Ctrl[btnToSet-1].Substring(0, 4) == "JoyY")
								{
									publicStorage.userCtrl2[btnToSet-1] = "None";
								}
							}
						}
						
						if(key.Length > 5)
						{
							if(key.Substring(0, 5) == "Mouse")
							{
								key = keyBefore;
								isKeySet = false;
							}
							else
							{
								//break;
							}
						}
					}
				}
				if(publicStorage.waitingInput)
					for(int i = 0; i < 6; i++)
				{
					if(Input.GetAxis(Axes[i]) != 0)
					{
						key = Axes[i];
						isKeySet = true;
						if(btnToSet == 0 || btnToSet == 2)
						{
							if(plrToSet == 1)
							{
								publicStorage.userCtrl1[btnToSet+1] = key;
							}
							else
							{
								publicStorage.userCtrl2[btnToSet+1] = key;
							}
						}
						else if(btnToSet == 1 || btnToSet == 3)
						{
							if(plrToSet == 1)
							{
								publicStorage.userCtrl1[btnToSet-1] = key;
							}
							else
							{
								publicStorage.userCtrl2[btnToSet-1] = key;
							}
						}
						break;
					}
				}
			}
			if(isKeySet)
			{
				publicStorage.waitingInput = false;
				Debug.Log("found a way?");
				strArgs = new string[] {key, plrToSet.ToString(), btnToSet.ToString()};
				setKeys(strArgs);
			}
		}
	}
}
