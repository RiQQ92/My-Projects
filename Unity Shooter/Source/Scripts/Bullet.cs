using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public int Speed = 5;
	public int Damage = 5;
	public ParticleSystem expPrefab;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate ()
	{
		MoveForward ();
	}

	protected void DoDamage(GameObject target)
	{
		if(target.GetComponent<Health>() != null)
		{
			target.GetComponent<Health>().getDamage(Damage);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (gameObject.CompareTag("PlayerBullet"))
		{
			if (other.CompareTag("Enemy"))
			{
				if (expPrefab != null)
				{
					Instantiate (expPrefab, transform.position, Quaternion.identity);
				}
				DoDamage (other.gameObject);
				Destroy (gameObject);
			}
		}
		if (gameObject.CompareTag("EnemyBullet"))
		{
			if (other.CompareTag("Player"))
			{
				if (expPrefab != null)
				{
					Instantiate (expPrefab, transform.position, Quaternion.identity);
				}
				DoDamage (other.gameObject);
				Destroy (gameObject);
			}
		}
		// Do something with the object hit
	}
	
	void MoveForward ()
	{
		transform.Translate (new Vector3 (0, Speed * Time.deltaTime, 0));
	}
}
