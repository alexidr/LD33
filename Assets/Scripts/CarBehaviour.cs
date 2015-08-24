using UnityEngine;
using System.Collections;

public class CarBehaviour : EnemyBehaviour
{
	public float shakeTime = 0.15f;
	public float shakeMinAngle = 5.0f;
	public float shakeMaxAngle = 10.0f;

	float moveShakeRotationMult = 1.0f;

	void ShakeMove()
	{
		if(dead)
			return;

		iTween.RotateTo(gameObject, iTween.Hash("z", Random.Range(shakeMinAngle, shakeMaxAngle)*moveShakeRotationMult, "time", shakeTime, 
		                                        "oncomplete", "ShakeMove"));
		moveShakeRotationMult = -moveShakeRotationMult;
	}

	void StopMove()
	{
		Destroy(GetComponent<iTween>());
		iTween.RotateTo(gameObject, iTween.Hash("z", 0.0f, "time", 0.0f));
	}

	void Move()
	{
		transform.position = transform.position + Vector3.left*movingSpeed*Time.deltaTime;
	}

	void Start()
	{
		ShakeMove();
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
