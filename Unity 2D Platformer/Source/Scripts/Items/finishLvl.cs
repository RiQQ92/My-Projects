using UnityEngine;
using System.Collections;

public class finishLvl : MonoBehaviour
{
	private bool plrCollision = false;
	
	private IEnumerator wait()
	{
		yield return new WaitForSeconds(0.1f);
		plrCollision = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Player" && !plrCollision)
		{
			if(col.gameObject.name == "HeadCollider")
				col.transform.parent.gameObject.GetComponent<plrControl>().lvlFinishd();
			else
				col.gameObject.GetComponent<plrControl>().lvlFinishd();

			plrCollision = true;
			StartCoroutine(wait());
		}
	}
}
