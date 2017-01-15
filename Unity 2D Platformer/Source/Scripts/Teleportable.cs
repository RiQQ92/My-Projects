using UnityEngine;
using System.Collections;

public class Teleportable : MonoBehaviour
{
	public bool rdyToTeleport = true;
	public bool cooledDown = true;
	private float coolDown = 0.5f;

	private IEnumerator wait()
	{
		yield return new WaitForSeconds(coolDown);
		cooledDown = true;
	}

	public void startCooldown()
	{
		cooledDown = false;
		StartCoroutine(wait());
	}
}
