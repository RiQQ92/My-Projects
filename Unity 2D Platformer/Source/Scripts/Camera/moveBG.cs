using UnityEngine;
using System.Collections;

public class moveBG : MonoBehaviour
{
	// holds speed returned by objToFind which is set in the Unity Inspector window instead of writing it somewhere, Drag N' Drop way to go
	private float xSpd;
	private float ySpd;
	private Vector2 netObj;
	private Vector2 objPos;
	
	public bool autoUpPos = true;
	public bool autoUpSpd = false;
	public bool manualUpdate = false;
	public string objToFind;

	private IEnumerator waitForInternet()
	{
		yield return new WaitForSeconds(0.5f);
		int plrNum = int.Parse (Network.player.ToString());
		if(plrNum == 0)
			if(transform.parent.name.ToString() != "BG_Camera1")
				Destroy(transform.parent.gameObject);

		netObj = transform.Find("/Player"+(plrNum+1).ToString()).position;
	}

	// updates the texture offset to make illusion of moving BG
	public void updateTextureManual(float velocityX, float velocityY)
	{
		if(manualUpdate)
			renderer.material.mainTextureOffset = new Vector2(renderer.material.mainTextureOffset.x + (velocityX/60) * Time.deltaTime, renderer.material.mainTextureOffset.y + (velocityY/60) * Time.deltaTime);
	}

	public void updareTextureManual(Vector2 from, Vector2 to, float time)
	{
		if(manualUpdate)
		{
			to = new Vector2(to.x - from.x, to.y - from.y);
			from = new Vector2(renderer.material.mainTextureOffset.x, renderer.material.mainTextureOffset.y);
			
			renderer.material.mainTextureOffset = Vector2.Lerp(from, from + to, time/60);
		}
	}
	public void updareTextureManual(Vector3 pos)
	{
		if(manualUpdate)
		{
			renderer.material.mainTextureOffset = (Vector2)pos/100;
		}
	}

	private void updateTexture()
	{
		if(!manualUpdate)
			renderer.material.mainTextureOffset = new Vector2(renderer.material.mainTextureOffset.x + (xSpd/60) * Time.deltaTime, renderer.material.mainTextureOffset.y + (ySpd/60) * Time.deltaTime);
	}

	private void updateByPos()
	{	
		if(!manualUpdate)
		{
			if(Network.isClient || Network.isServer)
				renderer.material.mainTextureOffset = netObj/100;

			else
				renderer.material.mainTextureOffset = objPos/100;
		}
	}

	// when this object activates
	void Awake()
	{
		if(!publicStorage.Splitscreen)
		{
			if(transform.parent.name.ToString() != "BG_Camera1")
				Destroy(transform.parent.gameObject);
			
			transform.parent.camera.rect = new Rect(0,0,1,1);
		}
		if(Network.isServer || Network.isClient)
		{
			StartCoroutine(waitForInternet());
		}
	}

	// Initialization
	void Start()
	{
		objPos = new Vector2(0,0);
		xSpd = 0;
		ySpd = 0;
	}

	//per fram update timed with physics engine
	void LateUpdate ()
	{
		if(autoUpSpd)
		{
			xSpd = 0;
			ySpd = 0;

			// finds the object speed
			xSpd = transform.Find(objToFind).rigidbody2D.velocity.x;
			ySpd = transform.Find(objToFind).rigidbody2D.velocity.y;

			updateTexture();
		}

		if(autoUpPos)
		{
			if(!Network.isClient && !Network.isServer)
				if(transform.Find(objToFind) != null)
				objPos = (Vector2)transform.Find(objToFind).position;
			updateByPos();
		}
	}
}
