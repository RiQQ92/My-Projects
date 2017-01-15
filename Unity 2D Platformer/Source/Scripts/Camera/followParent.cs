using UnityEngine;
using System.Collections;

public class followParent : MonoBehaviour
{
	public float time;

	private Vector3 whereTo;
	private Vector2 parentStopPos;
	
	private float xMargin;
	private float yMargin;
	private float wait;
	private float stoppingTime;

	private bool parentStopped;
	private bool stopCamera;

	private Transform BG1;
	private Transform BG2;


	// Use this for initialization
	void Start ()
	{
		BG1 = transform.Find("/BG&Scenery/BG_Camera1/Quad");
		BG2 = transform.Find("/BG&Scenery/BG_Camera2/Quad");


		parentStopped = false;
		stopCamera = false;
		
		xMargin = 2;
		yMargin = 4;
		stoppingTime = 0;
		time = 8;
		wait = 0.5f;

		parentStopPos = new Vector2(0,0);
		whereTo = new Vector3(0, 0, transform.localPosition.z); 
	}
	
	// Update is called once per frame
	void Update ()
	{

		// if parent facing left and this facing right => turn around
		if(transform.localPosition.x != 0 && transform.parent.localScale.x < 0 && transform.localScale.x > 0)
		{
			transform.localPosition = new Vector3(transform.localPosition.x *-1, transform.localPosition.y, transform.localPosition.z);
			transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
		}
		//if parent facing right and this facing left => turn around
		else if(transform.localPosition.x != 0 && transform.parent.localScale.x > 0 && transform.localScale.x < 0)
		{
			transform.localPosition = new Vector3(transform.localPosition.x *-1, transform.localPosition.y, transform.localPosition.z);
			transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
		}

		if(transform.parent.rigidbody2D.velocity.x == 0 && transform.parent.rigidbody2D.velocity.y == 0 && !parentStopped && !stopCamera)
		{
			parentStopped = true;
			stoppingTime = Time.time;
		}
		else if(parentStopped && !stopCamera)
		{
			if(transform.parent.rigidbody2D.velocity.x == 0 && transform.parent.rigidbody2D.velocity.y == 0)
			{
				if(stoppingTime+wait < Time.time)
				{
					stopCamera = true;
					parentStopPos = new Vector2(transform.parent.localPosition.x, transform.parent.localPosition.y);
				}
			}
			else
				parentStopped = false;
		}
		else if(stopCamera)
		{
			if(transform.localPosition.x < xMargin && transform.localPosition.x > -xMargin && transform.localPosition.y < yMargin && transform.localPosition.y > -yMargin && parentStopped)
			{
				transform.localPosition = new Vector3 ((parentStopPos.x - transform.parent.localPosition.x) *transform.localScale.x, parentStopPos.y - transform.parent.localPosition.y, transform.localPosition.z);
			}
			else if(transform.localPosition.x < xMargin && transform.localPosition.x > -xMargin && parentStopped && GetComponent<restrictMovement>().yEdge)
			{
				transform.localPosition = new Vector3 ((parentStopPos.x - transform.parent.localPosition.x) *transform.localScale.x, transform.localPosition.y, transform.localPosition.z);
			}
			else if(transform.localPosition.y < yMargin && transform.localPosition.y > -yMargin && parentStopped && GetComponent<restrictMovement>().xEdge)
			{
				transform.localPosition = new Vector3 (transform.localPosition.x, parentStopPos.y - transform.parent.localPosition.y, transform.localPosition.z);
			}
			else
			{
				parentStopped = false;
				if(transform.parent.name == "Player1")
					BG1.GetComponent<moveBG>().updareTextureManual(transform.position + transform.localPosition);
				else
					BG2.GetComponent<moveBG>().updareTextureManual(transform.position + transform.localPosition);
				/*
				if(GetComponent<restrictMovement>().xEdge && GetComponent<restrictMovement>().yEdge)
				{
					if(transform.parent.name == "Player1")
						transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(0, 0), new Vector2(0, 0), Time.deltaTime* time);
					else
						transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(0, 0), new Vector2(0, 0), Time.deltaTime* time);
				}
				else if(GetComponent<restrictMovement>().xEdge)
				{
					if(transform.parent.name == "Player1")
						transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(0, transform.localPosition.y), whereTo, Time.deltaTime* time);
					else
						transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(0, transform.localPosition.y), whereTo, Time.deltaTime* time);
				}
				else if(GetComponent<restrictMovement>().yEdge)
				{
					if(transform.parent.name == "Player1")
						transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(transform.localPosition.x, 0), whereTo, Time.deltaTime* time);
					else
						transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updareTextureManual(new Vector2(transform.localPosition.x, 0), whereTo, Time.deltaTime* time);
				}
				else
				{
					if(transform.parent.name == "Player1")
						transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updareTextureManual(transform.localPosition, whereTo, Time.deltaTime* time);
					else
						transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updareTextureManual(transform.localPosition, whereTo, Time.deltaTime* time);
				}
				*/
				transform.localPosition = Vector3.Lerp(transform.localPosition, whereTo, Time.deltaTime* time);
				if(transform.localPosition.x <= 0.05f && transform.localPosition.x >= -0.05f && transform.localPosition.y <= 0.05f && transform.localPosition.y >= -0.05f)
				{
					stopCamera = false;
					transform.localPosition = new Vector3(0,0,transform.localPosition.z);
				}
				else if(transform.localPosition.x <= 0.05f && transform.localPosition.x >= -0.05f && GetComponent<restrictMovement>().yEdge)
				{
					stopCamera = false;
					transform.localPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
				}
				else if(transform.localPosition.y <= 0.05f && transform.localPosition.y >= -0.05f && GetComponent<restrictMovement>().xEdge)
				{
					stopCamera = false;
					transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
				}
			}
		}


		if(!stopCamera)
		{
			transform.localPosition = new Vector3(0,0,transform.localPosition.z);
			if(transform.parent.name == "Player1")
				BG1.GetComponent<moveBG>().updareTextureManual(transform.position + transform.localPosition);
			else
				BG2.GetComponent<moveBG>().updareTextureManual(transform.position + transform.localPosition);
			/*
			if(GetComponent<restrictMovement>().xEdge && GetComponent<restrictMovement>().yEdge)
			{
				if(transform.parent.name == "Player1")
					transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updateTextureManual(0, 0);
				else
					transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updateTextureManual(0, 0);
			}
			else if(GetComponent<restrictMovement>().xEdge)
			{
				if(transform.parent.name == "Player1")
					transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updateTextureManual(0, transform.parent.rigidbody2D.velocity.y);
				else
					transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updateTextureManual(0, transform.parent.rigidbody2D.velocity.y);
			}
			else if(GetComponent<restrictMovement>().yEdge)
			{
				if(transform.parent.name == "Player1")
					transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updateTextureManual(transform.parent.rigidbody2D.velocity.x, 0);
				else
					transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updateTextureManual(transform.parent.rigidbody2D.velocity.x, 0);
			}
			else
			{
				if(transform.parent.name == "Player1")
					transform.Find("/BG&Scenery/BG_Camera1/Quad").GetComponent<moveBG>().updateTextureManual(transform.parent.rigidbody2D.velocity.x, transform.parent.rigidbody2D.velocity.y);
				else
					transform.Find("/BG&Scenery/BG_Camera2/Quad").GetComponent<moveBG>().updateTextureManual(transform.parent.rigidbody2D.velocity.x, transform.parent.rigidbody2D.velocity.y);
			}
			*/
		}
	}
}
