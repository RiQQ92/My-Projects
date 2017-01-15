using UnityEngine;
using System.Collections;

public class LifeSpanKill : MonoBehaviour {

	public float killTime;

	void Start () {
		Invoke ("KillObject", killTime);
	}

	void KillObject () {
		Destroy (gameObject);
	}

}
