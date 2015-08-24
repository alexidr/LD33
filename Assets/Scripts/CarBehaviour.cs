using UnityEngine;
using System.Collections;

public class CarBehaviour : EnemyBehaviour
{
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
		dead = true;
		Destroy(gameObject);
    }
}
