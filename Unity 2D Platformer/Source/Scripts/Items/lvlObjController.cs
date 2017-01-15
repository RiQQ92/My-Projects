using UnityEngine;
using System.Collections;

public class lvlObjController : MonoBehaviour
{
	public bool empty = true;
	public bool boost = false;
	public string boostName = "";
	public Sprite afterHit;

	public bool multihitBox = false;
	public int hitAmount = 10;

	public bool activeOnlyBelow = false;
	public bool overrideSkin = false;
	public Sprite overSkin;
	
	public Transform mushroom;
	public Transform flower;
	public Transform leaf;

	private Vector3 collided;
	private int beenHit;
	private bool skinOverridden;
	private bool checkContent;
	private string objToGive;
	private Animator anim;

	public void sendRPC(bool grown, Vector3 pos)
	{
		networkView.RPC("punchNDestroy", RPCMode.Others, grown, pos);
		Debug.Log("RPC_Sent!");
	}

	[RPC]
	private void punchNDestroy(bool isGrown, Vector3 pos)
	{
		gotPunch(pos);
		destroy(isGrown);
		Debug.Log("RPC_Received!");
	}

	private IEnumerator instantObject(Transform prefab)
	{
		yield return new WaitForSeconds(0.4f);
		GameObject powerup;
		Transform t;
		if(publicStorage.localGame || !publicNetworkData.cooperative)
		{
			t = (Transform) Instantiate(prefab, transform.position, Quaternion.identity);
			powerup = t.gameObject;
			powerup.name = prefab.name;
			
			if((transform.position.x - collided.x) > 0)
				powerup.GetComponent<Powerup>().direction = true;
			else
				powerup.GetComponent<Powerup>().direction = false;
		}
		else if(Network.isServer && !publicStorage.localGame)
		{
			t = (Transform) Network.Instantiate(prefab, transform.position, Quaternion.identity, 10);
			powerup = t.gameObject;
			powerup.name = prefab.name;
			
			if((transform.position.x - collided.x) > 0)
				powerup.GetComponent<Powerup>().direction = true;
			else
				powerup.GetComponent<Powerup>().direction = false;
		}

		yield return true;
	}

	private void giveContent()
	{
		if(objToGive == "mushroom")
		{
			StartCoroutine(instantObject(mushroom));
		}
		else if(objToGive == "flower")
		{
			StartCoroutine(instantObject(mushroom));
		}
		else if(objToGive == "leaf")
		{
			StartCoroutine(instantObject(mushroom));
		}
		else
		{
			;
		}
	}

	private IEnumerator wait()
	{
		yield return new WaitForSeconds (0.18f);
		transform.FindChild("Particle Brick").particleSystem.Emit(8);
		yield return true;
	}

	// Use this for initialization
	public void gotPunch(Vector3 col)
	{
		anim.SetTrigger("gotPunch");
		collided = col;
	}

	public void destroy(GameObject obj)
	{
		if(activeOnlyBelow)
			GetComponent<BoxCollider2D>().isTrigger = false;
		
		if(empty && obj.GetComponent<plrControl>().isGrownup)
		{
			if(multihitBox)
			{
				if(beenHit >= hitAmount)
				{
					StartCoroutine(wait());
					Destroy(gameObject.collider2D, 0.1f);
					Destroy(transform.parent.collider2D);
					Destroy(gameObject.renderer, 0.18f);
					Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
					Destroy(transform.parent.gameObject, 3);
				}
				beenHit++;
			}
			else
			{
				StartCoroutine(wait());
				Destroy(gameObject.collider2D, 0.1f);
				Destroy(transform.parent.collider2D);
				Destroy(gameObject.renderer, 0.18f);
				Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
				Destroy(transform.parent.gameObject, 3);
			}
		}
		else if(!empty)
		{
			if(multihitBox)
			{
				if(beenHit >= hitAmount)
				{
					Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
					Destroy(transform.parent.FindChild("Particle Coin").gameObject, 2f);
					Destroy(transform.FindChild("Particle Brick").gameObject, 0.1f);
					Destroy(transform.FindChild("Particle Boost").gameObject, 2f);
					Destroy(transform.parent.collider2D, 0.4f);
					
					gameObject.GetComponent<SpriteRenderer>().sprite = afterHit;
				}
				giveContent();
				
				if(objToGive == "coin")
					transform.parent.FindChild("Particle Coin").particleSystem.Emit(1);
				
				transform.FindChild("Particle Boost").particleSystem.Emit(1);
				
				beenHit++;
			}
			else
			{
				Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
				Destroy(transform.parent.FindChild("Particle Coin").gameObject, 2f);
				Destroy(transform.FindChild("Particle Brick").gameObject, 0.1f);
				Destroy(transform.FindChild("Particle Boost").gameObject, 2f);
				Destroy(transform.parent.collider2D, 0.4f);
				
				gameObject.GetComponent<SpriteRenderer>().sprite = afterHit;
				giveContent();
				
				if(objToGive == "coin")
					transform.parent.FindChild("Particle Coin").particleSystem.Emit(1);
				
				transform.FindChild("Particle Boost").particleSystem.Emit(1);
			}
		}
	}

