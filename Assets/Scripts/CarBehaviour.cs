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

		Destroy(GetComponent<iTween>());
		
		float rand = Random.value;
		if(rand > 0.8f)
			iTween.RotateTo(gameObject, new Vector3(90.0f, 0.0f, 0.0f), 0.3f);
		else if(rand > 0.6f)
			iTween.RotateTo(gameObject, new Vector3(-90.0f, 0.0f, 0.0f), 0.3f);
		else if(rand > 0.4f)
			iTween.RotateTo(gameObject, new Vector3(90.0f, gameObject.transform.rotation.eulerAngles.y, 0.0f), 0.3f);
		else
			iTween.RotateTo(gameObject, new Vector3(-90.0f, gameObject.transform.rotation.eulerAngles.y, 0.0f), 0.3f);
    }
}
