using UnityEngine;
using System.Collections;

public class CarBehaviour : EnemyBehaviour
{
	bool dead;

	void Move()
	{
		transform.position = transform.position + Vector3.left*movingSpeed*Time.deltaTime;
	}

	void Update () 
	{
		if(!dead)
			Move();
	}

	override public void PlayDeath()
	{
		Destroy(gameObject);
    }
}
