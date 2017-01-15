using UnityEngine;
//using UnityEditor;
using System.Collections.Generic;

public class checkGround : MonoBehaviour
{
	public Transform target;
	//public string ScriptName;

	//private MonoScript Script;

	void Awake()
	{
		//Script = (MonoScript) target.gameObject.GetComponent(ScriptName).GetClass();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "baseGround" && transform.parent.gameObject.rigidbody2D.velocity.y <= 0.1)
		{
			target.gameObject.SendMessage("setIsGrounded");
		}
	}
	void OnTriggerStay2D(Collider2D col)
	{
		if(col.gameObject.tag == "baseGround" && transform.parent.gameObject.rigidbody2D.velocity.y <= 0.1)
		{
			target.gameObject.SendMessage("setIsGrounded");
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		if(col.gameObject.tag == "baseGround")
		{
			target.gameObject.SendMessage("setNotGrounded");
		}
	}
}