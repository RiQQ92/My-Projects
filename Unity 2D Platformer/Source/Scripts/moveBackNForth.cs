using UnityEngine;
using System.Collections;

public class moveBackNForth : MonoBehaviour
{
	public float moveDistanceX = 0;
	public float moveDistanceY = 0;
	public float moveSpeed = 0;
	[HideInInspector]
	public bool returning;

	private bool xNegative;
	private bool yNegative;
	private int countFrames;
	private Vector3 localStartPos;

	[RPC]
	private void updatePos(bool isReturning, Vector3 pos)
	{
		returning = isReturning;
		transform.localPosition = pos;
	}
	
	private void checkEnds()
	{
		if(xNegative && yNegative)
		{
			if(returning){
				if(localStartPos.x < transform.localPosition.x || localStartPos.y < transform.localPosition.y){
					returning = false;
				}
			}
			else{
				if(localStartPos.x + moveDistanceX > transform.localPosition.x || localStartPos.y + moveDistanceY > transform.localPosition.y){
					returning = true;
				}
			}
		}
		else if(xNegative)
		{
			if(returning){
				if(localStartPos.x < transform.localPosition.x || localStartPos.y > transform.localPosition.y){
					returning = false;
				}
			}
			else{
				if(localStartPos.x + moveDistanceX > transform.localPosition.x || localStartPos.y + moveDistanceY < transform.localPosition.y){
					returning = true;
				}
			}
		}
		else if(yNegative)
		{
			if(returning){
				if(localStartPos.x > transform.localPosition.x || localStartPos.y < transform.localPosition.y){
					returning = false;
				}
			}
			else{
				if(localStartPos.x + moveDistanceX < transform.localPosition.x || localStartPos.y + moveDistanceY > transform.localPosition.y){
					returning = true;
				}
			}
		}
		else
		{
			if(returning){
				if(localStartPos.x > transform.localPosition.x || localStartPos.y > transform.localPosition.y){
					returning = false;
				}
			}
			else{
				if(localStartPos.x + moveDistanceX < transform.localPosition.x || localStartPos.y + moveDistanceY < transform.localPosition.y){
					returning = true;
				}
			}
		}
	}

	private void movement()
	{
		if(returning)
			transform.Translate(new Vector3((-moveDistanceX/10) *Time.deltaTime *moveSpeed, (-moveDistanceY/10) *Time.deltaTime *moveSpeed, 0));
		else
			transform.Translate(new Vector3((moveDistanceX/10) *Time.deltaTime *moveSpeed, (moveDistanceY/10) *Time.deltaTime *moveSpeed, 0));

		checkEnds();
	}

	// Use this for initialization
	void Start ()
	{
		countFrames = 0;
		returning = false;
		if(moveDistanceX < 0)
			xNegative = true;
		else
			xNegative = false;
		if(moveDistanceY < 0)
			yNegative = true;
		else
			yNegative = false;

		localStartPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		movement();

		if(!publicStorage.localGame && Network.isServer && publicNetworkData.cooperative)
		{
			if(countFrames >= 60)
			{
				countFrames = 0;
				networkView.RPC("updatePos", RPCMode.Others, returning, transform.localPosition);
			}
			countFrames++;
		}
	}
}
