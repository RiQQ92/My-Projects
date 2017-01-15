using UnityEngine;
using System.Collections;

public class EnemyBurst : Enemy
{
    public float burstInterval = 0.33f; // aikaväli panoksilla sarjatulessa
    public int burstAmount = 3; // sarjan panosten määrä

	// Update is called once per frame
	void FixedUpdate ()
	{
		cooldown++;
		if(weaponCooldown <= cooldown)
		{
			burst();
			cooldown = 0;
		}
	}

    protected void burst() // <- yliajaa Enemy luokan shoot funktion
    {
        // base.shoot(); // <- lisää Enemy luokan shoot funktion tähän funktioon

		for(int i = 0; i < burstAmount; i++)
			Invoke ("shoot", burstInterval*i);

        // oma lisäys/muutos funktioon
    }
}
