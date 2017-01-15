using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyAttackAI : MonoBehaviour {

	public int attackDamage;
	public float attackRange;
	[Range(0, 100)]
	public int accuracy;

	private Text accText;
	private Text dmgText;

	private GameObject player;

	public List<Vector3> directions;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		directions.Clear();
		directions.Add(new Vector3(0, 0, 1));
		directions.Add(new Vector3(0, 0, -1));
		directions.Add(new Vector3(1, 0, 0));
		directions.Add(new Vector3(-1, 0, 0));
	}

	void Start()
	{
		GameObject[] nums = GameObject.FindGameObjectsWithTag("Enemy Numbers");

		foreach (GameObject obj in nums)
		{
			if (obj.name == "DMG Bar" && gameObject == obj.transform.parent.parent.parent.gameObject)
			{
				dmgText = obj.GetComponent<Text>();
			}
			if (obj.name == "ACC Bar" && gameObject == obj.transform.parent.parent.parent.gameObject)
			{
				accText = obj.GetComponent<Text>();
			}
		}
		dmgText.text = attackDamage.ToString ();
		accText.text = accuracy.ToString ();
	}

	public bool AttemptAttack()
	{
		if(Mathf.Abs(Mathf.Abs(player.transform.position.x) - Mathf.Abs(this.transform.position.x)) <= attackRange*2 && Mathf.RoundToInt(player.transform.position.z) == Mathf.RoundToInt(this.transform.position.z) ||
		   Mathf.Abs(Mathf.Abs(player.transform.position.z) - Mathf.Abs(this.transform.position.z)) <= attackRange*2 && Mathf.RoundToInt(player.transform.position.x) == Mathf.RoundToInt(this.transform.position.x))
		{
            if (Random.Range(0, 100) <= accuracy)
                player.GetComponent<PHealth>().TakeDamage(attackDamage);
            else
                Debug.Log("Enemy missed");
			return true;
		}
		else
			return false;
	}

	int CalcDamage(float attack)
	{
		float randomValue = Random.Range(0f, attack);
		int damage = Mathf.RoundToInt(randomValue);
		if (damage == 0)
		{
			damage = 1;
		}
		return damage;
	}
}
