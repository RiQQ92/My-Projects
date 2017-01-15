using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    void Start()
    {
        //transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
    }

	void Update ()
	{
		transform.LookAt(Camera.main.transform.position, Vector3.up);
	}
}
