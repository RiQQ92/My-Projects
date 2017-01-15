using UnityEngine;
using System.Collections;

// quick commentation
public class plrControl : Gravitation
{
	//private Vector2 lastVelocity;
	//private Vector2 newVelocity;

	private float syncDelay;
	private float lastSyncTime;
	//private float syncTime;

	private bool sendRPCs;
	private bool sendingStartover;

	// Characters personal variables
	public LayerMask defaultLayer;
	private int nhspeed;
	private int phspeed;
	private int countFrames;
	private int myWorld;
	private int myLevel;

	private float jumpPressed;
	private float immortalTime;
	private float runAdjust;
	private float moveVertical;
	private float moveHorizontal;
	private float jumpLasting;
	private float jumpStart;
	private float gravitationForce;

	private bool runBtnIsPressed;
	private bool isDead;
	private bool playersInfoAllocated;
	private bool lvlComplete;
	private bool headStuck;
	private bool rdyToTakeDmg;
	private bool hasPowerup;
	private bool turnWithSkit;
	private bool downPressed;
	private bool isSliding;
	private bool facingRight;
	private bool onHill;
	private bool isOverOneWayPlatform;
	private bool hasBeenOveOneWayPlatform;
	private bool[] btnIsAxis;

	private Vector2 relativeVelocity;
	private Vector2 finishPos;
	private Vector2 movement;
	private Vector3 spriteBounds;

	private Animator anim;

	private hillOnFront frontCollider;
	private hillOnRear rearCollider;

	//public variables, most are set in Inspector window in Unity
	public bool headInWall;
	public bool onMovingPlatform;
	public bool onHillFront;
	public bool onHillRear;
	public bool isGrounded;
	public bool isGrownup;
	public bool isCrouching;
	public bool jumping;
	public bool jumpBtnIsPressed;

	public float jumpPower;
	public float thrustSpd;
	public float minSpd;
	public float maxSpd;
	public float maxDownForce;
	
	public string key_jump;
	public string key_run;
	public string key_up;
	public string key_down;
	public string key_left;
	public string key_right;

	public GUIText runHelp;
	public GUIText veloX;
	public GUIText veloY;
	public GUIText grounded;
	public GUIText hilled;

	public Vector2 groundMovement;
	// prefab for players so we can create players via script
	public Transform plrPrefab;

	private struct plrInfo
	{
		public NetworkViewID netID;
		public Vector3 pos;
		public Vector3 velo;
		public bool crouched;
		public bool left;
		public bool right;
		public string state; // ie, 'grownup, powerupped else its small'
	}

	private plrInfo[] playersInfo = new plrInfo[4];

	[RPC]
	private void setLevel(int lvlIndex)
	{
		myLevel = lvlIndex;
	}
	
	[RPC]
	private void setWorld(int worldIndex)
	{
		myWorld = worldIndex;
	}

	[RPC]
	private void checkNetWorldLevel(int plrID, NetworkMessageInfo info)
	{
		if(!publicStorage.localGame)
		{
			if(!publicNetworkData.cooperative)
			{
				if(!networkView.isMine)
				{
					if(myWorld == publicStorage.currentWorld)
					{
						if(myLevel == publicStorage.currentLevel)
						{
							GameObject me = gameObject;
							foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
							{
								if(go.networkView)
								{
									if(go.name.Contains("Player"))
									{
										if(go.networkView.isMine)
										{
											me = go;
											break;
										}
									}
								}
							}

							if(me != gameObject)
								me.networkView.RPC("activateCompetitivePlayer", info.sender);

							return;
						}
					}
				}
			}
		}

		if(!networkView.isMine)
			deactivatePlayer();
	}

	public void deactivatePlayer()
	{
		gameObject.renderer.enabled = false;
		gameObject.GetComponent<Animator>().enabled = false;
		for( int i = 0; i < transform.childCount; ++i )
		{
			transform.GetChild(i).gameObject.SetActive(false);
		} 
	}

	public void lvlFinishd()
	{
		finishPos = (Vector2)transform.localPosition;
		lvlComplete = true;

		if(!publicStorage.localGame)
		{
			if(networkView.isMine)
			{
				StartCoroutine(finishTimer());
			}
		}
		else
			StartCoroutine(finishTimer());
	}

	private IEnumerator netLevelEnd(bool levelComplete)
	{
		if(publicNetworkData.cooperative)
		{
			sendingStartover = true;
			networkView.RPC("stopRPC", RPCMode.All);
		}

		yield return new WaitForSeconds(0.5f);

		if(publicNetworkData.cooperative)
			transform.Find ("/NetworkManager").GetComponent<NetworkView>().RPC("startWorld", RPCMode.All, publicStorage.currentWorld, levelComplete);
		else
		{
			transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalWorld(publicStorage.currentWorld, levelComplete);
			networkView.RPC("netDestroy", RPCMode.Others);
			Destroy(gameObject);
		}


		yield return true;
	}

	private IEnumerator waitPlrInitialization()
	{
		yield return new WaitForSeconds(1.5f);

		//Transform publicStorage.refToNetManager = transform.Find("/NetworkManager");
		for(int a = 1; a < 5; a++)
		{
			Transform t = transform.Find("/Player"+a.ToString());
			if(t != null)
			{
				publicStorage.refToPlayers[a-1] = t;
				publicStorage.plrsInThisWorld = a;

				GameObject plrObj = transform.Find("/Player"+a.ToString()).gameObject;
				playersInfo[a-1].netID = plrObj.networkView.viewID;
				playersInfo[a-1].state = "small";
				playersInfo[a-1].pos = Vector3.zero;
				playersInfo[a-1].velo = Vector3.zero;
				playersInfo[a-1].crouched = false;

				networkView.RPC("shareNetIDs", RPCMode.Others, a-1, playersInfo[a-1].netID);
			}
		}
		networkView.RPC("countPlayers", RPCMode.Others);

		yield return true;
	}

	private IEnumerator finishTimer()
	{
		yield return new WaitForSeconds(5.0f);

		if(!publicStorage.localGame)
		{
			if(networkView.isMine)
			{
				StartCoroutine(netLevelEnd(true));
			}
		}
		else
		{
			publicStorage.splitCamInstantiated = false;
			publicStorage.lvlLoaded = false;
			publicStorage.splitInstantiated = false;
			transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalWorld(publicStorage.currentWorld, true);
		}

		yield return true;
	}

