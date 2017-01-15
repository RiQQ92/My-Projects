using UnityEngine;
using System.Collections;

public class selectorControls : MonoBehaviour
{
	private bool expectingLeftSide;
	private bool expectingRightSide;
	private bool expectingLevel;
	private bool isOnMove;

	private bool jumpBtnIsPressed;
	private bool runBtnIsPressed;
	private bool upPressed;
	private bool downPressed;
	private bool leftPressed;
	private bool rightPressed;
	private bool[] btnIsAxis = new bool[6];
	
	private string key_jump;
	private string key_run;
	private string key_up;
	private string key_down;
	private string key_left;
	private string key_right;

	private Vector2 lastMoveDir;
	private Vector2 moveDir;
	private Collider2D[] colResults = new Collider2D[10];

	[RPC] // destroy this also on other inet players when getting destroyed
	private void netDestroy()
	{
		Destroy (gameObject);
	}

	// deactivates other world players so they wont be seen when going to other level
	public void deactivateWorldPlayers()
	{
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(go.name.Contains("plrWorld"))
			{
				if(go != gameObject)
				{
					go.renderer.enabled = false;
					for( int i = 0; i < go.transform.childCount; ++i )
					{
						go.transform.GetChild(i).gameObject.SetActive(false);
					} 
					go.GetComponent<selectorControls>().enabled = false;
				}
			}
		}

