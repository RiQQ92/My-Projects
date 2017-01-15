using UnityEngine;
using System.Collections;

public class followAnyone : MonoBehaviour
{

	private Transform personToFollow;
	private Transform BG;
	public float time;
	
	private Vector3 whereTo;
	private Vector3 myLastStopPos;
	private Vector2 targetStopPos;

	private string PTF = "";
	private string BGname = "";

	private int frameCounter;

	private float xMargin;
	private float yMargin;
	private float wait;
	private float stoppingTime;

	private bool waitingForLocal;
	private bool needToSetBG;
	private bool needToSetPTF;
	private bool targetStopped;
	private bool stopCamera;

	private IEnumerator waitForLocal()
	{
		waitingForLocal = true;
		while(!publicStorage.lvlLoaded)
		{
			yield return new WaitForEndOfFrame();
		}

		if(needToSetPTF)
			personToFollow = transform.Find(PTF);

		if(needToSetBG)
			BG = transform.Find(BGname);

		BG.GetComponent<moveBG>().autoUpPos = false;
		BG.GetComponent<moveBG>().manualUpdate = true;

		needToSetPTF = false;
		needToSetBG = false;
		waitingForLocal = false;
		yield return true;
	}

	private IEnumerator waitForInternet()
	{
		yield return new WaitForSeconds(0.5f);
		int plrNum = int.Parse (Network.player.ToString());
		personToFollow = transform.Find ("/Player"+(plrNum+1).ToString());
		Debug.Log(personToFollow);
		Debug.Log(personToFollow.name);
	}

	public string playerToFollow
	{
		set
		{
			PTF = "/"+value;
			needToSetPTF = true;
			if(!waitingForLocal)
				StartCoroutine(waitForLocal());
		}
		get
		{
			return(personToFollow.name);
		}
	}

	public string BGToRoll
	{
		set
		{
			BGname = "/"+value;
			needToSetBG = true;
			if(!waitingForLocal)
				StartCoroutine(waitForLocal());
		}
		get
		{
			return(BG.name);
		}
	}

	// Use this for initialization
	void Awake ()
	{
		waitingForLocal = false;
		needToSetBG = false;
		needToSetPTF = false;

		PTF = "/Player1";
		BG = transform.Find("/BG&Scenery/BG_Camera1/Quad");
		targetStopped = false;
		stopCamera = false;

		frameCounter = 0;
		xMargin = 2;
		yMargin = 4;
		stoppingTime = 0;
		time = 8;
		wait = 0.5f;

		targetStopPos = Vector2.zero;
		myLastStopPos = Vector3.zero;
		whereTo = new Vector3(0, 0, transform.position.z); 

		if(!publicStorage.localGame)
		{
			BG.GetComponent<moveBG>().autoUpPos = false;
			BG.GetComponent<moveBG>().manualUpdate = true;

			StartCoroutine(waitForInternet());
		}
		else if(!publicStorage.splitCamInstantiated && publicStorage.Splitscreen)
		{
			gameObject.name = "Main Camera1";
			playerToFollow = "Player1";
			BGToRoll = "BG&Scenery/BG_Camera1/Quad";
			publicStorage.splitCamInstantiated = true;
			gameObject.camera.rect = new Rect(0f,0.5f,1f,0.5f);
			transform.GetComponent<setBoxToCamSize>().updateBox();
			//gameObject.SetActive(false);
			GameObject go = (GameObject) Instantiate(gameObject);
			go.name = "Main Camera2";
			go.GetComponent<followAnyone>().playerToFollow = "Player2";
			go.GetComponent<followAnyone>().BGToRoll = "BG&Scenery/BG_Camera2/Quad";
			go.GetComponent<AudioListener>().enabled = false;
			go.camera.rect = new Rect(0f,0f,1f,0.5f);
		}
		else
			playerToFollow = "Player1";
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!targetStopped && !stopCamera)
		{
			frameCounter++;

			if(frameCounter >= 2)
			{
				if(transform.position == myLastStopPos)
				{
					frameCounter = 0;
					targetStopped = true;
					stoppingTime = Time.time;
				}
				myLastStopPos = transform.position;
			}
		}
		else if(targetStopped && !stopCamera)
		{
			if(GetComponent<restrictMovement>().yEdge && GetComponent<restrictMovement>().xEdge)
			{;}
			else if(GetComponent<restrictMovement>().yEdge)
			{
				if(transform.position.x == myLastStopPos.x)
				{
					if(stoppingTime+wait < Time.time)
					{
						stopCamera = true;
						targetStopPos = (Vector2)personToFollow.transform.position;
					}
				}
				else
					targetStopped = false;
			}
			else if(GetComponent<restrictMovement>().xEdge)
			{
				if(transform.position.y == myLastStopPos.y)
				{
					if(stoppingTime+wait < Time.time)
					{
						stopCamera = true;
						targetStopPos = (Vector2)personToFollow.transform.position;
					}
				}
				else
					targetStopped = false;
			}
			else
			{
				if(transform.position == myLastStopPos)
				{
					if(stoppingTime+wait < Time.time)
					{
						stopCamera = true;
						targetStopPos = (Vector2)personToFollow.transform.position;
					}
				}
				else
					targetStopped = false;
			}
		}
		else if(stopCamera)
		{
			if(targetStopped)
			{
				if(targetStopPos.x - xMargin > personToFollow.transform.position.x || targetStopPos.x + xMargin < personToFollow.transform.position.x
				   || targetStopPos.y - yMargin > personToFollow.transform.position.y || targetStopPos.y + yMargin < personToFollow.transform.position.y)
					targetStopped = false;
			}
			else
			{
				targetStopped = false;
				
				BG.GetComponent<moveBG>().updareTextureManual(transform.position);
				whereTo = new Vector3(personToFollow.position.x, personToFollow.position.y, transform.position.z);
				transform.position = Vector3.Lerp(transform.position, whereTo, Time.deltaTime* time);

				if(transform.position.x <= personToFollow.position.x +0.05f && transform.position.x >= personToFollow.position.x -0.05f
				   && transform.position.y <= personToFollow.position.y +0.05f && transform.position.y >= personToFollow.position.y -0.05f)
				{
					stopCamera = false;
				}
				else if(transform.position.x <= personToFollow.position.x +0.05f && transform.position.x >= personToFollow.position.x -0.05f && GetComponent<restrictMovement>().yEdge)
				{
					stopCamera = false;
				}
				else if(transform.position.y <= personToFollow.position.y +0.05f && transform.position.y >= personToFollow.position.y -0.05f && GetComponent<restrictMovement>().xEdge)
				{
					stopCamera = false;
				}
				else if(GetComponent<restrictMovement>().xEdge && GetComponent<restrictMovement>().yEdge)
				{
					stopCamera = false;
				}
			}
		}
		if(BG != null && !stopCamera)
			BG.GetComponent<moveBG>().updareTextureManual(transform.position);

		//if(!publicStorage.localGame)
		if(personToFollow != null && !stopCamera)
			transform.position = new Vector3(personToFollow.position.x, personToFollow.position.y, transform.position.z);
	}
}
