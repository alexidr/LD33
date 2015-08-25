using UnityEngine;
using System.Collections;

public class CarBehaviour : EnemyBehaviour
{
	public float shakeTime = 0.15f;
	public float shakeMinAngle = 5.0f;
	public float shakeMaxAngle = 10.0f;

	public AudioClip[] movingSounds;
	public float movingSoundsDelay = 0.5f;
	public AudioClip[] deathSounds;

	public bool moveLeft = true;
	float yRotation;

	float moveShakeRotationMult = 1.0f;

	static IEnumerator PlaySounds(float waitTime, CarBehaviour mc)
	{
		yield return new WaitForSeconds(waitTime);
		Sounds.PlaySounds(mc.gameObject, mc.movingSounds);
	}

	void Start()
	{
		ShakeMove();
		yRotation = transform.rotation.eulerAngles.y;

		StartCoroutine(PlaySounds(movingSoundsDelay, this));
	}

	void ShakeMove()
	{
		if(dead)
			return;

		iTween.RotateTo(gameObject, iTween.Hash("z", Random.Range(shakeMinAngle, shakeMaxAngle)*moveShakeRotationMult, 
		                                        "y", (moveLeft ? 0.0f : 180.0f) + yRotation,
		                                        "time", shakeTime, 
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
		transform.position = transform.position + (moveLeft ? Vector3.left : Vector3.right) * movingSpeed*Time.deltaTime;
	}

	void Update () 
	{
		if(!dead)
			Move();
	}

	override public void PlayDeath()
	{
		dead = true;
		MonsterController.AddPoints(points);

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

		Sounds.PlaySounds(gameObject, deathSounds);
    }
}
