using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public Transform enemyShip;

	private float maxWidth = 9.67f;
	private float SpawnHeight = 12.88f;

	void SpawnRandomX ()
	{
		Transform enemy = Instantiate (enemyShip, new Vector3 (RandomX(), SpawnHeight, -0.1f), Quaternion.Euler(0, 0, 0)) as Transform;
		enemy.name = "Enemy";
		enemy.gameObject.GetComponentInChildren<Enemy> ().PathAnimation = "Fly_Down";
	}

	float RandomX()
	{
		return Random.Range (-maxWidth, maxWidth);
	}
}
