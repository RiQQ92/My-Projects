using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public float maxHealth;
	private float currentHealth;
	[HideInInspector]
	public bool isDead;

	public int expGain;

	public RectTransform hpBar;

	void Start()
	{
		GameObject[] bars = GameObject.FindGameObjectsWithTag("Enemy Bars");
		
		foreach (GameObject obj in bars)
		{
			if(obj.name == "HP Bar" && gameObject == obj.transform.parent.parent.parent.gameObject)
			{
				hpBar = obj.GetComponent<RectTransform>();
			}
		}
		currentHealth = maxHealth;
	}

	public void TakeDamage(int damageAmount)
	{
		currentHealth -= damageAmount;
		hpBar.sizeDelta = new Vector2(currentHealth / maxHealth * 140, 35);
		Debug.Log("Took " + damageAmount + " damage. " + currentHealth + " health left.");
		if (currentHealth <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		isDead = true;
		GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>().Experience += expGain;
		Destroy(gameObject);
		Debug.Log("Dead");
	}
}