	private IEnumerator immortal()
	{
		yield return new WaitForSeconds(immortalTime);

		rdyToTakeDmg = true;

		yield return true;
	}

	private bool checkIfOverOneWayPlatform()
	{
		if(!jumping)
		{
			if(rigidbody2D.velocity.y < 0.5f || onMovingPlatform)
			{
				if(isGrounded)
				{
					return isOverOneWayPlatform;
				}
			}
		}
		return false;
	}

	private void updateCamera(int playerNumber)
	{
		Camera cam;
		if(playerNumber == 1)
		{
			//cam = transform.Find("/Player1/Main Camera").camera;
			transform.Find("/Main Camera2").camera.enabled = false;
			cam = transform.Find("/Main Camera1").camera;
			BoxCollider2D box = cam.GetComponent<BoxCollider2D>();
			transform.Find("/BG&Scenery/BG_Camera1").camera.rect = new Rect(0,0,1,1);
			cam.rect = new Rect(0,0,1f,1);
			cam.pixelRect = new Rect (0, 0, cam.pixelWidth, cam.pixelHeight);
			cam.aspect = cam.pixelWidth/cam.pixelHeight;
			//cam.GetComponent<setBoxToCamSize>().updateBox();
			float height = 2f * cam.orthographicSize;
			float width = height * cam.aspect;
			box.size = new Vector2(width, height);

			cam.GetComponent<restrictMovement>().updateBorders();
			//box.size = new Vector2(cam.pixelWidth, cam.pixelHeight);
		}
		else
		{
			//cam = transform.Find("/Player2/Main Camera").camera;
			transform.Find("/Main Camera1").camera.enabled = false;
			cam = transform.Find("/Main Camera2").camera;
			BoxCollider2D box = cam.GetComponent<BoxCollider2D>();
			transform.Find("/BG&Scenery/BG_Camera2").camera.rect = new Rect(0,0,1,1);
			cam.rect = new Rect(0,0,1f,1);
			cam.pixelRect = new Rect (0, 0, cam.pixelWidth, cam.pixelHeight);
			cam.aspect = cam.pixelWidth/cam.pixelHeight;
			//cam.GetComponent<setBoxToCamSize>().updateBox();
			float height = 2f * cam.orthographicSize;
			float width = height * cam.aspect;
			box.size = new Vector2(width, height);

			cam.GetComponent<restrictMovement>().updateBorders();
			//box.size = new Vector2(cam.pixelWidth, cam.pixelHeight);
		}
	}