	public void destroy(bool grownUp)
	{
		if(activeOnlyBelow)
		{
			transform.parent.GetComponent<BoxCollider2D>().enabled = true;
			GetComponent<BoxCollider2D>().isTrigger = false;
		}
		
		if(empty && grownUp)
		{
			if(multihitBox)
			{
				if(beenHit >= hitAmount)
				{
					StartCoroutine(wait());
					Destroy(gameObject.collider2D, 0.1f);
					Destroy(transform.parent.collider2D);
					Destroy(gameObject.renderer, 0.18f);
					Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
					Destroy(transform.parent.gameObject, 3);
				}
				beenHit++;
			}
			else
			{
				StartCoroutine(wait());
				Destroy(gameObject.collider2D, 0.1f);
				Destroy(transform.parent.collider2D);
				Destroy(gameObject.renderer, 0.18f);
				Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
				Destroy(transform.parent.gameObject, 3);
			}
		}
		else if(!empty)
		{
			if(multihitBox)
			{
				if(beenHit >= hitAmount)
				{
					Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
					Destroy(transform.parent.FindChild("Particle Coin").gameObject, 2f);
					Destroy(transform.FindChild("Particle Brick").gameObject, 0.1f);
					Destroy(transform.FindChild("Particle Boost").gameObject, 2f);
					Destroy(transform.parent.collider2D, 0.4f);
					
					gameObject.GetComponent<SpriteRenderer>().sprite = afterHit;
				}
				giveContent();
				
				if(objToGive == "coin")
					transform.parent.FindChild("Particle Coin").particleSystem.Emit(1);
				
				transform.FindChild("Particle Boost").particleSystem.Emit(1);
				
				beenHit++;
			}
			else
			{
				Destroy(transform.parent.FindChild("punchCollider").gameObject, 0.1f);
				Destroy(transform.parent.FindChild("Particle Coin").gameObject, 2f);
				Destroy(transform.FindChild("Particle Brick").gameObject, 0.1f);
				Destroy(transform.FindChild("Particle Boost").gameObject, 2f);
				Destroy(transform.parent.collider2D, 0.4f);
				
				gameObject.GetComponent<SpriteRenderer>().sprite = afterHit;
				giveContent();
				
				if(objToGive == "coin")
					transform.parent.FindChild("Particle Coin").particleSystem.Emit(1);
				
				transform.FindChild("Particle Boost").particleSystem.Emit(1);
			}
		}
	}

	void Start ()
	{
		if(activeOnlyBelow)
		{
			transform.parent.GetComponent<BoxCollider2D>().enabled = false;
			GetComponent<BoxCollider2D>().isTrigger = true;
			transform.parent.FindChild("punchCollider").GetComponent<BoxCollider2D>().isTrigger = true;
		}

		beenHit = 0;
		if(overSkin == null)
			overSkin = Sprite.Create(new Texture2D(0,0), new Rect(0,0,0,0), new Vector2(0,0));

		skinOverridden = false;
		anim = GetComponent<Animator>();
		objToGive = "";
		checkContent = false;

		//if(!publicStorage.localGame)
		//	networkView.viewID = Network.AllocateViewID();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if(!checkContent)
		{
			if(empty && !boost)
			{
				boost = false;
				boostName = "";
				objToGive = "";
			}
			else if(!boost)
			{
				empty = false;
				boostName = "";
				objToGive = "coin";
			}
			else if(boost && boostName == "mushroom" || boost && boostName == "leaf" || boost && boostName == "flower")
			{
				empty = false;
				objToGive = boostName;
			}
			else
			{
				empty = false;
				boostName = "";
				boost = false;
				objToGive = "coin";
			}

			checkContent = true;
		}
		if(overrideSkin && !skinOverridden)
		{
			if(overSkin != null)
			{
				gameObject.GetComponent<SpriteRenderer>().sprite = overSkin;
			}

			skinOverridden = true;
		}
	}
}
