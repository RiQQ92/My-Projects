using UnityEngine;
using System.Collections;

public class setBoxToCamSize : MonoBehaviour {

	// Use this for initialization
	void Awake ()
	{
		BoxCollider2D box = GetComponent<BoxCollider2D>();
		float height = 2f * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;
		box.size = new Vector2(width, height);
	}

	public void updateBox()
	{
		Debug.Log("UpdatedCamSize");
		BoxCollider2D box = GetComponent<BoxCollider2D>();
		float height = 2f * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;
		box.size = new Vector2(width, height);
	}
}