		gameObject.renderer.enabled = false;
		for( int i = 0; i < transform.childCount; ++i )
		{
			transform.GetChild(i).gameObject.SetActive(false);
		} 
		gameObject.GetComponent<selectorControls>().enabled = false;
	}

	// sets the level you are standing on to finished
	public void setLvlFinished()
	{
		bool levelAlreadyComplete = false;
		foreach(Collider2D col in Physics2D.OverlapCircleAll((Vector2)transform.position, 0.1f))
		{
			if(col.tag == "level")
			{
				col.GetComponent<worldLevelInfo>().isComplete = true;
				for(int i = 0; i < publicStorage.levelsFinished.Count; i++)
					if((int)publicStorage.levelsFinished[i] == col.GetComponent<worldLevelInfo>().levelIndex)
						levelAlreadyComplete = true;

				if(!levelAlreadyComplete)
					publicStorage.levelsFinished.Add(col.GetComponent<worldLevelInfo>().levelIndex);

				break;
			}
		}
	}

	// sets player position to excact whole number
	private void setPos()
	{
		transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);

		expectingLeftSide = false;
		expectingRightSide = false;
		expectingLevel = false;
		isOnMove = false;

		lastMoveDir = moveDir;
	}

	// checks if standing on level and also possibly starts this level you are standing on
	private bool checkForLevel(bool onlyCheck)
	{
		Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position, 0.05f);
		if(col)
		{
			if(col.tag == "level")
			{
				if(!onlyCheck)
				{
					publicStorage.currentWorldPos = transform.position;
					int levelIndex = col.GetComponent<worldLevelInfo>().levelIndex;

					if(!publicNetworkData.cooperative || publicStorage.localGame)
					{
						//networkView.RPC("netDestroy", RPCMode.Others);
						deactivateWorldPlayers();
						transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalLevel(levelIndex);
					}
					else
					{
						if(Network.isServer)
						{
							transform.Find ("/NetworkManager").GetComponent<NetworkManager>().networkView.RPC("startLevel", RPCMode.All, levelIndex);
						}
					}
				}

				return true;
			}
		}
		return false;
	}

	// check if level is completed you are standing on
	private bool checkForLevelCompletion()
	{
		Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position, 0.05f);
		if(col)
		{
			if(col.tag == "level")
			{
				if(col.GetComponent<worldLevelInfo>().isComplete)
				{
					return true;
				}
			}
		}

		return false;
	}

	// checks my position so i wont be going over bounds when moving
	private bool checkMyPos()
	{
		int colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position - moveDir/2, 0.05f, colResults);
		if(expectingLevel)
			for(int i = 0; i < colAmount; i++)
			{
				if(colResults[i].tag == "level")
					return true;
			}

		if(moveDir == Vector2.up || moveDir == -Vector2.up)
		{
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position - moveDir/2 +Vector2.right, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(expectingLeftSide)
					if(colResults[i].tag == "level" || colResults[i].tag == "road")
						return true;
			}
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position - moveDir/2 -Vector2.right, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(expectingRightSide)
					if(colResults[i].tag == "level" || colResults[i].tag == "road")
						return true;
			}
		}
		else
		{
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position - moveDir/2 +Vector2.up, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(expectingLeftSide)
					if(colResults[i].tag == "level" || colResults[i].tag == "road")
						return true;
			}
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position - moveDir/2 -Vector2.up, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(expectingRightSide)
					if(colResults[i].tag == "level" || colResults[i].tag == "road")
						return true;
			}
		}

		return false;
	}

	// checks step ahead to where i'm about to move, for it is free to move
	private bool checkStepAhead()
	{
		int colAmount;
		if(moveDir == Vector2.up || moveDir == -Vector2.up)
		{
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + moveDir +Vector2.right, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(colResults[i].tag == "level" || colResults[i].tag == "road")
					expectingLeftSide = true;
			}
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + moveDir -Vector2.right, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(colResults[i].tag == "level" || colResults[i].tag == "road")
					expectingRightSide = true;
			}
		}
		else
		{
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + moveDir +Vector2.up, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(colResults[i].tag == "level" || colResults[i].tag == "road")
					expectingLeftSide = true;
			}
			colAmount = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + moveDir -Vector2.up, 0.05f, colResults);
			for(int i = 0; i < colAmount; i++)
			{
				if(colResults[i].tag == "level" || colResults[i].tag == "road")
					expectingRightSide = true;
			}
		}

		Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position + moveDir, 0.49f);
		if(col)
		{
			if(col.tag == "level")
				expectingLevel = true;

			return true;
		}

		return false;
	}

	// function to move the player frame by frame
	private void Move()
	{
		if(checkStepAhead())
		{
			transform.Translate(moveDir.x/10, moveDir.y/10, 0);
			if(checkMyPos())
				setPos();
		}
		else
		{
			setPos();
		}
	}

	// updates keys if they've been modified
	private void setNewKeys()
	{
		key_up = publicStorage.userCtrl1[0];
		key_down = publicStorage.userCtrl1[1];
		key_left = publicStorage.userCtrl1[2];
		key_right = publicStorage.userCtrl1[3];
		key_run = publicStorage.userCtrl1[4];
		key_jump = publicStorage.userCtrl1[5];
		
		if(publicStorage.hasAxis)
		{
			for(int i = 0; i < 6; i++)
			{
				if(publicStorage.userCtrl1[i].Length > 4)
				{
					if(publicStorage.userCtrl1[i].Substring(0, 4) == "JoyX" || publicStorage.userCtrl1[i].Substring(0, 4) == "JoyY")
					{
						btnIsAxis[i] = true;
					}
					else
					{
						btnIsAxis[i] = false;
					}
				}
			}
		}
		publicStorage.upToDate = true;
	}

	// Use this for initialization
	void Start ()
	{
		btnIsAxis = new bool[] {false, false, false, false, false, false};

		expectingLeftSide = false;
		expectingRightSide = false;
		expectingLevel = false;
		isOnMove = false;

		jumpBtnIsPressed = false;
		runBtnIsPressed = false;
		upPressed = false;
		downPressed = false;
		leftPressed = false;
		rightPressed = false;

		lastMoveDir = Vector2.zero;
		moveDir = Vector2.zero;

		setNewKeys();
		DontDestroyOnLoad(gameObject);

		if(!publicStorage.localGame)
		{
			if(!publicNetworkData.cooperative)
			{
				if(!networkView.isMine)
				{
					GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f, 0.5f);
					Destroy(transform.FindChild("Main Camera").gameObject);
				}
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(!publicStorage.upToDate)
			setNewKeys();

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(publicStorage.localGame)
			{
				Time.timeScale = 0;
			}
			
			publicStorage.refToPauser.gameObject.SetActive(true);
			publicStorage.gamePaused = true;
		}

		if(btnIsAxis[5])
		{
			if(Input.GetAxis(key_jump) != 0 && !jumpBtnIsPressed)
			{
				jumpBtnIsPressed = true;
			}
			else
			{
				jumpBtnIsPressed = false;
			}
		}
		else
		{
			if(Input.GetKey( (KeyCode)System.Enum.Parse(typeof(KeyCode), key_jump) ))
			{
				jumpBtnIsPressed = true;
			}
			else
			{
				jumpBtnIsPressed = false;
			}
		}
		
		if(btnIsAxis[4])
		{
			if(Input.GetAxis(key_run) != 0)
				runBtnIsPressed = true;
			else
				runBtnIsPressed = false;
		}
		else
		{
			if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_run)))
				runBtnIsPressed = true;
			else
				runBtnIsPressed = false;
		}
		
		if(btnIsAxis[0] || btnIsAxis[1])
		{
			if(Input.GetAxis(key_down) < 0)
			{
				downPressed = true;
			}
			else
			{
				downPressed = false;
			}

			if(Input.GetAxis(key_down) > 0)
			{
				upPressed = true;
			}
			else
			{
				upPressed = false;
			}
		}
		else
		{
			if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_down)))
			{
				downPressed = true;
			}
			else
			{
				downPressed = false;
			}

			if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_up)))
			{
				upPressed = true;
			}
			else
			{
				upPressed = false;
			}
		}
		
		if(btnIsAxis[2] || btnIsAxis[3])
		{
			if(Input.GetAxis(key_right) < 0)
			{
				leftPressed = true;
			}
			else
			{
				leftPressed = false;
			}
			
			if(Input.GetAxis(key_right) > 0)
			{
				rightPressed = true;
			}
			else
			{
				rightPressed = false;
			}
		}
		else
		{
			if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_left)))
			{
				leftPressed = true;
			}
			else
			{
				leftPressed = false;
			}
			
			if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_right)))
			{
				rightPressed = true;
			}
			else
			{
				rightPressed = false;
			}
		}

		if(!isOnMove && !publicStorage.gamePaused)
		{
			if (publicStorage.localGame 
			|| !publicStorage.localGame && publicNetworkData.cooperative && Network.isServer 
			|| !publicStorage.localGame && !publicNetworkData.cooperative && networkView.isMine)
			{
				if(runBtnIsPressed)
				{
						
				}
				else if(jumpBtnIsPressed)
				{
					// select level you are standing on
					checkForLevel(false);
				}
				else if(upPressed)
				{
					moveDir = Vector2.up;
					if(checkForLevel(true))
					{
						if(!checkForLevelCompletion())
						{
							if(-lastMoveDir == moveDir)
								isOnMove = true;
						}
						else
							isOnMove = true;
					}
					else
						isOnMove = true;
				}
				else if(rightPressed)
				{
					moveDir = Vector2.right;
					if(checkForLevel(true))
					{
						if(!checkForLevelCompletion())
						{
							if(-lastMoveDir == moveDir)
								isOnMove = true;
						}
						else
							isOnMove = true;
					}
					else
						isOnMove = true;
				}
				else if(downPressed)
				{
					moveDir = -Vector2.up;
					if(checkForLevel(true))
					{
						if(!checkForLevelCompletion())
						{
							if(-lastMoveDir == moveDir)
								isOnMove = true;
						}
						else
							isOnMove = true;
					}
					else
						isOnMove = true;
				}
				else if(leftPressed)
				{
					moveDir = -Vector2.right;
					if(checkForLevel(true))
					{
						if(!checkForLevelCompletion())
						{
							if(-lastMoveDir == moveDir)
								isOnMove = true;
						}
						else
							isOnMove = true;
					}
					else
						isOnMove = true;
				}
			}
		}
		else if(!publicStorage.gamePaused)
		{
			Move();
		}
	}
}