	private void kill()
	{
		//Destroy(gameObject);
		isDead = true;
		if(!publicStorage.localGame)
		{
			if(publicNetworkData.cooperative)
			{
				GameObject theOther = gameObject;
				bool foundOther = false;
				foreach(GameObject gObj in GameObject.FindGameObjectsWithTag("Player"))
				{
					if(gObj.transform.parent == null)
					{
						if(gObj != gameObject)
						{
							theOther = gObj;
							foundOther = true;
						}
					}
				}
				if(foundOther)
				{
					Transform cam = transform.Find("/Main Camera");
					cam.GetComponent<followAnyone>().playerToFollow = theOther.name;
					//StartCoroutine(transform.Find("/NetworkManager").GetComponent<NetworkManager>().returnCommunicationsAfter(0.5f));
					networkView.RPC("plrDied", RPCMode.Others);
					Destroy(gameObject, 0.5f);
				}
				else
					if(networkView.isMine && !sendingStartover)
						StartCoroutine(netLevelEnd(false));
			}
			else
			{
				// something private, not connected to other clients/server
				publicStorage.lvlLoaded = false;
				StartCoroutine(netLevelEnd(false));
			}
		}
		else if(publicStorage.Splitscreen)
		{
			Debug.Log("my name: "+gameObject.name);
			bool otherAlive = false;
			GameObject[] plrs = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject found in plrs)
			{
				Debug.Log(gameObject.name+"!="+found.gameObject.name);
				if(found.gameObject.name != gameObject.name && found.gameObject.name != "HeadCollider" && found.gameObject.name != "plrWorld1")
				{
					Debug.Log("test");
					otherAlive = true;
				}
			}
			if(otherAlive)
			{
				int plrNum;
				if(gameObject.name == "Player1")
				{
					Destroy(transform.Find("/BG&Scenery/BG_Camera1").gameObject);
					Destroy(transform.Find("/Main Camera1").gameObject);
					plrNum = 2;
				}
				else
				{
					Destroy(transform.Find("/BG&Scenery/BG_Camera2").gameObject);
					Destroy(transform.Find("/Main Camera2").gameObject);
					plrNum = 1;
				}
					
				updateCamera(plrNum);
				//enabled = false;
				Destroy(gameObject);
			}
			else
			{
				Debug.Log("reloading world");
				publicStorage.splitInstantiated = false;
				publicStorage.splitCamInstantiated = false;
				publicStorage.lvlLoaded = false;
				transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalWorld(publicStorage.currentWorld);
			}
		}
		else
		{
			publicStorage.lvlLoaded = false;
			transform.Find ("/NetworkManager").GetComponent<NetworkManager>().startLocalWorld(publicStorage.currentWorld);
		}
	}

	private void takeHit()
	{
		if(hasPowerup)
		{
			hasPowerup = false;
		}
		else if(isGrownup)
		{
			isGrownup = false;
		}
		else
		{
			kill();
		}
	}

	private void setNewKeys(string name)
	{
		if(name == "Player1" || !publicStorage.localGame)
		{
			key_up = publicStorage.userCtrl1[0];
			key_down = publicStorage.userCtrl1[1];
			key_left = publicStorage.userCtrl1[2];
			key_right = publicStorage.userCtrl1[3];
			key_run = publicStorage.userCtrl1[4];
			key_jump = publicStorage.userCtrl1[5];
		}
		else
		{
			key_up = publicStorage.userCtrl2[0];
			key_down = publicStorage.userCtrl2[1];
			key_left = publicStorage.userCtrl2[2];
			key_right = publicStorage.userCtrl2[3];
			key_run = publicStorage.userCtrl2[4];
			key_jump = publicStorage.userCtrl2[5];
		}

		if(publicStorage.hasAxis)
		{
			for(int i = 0; i < 6; i++)
			{
				if(publicStorage.userCtrl1[i].Length > 4 && name == "Player1" || publicStorage.userCtrl1[i].Length > 4 && !publicStorage.localGame)
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
				if(publicStorage.userCtrl2[i].Length > 4 && name == "Player2")
				{
					if(publicStorage.userCtrl2[i].Substring(0, 4) == "JoyX" || publicStorage.userCtrl2[i].Substring(0, 4) == "JoyY")
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

	// flips player
	private void flip()
	{
		facingRight = !facingRight;
		transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
	}

	// returns the scaleY slowly to normal
	private void resetScale()
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y +0.04f, transform.localScale.z);
		//if scle goes larger than 100% set it to 100%
		if(transform.localScale.y > 1)
			transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
	}

	private void playerMove()
	{
		if(!onMovingPlatform)
			relativeVelocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y - groundMovement.y);
		else
			relativeVelocity = new Vector2(relativeVelocity.x, 0);

		if(publicStorage.localGame || Network.isServer)
		{
			if(!rdyToTeleport)
			{
				bool findTeleport = false;
				foreach(Collider2D col in Physics2D.OverlapCircleAll(rigidbody2D.position, 1f))
				{
					if(col.gameObject.tag == "teleport")
					{
						findTeleport = true;
					}
				}
				if(!findTeleport)
					rdyToTeleport = true;
			}
		}


		// Raycast colliding to see are we standing on ground and able to jump again
		//set raycast start point
		Vector3 objLeftLowerCorner = new Vector3(transform.position.x -spriteBounds.x+ 0.01f, transform.position.y -spriteBounds.y -0.05f, transform.position.z);
		Vector3 objRightLowerCorner = new Vector3(transform.position.x +spriteBounds.x- 0.01f, transform.position.y -spriteBounds.y -0.05f, transform.position.z);
		Vector3 objCenterLowerCorner = new Vector3(transform.position.x, transform.position.y -spriteBounds.y -0.05f, transform.position.z);
		
		//start raycast from these points and set direction
		RaycastHit2D hit1 = Physics2D.Raycast(objLeftLowerCorner, -Vector2.up, 0.5f, defaultLayer);
		RaycastHit2D hit2 = Physics2D.Raycast(objRightLowerCorner, -Vector2.up, 0.5f, defaultLayer);
		RaycastHit2D hit3 = Physics2D.Raycast(objCenterLowerCorner, -Vector2.up, 0.5f, defaultLayer);
		
		
		// Raycasts hit something?
		if (hit2.collider != null || hit1.collider != null || hit3.collider != null)
		{
			float maxDist = 0.15f;
			bool found = false;
			//was it this that hit?
			if(hit3.collider != null)
			{
				if(hit3.collider.gameObject.tag == "baseGround" && !found)
				{
					if(hit3.collider.GetComponent<BoxCollider2D>() != null)
					{
						if(!hit3.collider.GetComponent<BoxCollider2D>().isTrigger || hit3.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit3.point.y - objCenterLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit3.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
					else
					{
						if(!hit3.collider.GetComponent<PolygonCollider2D>().isTrigger || hit3.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit3.point.y - objLeftLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit3.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
				}
			}
			//or this that hit?
			if(hit2.collider != null)
			{
				if(hit2.collider.gameObject.tag == "baseGround" && !found)
				{
					if(hit2.collider.GetComponent<BoxCollider2D>() != null)
					{
						if(!hit2.collider.GetComponent<BoxCollider2D>().isTrigger || hit2.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit2.point.y - objRightLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit2.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
					else
					{
						if(!hit2.collider.GetComponent<PolygonCollider2D>().isTrigger || hit2.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit2.point.y - objLeftLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit2.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
				}
			}
			//or this that hit?
			if(hit1.collider != null)
			{
				if(hit1.collider.gameObject.tag == "baseGround" && !found)
				{
					if(hit1.collider.GetComponent<BoxCollider2D>() != null)
					{
						if(!hit1.collider.GetComponent<BoxCollider2D>().isTrigger || hit1.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit1.point.y - objLeftLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit1.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
					else
					{
						if(!hit1.collider.GetComponent<PolygonCollider2D>().isTrigger || hit1.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
						{
							//absolute value
							float distance = Mathf.Abs(hit1.point.y - objLeftLowerCorner.y);
							
							// distance between start point and hit point
							if(distance < maxDist)
							{
								isGrounded = true; 
								found = true;
								if(hit1.collider.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
									if(distance <= 0.01f)
										isOverOneWayPlatform = true;
							}
							else
								isGrounded = false;
						}
					}
				}
			}
			// if didnt collide on ground, we are still probably flying
			if(!found)
			{
				isGrounded = false;
				isOverOneWayPlatform = false;
			}
		}
		else
			isGrounded = false;

		/*
		if(!isGrounded)
			groundMovement = Vector2.zero;
		*/
		if(!publicStorage.upToDate)
			setNewKeys(gameObject.name);
		
		// if scale is not 100%
		if(transform.localScale.y != 1)
			resetScale();

		// if hill checker returning true - player is on hill
		if(onHillFront && onHillRear)
		{
			onHill = false;
			transform.Translate(0, 0.05f, 0);
		}
		else if(onHillFront && !onHillRear || onHillRear && !onHillFront)
		{
			onHill = true;
		}
		else
		{
			onHill = false;
		}
		
		headStuck = false;
		turnWithSkit = false;
		
		// get some information from animator components about what state we are animating at the moment
		AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);
		if(animState.nameHash == Animator.StringToHash("Base Layer.playerCrouch"))
		{
			isCrouching = true;
		}
		else if(animState.nameHash == Animator.StringToHash("Base Layer.playerBottomSlope") || animState.nameHash == Animator.StringToHash("Base Layer.playerBottomSlopeSmall"))
		{
			isCrouching = true;
			isSliding = true;
		}
		else
		{
			isCrouching = false;
			isSliding = false;
		}
		
		if(animState.nameHash == Animator.StringToHash("Base Layer.playerCheckHead") && headInWall)
		{
			foreach(Collider2D col in Physics2D.OverlapCircleAll(new Vector2(rigidbody2D.position.x, rigidbody2D.position.y +0.35f), 0.35f))
			{
				if(col.gameObject.tag == "baseGround")
				{
					headStuck = true;
					break;
				}
			}
			if(!headStuck)
				headInWall = false;
		}
		
		if(!headStuck)
		{
			if(rigidbody2D.velocity.y == 0 || onMovingPlatform)
			{
				if(frontCollider.isColliding && rearCollider.isColliding)
				{
					transform.Translate(0, 0.02f, 0);
				}
			}
		}

		//negativeHorizontalSpeed
		nhspeed = 0;
		//positiveHorizontalSpeed
		phspeed = 0;
		
		if(!lvlComplete)
		{
			if(networkView.isMine && !publicStorage.gamePaused || publicStorage.localGame)
			{
				if(Input.GetKeyUp(KeyCode.Escape))
				{
					if(publicStorage.localGame)
					{
						Time.timeScale = 0;
					}

					publicStorage.refToPauser.gameObject.SetActive(true);
					publicStorage.gamePaused = true;
				}

				if(!headStuck)
				{
					// if jumpkey pressed, grounded, and is not jumping
					if(btnIsAxis[5])
					{
						if(Input.GetAxis(key_jump) != 0 && !jumpBtnIsPressed)
						{
							jumpBtnIsPressed = true;
							jumpPressed = Time.time;
							if(isGrounded && !jumping)
							{
								//set jump power related to runningboost which is gained by running fast
								moveVertical = jumpPower *(1 +runAdjust /25);
								
								//set velocity same as this horixontal speed is already and set Vertical speed to jumpPower
								relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
								isGrounded = false;
								
								//time to keep track how long you can jump -> bigger jumps
								jumpStart = Time.time;
								jumping = true;
							}
						}
						else if(Input.GetAxis(key_jump) != 0 && isGrounded && !jumping && jumpPressed+0.05f > Time.time)
						{
							//set jump power related to runningboost which is gained by running fast
							moveVertical = jumpPower *(1 +runAdjust /25);
							
							//set velocity same as this horixontal speed is already and set Vertical speed to jumpPower
							relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
							isGrounded = false;
							
							//time to keep track how long you can jump -> bigger jumps
							jumpStart = Time.time;
							jumping = true;
						}
						// checking the time if keypressed constantly and jump start time
						else if((jumpStart + jumpLasting) > Time.time && Input.GetAxis(key_jump) != 0 && jumping)
						{
							relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
						}
						else
						{
							jumping = false;
							moveVertical = 0;
						}

						if(Input.GetAxis(key_jump) == 0)
						{
							jumpBtnIsPressed = false;
						}
					}
					else
					{
						if(Input.GetKey( (KeyCode)System.Enum.Parse(typeof(KeyCode), key_jump) ) && !jumpBtnIsPressed)
						{
							jumpBtnIsPressed = true;
							jumpPressed = Time.time;
							if(isGrounded && !jumping)
							{
								//set jump power related to runningboost which is gained by running fast
								moveVertical = jumpPower *(1 +runAdjust /25);
								
								//set velocity same as this horixontal speed is already and set Vertical speed to jumpPower
								relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
								isGrounded = false;
								
								//time to keep track how long you can jump -> bigger jumps
								jumpStart = Time.time;
								jumping = true;
							}
						}
						else if(Input.GetKey( (KeyCode)System.Enum.Parse(typeof(KeyCode), key_jump) ) && isGrounded && !jumping && jumpPressed+0.05f > Time.time)
						{
							//set jump power related to runningboost which is gained by running fast
							moveVertical = jumpPower *(1 +runAdjust /25);
							
							//set velocity same as this horixontal speed is already and set Vertical speed to jumpPower
							relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
							isGrounded = false;
							
							//time to keep track how long you can jump -> bigger jumps
							jumpStart = Time.time;
							jumping = true;
						}
						// checking the time if keypressed constantly and jump start time
						else if((jumpStart + jumpLasting) > Time.time && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_jump)) && jumping)
						{
							relativeVelocity = new Vector2(relativeVelocity.x, moveVertical);
						}
						else
						{
							jumping = false;
							moveVertical = 0;
						}

						if(!Input.GetKey( (KeyCode)System.Enum.Parse(typeof(KeyCode), key_jump)))
						{
							jumpBtnIsPressed = false;
						}
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
				}
				
				if(btnIsAxis[2] || btnIsAxis[3])
				{
					if(Input.GetAxis(key_right) < 0)
					{
						nhspeed = -1;
					}
					else
					{
						nhspeed = 0;
					}
					
					if(Input.GetAxis(key_right) > 0)
					{
						phspeed = 1;
					}
					else
					{
						phspeed = 0;
					}
				}
				else
				{
					if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_left)))
					{
						nhspeed = -1;
					}
					else
					{
						nhspeed = 0;
					}
					
					if(Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key_right)))
					{
						phspeed = 1;
					}
					else
					{
						phspeed = 0;
					}
				}
			}
			else
			{
				int plrNum = 0;
				for(int a = 0; a < 4; a++)
				{
					if(networkView.viewID == playersInfo[a].netID)
						plrNum = a;
				}

				if(playersInfo[plrNum].left)
					nhspeed = -1;
				else
					nhspeed = 0;

				if(playersInfo[plrNum].right)
					phspeed = 1;
				else
					phspeed = 0;

				Debug.Log(plrNum);
				Debug.Log("positive: "+phspeed);
				Debug.Log("negative: "+nhspeed);
			}

			if(GetComponent<restrictMovement>().xEdge)
			{
				if(transform.position.x > 5)
					phspeed = 0;
				else
					nhspeed = 0;
			}
			// calculates Horizontal movement to be either +1 or -1 or 0
			moveHorizontal = nhspeed + phspeed;
			
			// makes it simple to only multiply horizontal speed with movement thrustpower and delta time, vertical speed is same as we are moving now
			movement = new Vector2(moveHorizontal * thrustSpd * Time.deltaTime, relativeVelocity.y);
			
			if(!headStuck)
			{
				//if going faster than allowed
				if(relativeVelocity.x < -maxSpd && !isCrouching && isGrounded || relativeVelocity.x > maxSpd && !isCrouching && isGrounded)
				{
					if(relativeVelocity.x > 0)
						movement.x = maxSpd;
					else
						movement.x = -maxSpd;
					
					relativeVelocity = movement;
				}
				// if going assSliding down faster than allowed
				else if(relativeVelocity.x < -maxSpd*1.4f && isCrouching && isGrounded || relativeVelocity.x > maxSpd*1.4f && isCrouching && isGrounded)
				{
					if(relativeVelocity.x > 0)
						movement.x = maxSpd*1.4f;
					else
						movement.x = -maxSpd*1.4f;
					
					relativeVelocity = movement;
				}
				// if going faster than minimum speed without pressing running key -> starts dragging speed down to minimum limit
				else if( (relativeVelocity.x < -minSpd && !runBtnIsPressed && isGrounded ) || ( relativeVelocity.x > minSpd && !runBtnIsPressed && isGrounded) )
				{
					float spdDecrese;
					
					if(movement.x > 0)
						spdDecrese = 0.075f;
					else if(movement.x < 0)
						spdDecrese = -0.075f;
					else
						spdDecrese = 0;
					
					relativeVelocity = new Vector2 (relativeVelocity.x - spdDecrese, movement.y);
				}
				// all limits checked, go into adding speed anyways
				else
				{
					if( (!isGrounded && relativeVelocity.x < -minSpd) || (!isGrounded && relativeVelocity.x > minSpd) )
						relativeVelocity = new Vector2 (relativeVelocity.x, movement.y);
					else if(isGrounded && !isCrouching)
					{
						if((runAdjust+minSpd) > relativeVelocity.x && relativeVelocity.x > minSpd && moveHorizontal != 0)
							relativeVelocity = new Vector2 (relativeVelocity.x + movement.x + 0.2f, movement.y);
						else if((-runAdjust -minSpd) < relativeVelocity.x && relativeVelocity.x < -minSpd && moveHorizontal != 0)
							relativeVelocity = new Vector2 (relativeVelocity.x + movement.x - 0.2f, movement.y);
						else
							relativeVelocity = new Vector2 (relativeVelocity.x + movement.x, movement.y);
					}
					else if(!isGrounded)
						relativeVelocity = new Vector2 (relativeVelocity.x + movement.x, movement.y);
				}
			}
			else
			{
				transform.Translate(new Vector3(2.5f * Time.deltaTime, 0, 0));
			}
			
			// if not pressing left or right and is on ground or if crouching and on ground
			if(phspeed == 0 && nhspeed == 0 && isGrounded || isCrouching && isGrounded || headStuck)
			{
				float drag;
				if(relativeVelocity.x < 0)
					drag = -0.2f;
				else if(relativeVelocity.x > 0)
					drag = 0.2f;
				else
					drag = 0;
				
				// create artificial drag
				relativeVelocity = new Vector2 (relativeVelocity.x -drag, movement.y); 
				
				//if drag pulls over 0, make horizontal speed 0
				if(drag > 0.1f && relativeVelocity.x < 0)
					relativeVelocity = new Vector2 (0, movement.y);
				if(drag < -0.1f && relativeVelocity.x > 0)
					relativeVelocity = new Vector2 (0, movement.y);
			}
			// if pressing only left
			else if(phspeed == 0 && nhspeed == -1)
			{
				//pressing left but still going to right
				if(relativeVelocity.x > 0)
				{
					if(onHill)
						relativeVelocity = new Vector2(relativeVelocity.x -1.1f, movement.y);
					else
						relativeVelocity = new Vector2(relativeVelocity.x -0.5f, movement.y);
					
					turnWithSkit = true;
				}
				// pressing left, going left but not yet to minimum speed -> boost acceleration
				else if(relativeVelocity.x > -minSpd)
				{
					if(onHill)
						relativeVelocity = new Vector2(relativeVelocity.x -0.55f, movement.y);
					else
						relativeVelocity = new Vector2(relativeVelocity.x -0.5f, movement.y);
				}
			}
			//if pressing only right
			else if(phspeed == 1 && nhspeed == 0)
			{
				//pressing right but still going to left
				if(relativeVelocity.x < 0)
				{
					if(onHill)
						relativeVelocity = new Vector2(relativeVelocity.x +1.1f, movement.y);
					else
						relativeVelocity = new Vector2(relativeVelocity.x +0.5f, movement.y);
					
					turnWithSkit = true;
				}
				// pressing right, going right but not yet to minimum speed -> boost acceleration
				else if(relativeVelocity.x < minSpd)
				{
					if(onHill)
						relativeVelocity = new Vector2(relativeVelocity.x +0.55f, movement.y);
					else
						relativeVelocity = new Vector2(relativeVelocity.x +0.5f, movement.y);
				}
			}
			
			// limits maximum falling speed
			if(relativeVelocity.y < maxDownForce)
				relativeVelocity = new Vector2(relativeVelocity.x, maxDownForce);
			
			// sets the runAdjust based on the values of minimum, maximum and current horizontal speed, only create run adjust if speed between min and max speed
			if(relativeVelocity.x < -minSpd || relativeVelocity.x > minSpd)
			{
				if(relativeVelocity.x > 0 && (runAdjust+minSpd) < relativeVelocity.x)
				{
					runAdjust = relativeVelocity.x -minSpd;
				}
				else if(relativeVelocity.x < 0 && (runAdjust+minSpd) < -relativeVelocity.x)
				{
					float temp = relativeVelocity.x *-1;
					runAdjust = temp -minSpd;
				}
			}
			
			// decrease runAdjust if the speed is not kept up
			if(relativeVelocity.x >= 0 && (runAdjust+minSpd) > relativeVelocity.x && runAdjust >= 0)
			{
				runAdjust -= 0.05f;
				if(runAdjust < 0)
					runAdjust = 0;
			}
			else if(relativeVelocity.x <= 0 && (runAdjust+minSpd) > -relativeVelocity.x && runAdjust >= 0)
			{
				runAdjust -= 0.05f;
				if(runAdjust < 0)
					runAdjust = 0;
			}
			
			// creates special drag and physical states to stop in hills
			if(onHill && moveHorizontal == 0)
			{
				if(Mathf.Abs(rigidbody2D.velocity.x) < 0.5f  && !isCrouching && !downPressed)
				{
					relativeVelocity = new Vector2 (0, relativeVelocity.y);
					rigidbody2D.velocity = Vector2.zero;
				}
				else if(!isCrouching && !downPressed)
				{
					float drag;
					if(relativeVelocity.x < 0)
						drag = -0.6f;
					else if(relativeVelocity.x > 0)
						drag = 0.6f;
					else
						drag = 0;
					
					// create artificial drag
					relativeVelocity = new Vector2 (relativeVelocity.x -drag, relativeVelocity.y); 
				}
				
				if(rigidbody2D.velocity.x == 0 && !isCrouching && !downPressed)
				{
					relativeVelocity = new Vector2 (0, 0); 
					rigidbody2D.velocity = Vector2.zero;
					gravityEnabled = false;
					rigidbody2D.isKinematic = true;
				}
				else
				{
					gravityEnabled = true;
					rigidbody2D.isKinematic = false;
				}
			}
			else if(!gravityEnabled)
			{
				gravityEnabled = true;
				rigidbody2D.isKinematic = false;
			}
			
		}
		else if(lvlComplete && isGrounded)
		{
			gravityEnabled = false;

			if(gameObject.GetComponent<restrictMovement>())
				Destroy(gameObject.GetComponent<restrictMovement>());
			
			if(!gameObject.rigidbody2D.isKinematic)
				gameObject.rigidbody2D.isKinematic = true;

			if(gravityEnabled)
				gravityEnabled = false;
			
			if(finishPos.x +20 > transform.localPosition.x)
				relativeVelocity = new Vector3(2.5f , 0, 0);
			else
				relativeVelocity = new Vector3(0, 0, 0);
		}
		else
			relativeVelocity = new Vector3(0, relativeVelocity.y, 0);
		
		//sets direction depending on speed when sliding on ass
		if(!isSliding && !lvlComplete)
		{
			if(nhspeed == -1 && phspeed == 0 && facingRight)
				flip();
			else if(phspeed == 1 && nhspeed == 0 && !facingRight)
				flip();
		}
		//othervice its set by which direction is pressed
		else
		{
			if(relativeVelocity.x < 0 && facingRight)
				flip();
			else if(relativeVelocity.x > 0 && !facingRight)
				flip();
		}

		if(!publicStorage.localGame)
			if(!publicNetworkData.cooperative)
				if(!networkView.isMine)
					gravityEnabled = false;

		if(checkIfOverOneWayPlatform())
		{
			if(!hasBeenOveOneWayPlatform)
			{
				RaycastHit2D surfaceCast = Physics2D.Raycast(transform.position, -Vector2.up, spriteBounds.y, defaultLayer);
				if(surfaceCast)
				{
					float moveToSpot = surfaceCast.collider.bounds.center.y + surfaceCast.collider.bounds.extents.y + spriteBounds.y;

					transform.position = new Vector3(transform.position.x , moveToSpot, transform.position.z);
				}

				hasBeenOveOneWayPlatform = true;
				relativeVelocity = new Vector2(relativeVelocity.x, 0);
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
			}
			gravityEnabled = false;
			rigidbody2D.isKinematic = true;
		}
		else if(!lvlComplete)
		{
			hasBeenOveOneWayPlatform = false;
			if(!onHill)
			{
				gravityEnabled = true;
				rigidbody2D.isKinematic = false;
			}
		}

		relativeVelocity = gravityPull(relativeVelocity);
		rigidbody2D.velocity = relativeVelocity + groundMovement;

		//Physics2D.IgnoreLayerCollision( LayerMask.NameToLayer("Characters"), LayerMask.NameToLayer("OneWayPlatform"), !isGrounded && rigidbody2D.velocity.y > 0.1f);

		onMovingPlatform = false;
		groundMovement = Vector2.zero;
		
		// set animator variables so it can change animations depending on these variables
		anim.SetFloat("Speed", Mathf.Abs(relativeVelocity.x));
		anim.SetBool ("onHill", onHill);
		anim.SetBool("Grounded", isGrounded);
		anim.SetBool("Crouching", downPressed);
		anim.SetBool("skitTurn", turnWithSkit);
		anim.SetBool("grownUp", isGrownup);
		anim.SetBool("headCollision", headInWall);
		
		/*
		// debug gui values
		runHelp.text = "runBoost: " + runAdjust.ToString();
		veloX.text = "velocityX: "+ relativeVelocity.x.ToString();
		veloY.text = "velocityY: "+ relativeVelocity.y.ToString();
		grounded.text = "grounded: " + isGrounded.ToString();
		hilled.text = "onHill: " + onHill.ToString();
		*/
		/*
		if(groundMovement != Vector2.zero)
			transform.Translate((Vector3)groundMovement);
		*/
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			int plrNum = 0;
			for(int a = 0; a < 4; a++)
			{
				if(networkView.viewID == playersInfo[a].netID)
					plrNum = a;
			}
			if(playersInfo[plrNum].crouched)
			{
				downPressed = true;
				if(!networkView.isMine)
					anim.CrossFade("playerCrouch", 0f);
			}
			else
				downPressed = false;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "powerup")
		{
			if(col.collider.gameObject.name == "pwupMushroom")
			{
				if(publicStorage.localGame || !publicNetworkData.cooperative)
				{
					if(!publicStorage.localGame && !publicNetworkData.cooperative)
						networkView.RPC("grewUp",RPCMode.Others);

					isGrownup = true;
					Destroy(col.gameObject);
				}
				else
				{
					isGrownup = true;
					networkView.RPC("grewUp",RPCMode.Others);
					col.gameObject.networkView.RPC("netDestroy",RPCMode.Others);
					Destroy(col.gameObject);
				}
			}
		}
		if(col.gameObject.tag == "enemy" && rdyToTakeDmg)
		{
			if(publicStorage.localGame || !publicNetworkData.cooperative)
			{
				takeHit();
				rdyToTakeDmg = false;
				StartCoroutine(immortal());
			}
			else
			{
				if(gameObject.networkView.isMine)
				{
					networkView.RPC("tookHit",RPCMode.Others);
					takeHit();
					rdyToTakeDmg = false;
					StartCoroutine(immortal());
				}
			}
		}
		if(col.gameObject.tag == "instaKiller")
		{
			kill();
		}
	}

	void Awake()
	{
		//if(publicStorage.localGame)
			//transform.FindChild("Main Camera").gameObject.SetActive(true);

		spriteBounds = renderer.bounds.extents;
	}
	//Initialization
	void Start()
	{
		// personal
		btnIsAxis = new bool[] {false, false, false, false, false, false};

		setNewKeys(gameObject.name);

		runBtnIsPressed = false;
		jumpBtnIsPressed = false;
		playersInfoAllocated = false;
		lvlComplete = false;
		headStuck = false;
		headInWall = false;
		rdyToTakeDmg = true;
		hasPowerup = false;
		isGrownup = false;
		onHill = false;
		onHillFront = false;
		onHillRear = false;
		isGrounded = false;
		turnWithSkit = false;
		isCrouching = false;
		isSliding = false;
		isDead = false;
		downPressed = false;
		facingRight = true;
		onMovingPlatform = false;
		isOverOneWayPlatform = false;
		hasBeenOveOneWayPlatform = false;

		anim = GetComponent<Animator>();
		frontCollider = GetComponentInChildren<hillOnFront>();
		rearCollider = GetComponentInChildren<hillOnRear>();

		//gravitationForce = rigidbody2D.gravityScale;
		jumpPressed = 0;
		countFrames = 0;
		immortalTime = 1;
		jumpLasting = 0.25f;
		jumping = false;
		runAdjust = 0;
		//defaultLayer = LayerMask.NameToLayer("OneWayPlatform");
		print(LayerMask.NameToLayer("Default"));
		print (LayerMask.NameToLayer("OneWayPlatform"));
		relativeVelocity = Vector2.zero;
		groundMovement = Vector2.zero;
		finishPos = Vector2.zero;
		gravityFactor = 4.5f;

		// network
		//lastVelocity = Vector2.zero;
		//newVelocity = Vector2.zero;
		
		syncDelay = 0f;
		lastSyncTime = 0f;
		//syncTime = 0f;

		sendRPCs = true;
		sendingStartover = false;

		if(!publicStorage.Splitscreen)
		{
			publicStorage.refToPlayers[0] = transform;
			publicStorage.plrsInThisWorld = 1;
		}
		// if splitscreen A.K.A. multiplayer, creates another player next to other
		if(publicStorage.Splitscreen && !publicStorage.splitInstantiated && !Network.isServer && !Network.isClient)
		{
			// set plr1's camera to up
			GameObject[] cams = GameObject.FindGameObjectsWithTag("MainCamera");
			/*
			gameObject.transform.FindChild("Main Camera").camera.rect = new Rect(0f,0.5f,1f,0.5f);
			transform.FindChild("Main Camera").GetComponent<setBoxToCamSize>().updateBox();
			*/

			// set public variable spliscreen to false to avoid infinite duplication
			publicStorage.splitInstantiated = true;

			//Creates duplicate of Player
			Transform t = (Transform) Instantiate(plrPrefab, new Vector3(transform.localPosition.x +1, transform.localPosition.y, transform.localPosition.z), Quaternion.identity);
			GameObject Player2 = t.gameObject;

			// set Settings for newli instantiated player
			//Player2.GetComponentInChildren<AudioListener>().enabled = false;
			Player2.name = "Player2";
			Player2.renderer.material.color = Color.red;
			// Player2.transform.FindChild("Main Camera").camera.rect = new Rect(0f,0f,1f,0.5f);

			foreach(GameObject cam in cams)
			{
				if(cam.name == "Main Camera1")
				{
					
					Debug.Log("cam Ready");
					cam.camera.rect = new Rect(0f,0.5f,1f,0.5f);
					cam.GetComponent<setBoxToCamSize>().updateBox();
				}
				if(cam.name == "Main Camera2")
				{
					Debug.Log("cam Ready");
					cam.camera.rect = new Rect(0f,0f,1f,0.5f);
					cam.GetComponent<setBoxToCamSize>().updateBox();
				}
			}
			
			publicStorage.refToPlayers[0] = transform;
			publicStorage.refToPlayers[1] = Player2.transform;
			publicStorage.plrsInThisWorld = 2;
		}

		if(!publicStorage.localGame)
		{
			/*
			if(publicNetworkData.cooperative)
			{
				if(Network.isServer)
					StartCoroutine(waitPlrInitialization());
				/*
				else
					Network.SetSendingEnabled(0, false);
				* /
			}
			*/

			if(!publicNetworkData.cooperative)
			{


				if(Network.isServer)
					publicStorage.refToNetManager.GetComponent<NetworkManager>().getPlayerNames();

				if(!networkView.isMine)
				{
					GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f, 0.5f);
					int childs = transform.childCount;
					for( int i = 0; i < childs; ++i )
					{
						Destroy(transform.GetChild(i).gameObject);
					} 
					Destroy(collider2D);
					rigidbody2D.isKinematic = true;
					gravityEnabled = false;
				}

				DontDestroyOnLoad(gameObject);
			}

			if(Network.isServer)
			{
				StartCoroutine(waitPlrInitialization());
			}
		}
	}

	[RPC]
	private void countPlayers()
	{
		int cnt = 0;

		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(go.name.Contains("Player"))
			{
				publicStorage.refToPlayers[cnt] = go.transform;
				cnt++;
				publicStorage.plrsInThisWorld = cnt;
			}
		}
	}

	[RPC]
	private void netDestroy()
	{
		Destroy(gameObject);
	}

	[RPC]
	private void activateCompetitivePlayer()
	{
		gameObject.renderer.enabled = true;
		gameObject.GetComponent<Animator>().enabled = true;
		for( int i = 0; i < transform.childCount; ++i )
		{
			transform.GetChild(i).gameObject.SetActive(true);
		} 
	}

	[RPC]
	private void setPositionByServer(Vector3 pos)
	{
		transform.position = pos;
	}

	/*
	[RPC]
	private void catchClientVelocities(Vector3 velo, NetworkMessageInfo info)
	{
		int from = int.Parse(info.sender.ToString())+1;
		Debug.Log(from);
		GameObject clientObj = NetworkView.Find(info.networkView.viewID).gameObject;
		clientObj.rigidbody2D.velocity = velo;
		//networkView.RPC("catchPlayerUpdateFromServer", info.sender, (Vector3)clientObj.rigidbody2D.position);
		//networkView.RPC("updateVelocity", RPCMode.Others, (Vector3)clientObj.rigidbody2D.velocity, info.networkView.viewID);
	}*/

	[RPC]
	private void gotKilled()
	{
		kill();
	}

	[RPC]
	private void tookHit()
	{
		takeHit();
	}

	[RPC]
	private void grewUp()
	{
		isGrownup = true;
	}

	[RPC]
	private void catchPlayerUpdateFromServer(Vector3 pos, NetworkViewID netID)
	{
		if(Network.isClient)
			NetworkView.Find(netID).gameObject.rigidbody2D.position = ((Vector2)pos);
	}

	[RPC]
	private void updateVelocity(Vector3 velo, NetworkMessageInfo info)
	{
		syncDelay = Time.time - lastSyncTime;
		lastSyncTime = Time.time;

		NetworkView.Find(info.networkView.viewID).transform.Translate(velo * syncDelay);
		NetworkView.Find(info.networkView.viewID).gameObject.rigidbody2D.velocity = (Vector2)publicStorage.addVectors(velo, (velo*syncDelay));
	}

	[RPC]
	private void shareControlInfo(bool leftPressed, bool rightPressed, bool isCrouching, NetworkViewID netID)
	{
		for(int i = 0; i < 4; i++)
		{
			if(playersInfo[i].netID == netID)
			{
				playersInfo[i].left = leftPressed;
				playersInfo[i].right = rightPressed;
				playersInfo[i].crouched = isCrouching;
				i = 10;
			}
		}
	}

	[RPC]
	private void stopRPC()
	{
		sendRPCs = false;
	}

	[RPC]
	private void plrDied()
	{
		isDead = true;
		Destroy(gameObject, 0.5f);
	}

	[RPC]
	private void shareNetIDs(int plrNum, NetworkViewID netID)
	{
		playersInfo[plrNum].netID = netID;
	}

	private void sendPlrPos()
	{
		for(int i = 0; i < 4; i++)
		{
			if(transform.Find("/Player"+(i+1).ToString()) && playersInfoAllocated)
			{
				Debug.Log(playersInfo[i].netID);
				networkView.RPC("catchPlayerUpdateFromServer", RPCMode.Others, playersInfo[i].pos*(syncDelay+1), playersInfo[i].netID);
			}
		}
	}

	private void serverUpdatePlayers()
	{
		for(int a = 1; a < 5; a++)
		{
			if(transform.Find("/Player"+a.ToString()))
			{
				GameObject plrObj = transform.Find("/Player"+a.ToString()).gameObject;
				//playersInfo[a-1].state = "small";
				playersInfo[a-1].pos = (Vector3)plrObj.rigidbody2D.position;
				playersInfo[a-1].velo = (Vector3)plrObj.rigidbody2D.velocity;
				//playersInfo[a-1].crouched = false;
				Debug.Log("Player"+a+" Updated!");
			}
		}
		if(!playersInfoAllocated)
			playersInfoAllocated = true;
	}

	private void syncPos()
	{
		/*
		syncTime += Time.deltaTime;
		rigidbody2D.velocity = Vector2.Lerp (rigidbody2D.velocity, rigidbody2D.velocity * (syncDelay+syncTime), syncTime / syncDelay);
		*/
	}

	void OnDestroy()
	{
		for(int i = 0; i < 4; i++)
		{
			if(publicStorage.refToPlayers[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
			{
				publicStorage.plrsInThisWorld--;
				publicStorage.refToPlayers[i] = null;

				for(int a = i; a < 3; a++)
				{
					publicStorage.refToPlayers[a] = publicStorage.refToPlayers[a+1];
				}

				publicStorage.refToPlayers[3] = null;
				break;
			}
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{

		Vector3 syncVelo = Vector3.zero;
		Vector3 syncPos = Vector3.zero;

		if(stream.isWriting)
		{
			syncVelo = (Vector3) rigidbody2D.velocity;
			stream.Serialize(ref syncVelo);
			//*if(Network.isServer)
			//{
				syncPos = (Vector3)rigidbody2D.position;
				stream.Serialize(ref syncPos);
			//}*/
		}

		else
		{
			stream.Serialize(ref syncVelo);
			rigidbody2D.velocity = (Vector2)syncVelo;
			stream.Serialize(ref syncPos);
			rigidbody2D.position = (Vector2)syncPos;
		}
	}

	// once per frame update timed with physic engine
	void FixedUpdate() 
	{
		if(!isDead)
		{
			if(publicStorage.localGame || publicStorage.refToNetManager.GetComponent<NetworkManager>().gameReady)
				playerMove();

			if(!publicStorage.localGame)
			{
				if(publicStorage.refToNetManager.GetComponent<NetworkManager>().gameReady && sendRPCs)
				{
					//networkView.RPC("shareControlInfo", RPCMode.Others, nhspeed == -1, phspeed == 1, Animator.StringToHash("Base Layer.playerCrouch") == anim.GetCurrentAnimatorStateInfo(0).nameHash, networkView.viewID);
				///*if(Network.isClient)
				//{
					if(countFrames >= 5)
					{
						networkView.RPC("shareControlInfo", RPCMode.Others, nhspeed == -1, phspeed == 1, Animator.StringToHash("Base Layer.playerCrouch") == anim.GetCurrentAnimatorStateInfo(0).nameHash, networkView.viewID);
						//networkView.RPC("updateVelocity", RPCMode.Server, (Vector3)rigidbody2D.velocity);
						countFrames = 0;
					}
					countFrames++;
				}
		//*/
				if(Network.isServer)
				{
					/*
					serverUpdatePlayers();
					if(countFrames > 15)
					{
						sendPlrPos();
						countFrames = 0;
					}
					countFrames++;
					/*
					if(!networkView.isMine)
					{
						//syncPos();
					}
					*/
				}
			}
		}
	}
}
