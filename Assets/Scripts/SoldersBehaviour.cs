using UnityEngine;
using System.Collections;

public class SoldersBehaviour : EnemyBehaviour 
{
	public float attackDistance;
	public float flipDistance = 5.0f;

	public float damage;

	public Transform firePoint;
	public GameObject bullet;
	public float shootFrequency = 1.0f;

	bool playingDeath = false;
	float nextShootTime;
    
    // Use this for initialization
	void Start () 
	{
		//iTween.RotateTo(gameObject, new iTween. new Vector3(0.0f, 0.0f, 90.0f), 100000.0f);
		nextShootTime = 0.0f;
	}

	void Fight()
	{
		Vector3 targetPoint = transform.position;
		targetPoint.x = targetObject.transform.position.x;
		
		Vector3 direction = targetPoint - transform.position;
		float dist = direction.magnitude;
		direction.Normalize();
		
		if(dist > attackDistance)
		{
			transform.position = transform.position + direction*movingSpeed*Time.deltaTime;
			nextShootTime = Time.time + 1.0f / shootFrequency;
		}
		else if(Time.time > nextShootTime && bullet != null)
		{
			nextShootTime = Time.time + 1.0f / shootFrequency;

			Instantiate(bullet, firePoint.position, firePoint.rotation);
		}

		bool needFlip = (direction.x > 0.0f && transform.right.x > 0.0f) || (direction.x < 0.0f && transform.right.x < 0.0f);
		
		if(dist > flipDistance && needFlip)
			transform.right = direction.x > 0.0f ? Vector3.left : Vector3.right;
	}

	void Death()
	{
		float rand = Random.value;
		if(rand > 0.8f)
			iTween.RotateTo(gameObject, new Vector3(90.0f, 0.0f, 0.0f), 0.5f);
		else if(rand > 0.6f)
			iTween.RotateTo(gameObject, new Vector3(-90.0f, 0.0f, 0.0f), 0.5f);
		else if(rand > 0.4f)
			iTween.RotateTo(gameObject, new Vector3(90.0f, gameObject.transform.rotation.eulerAngles.y, 0.0f), 0.5f);
		else
			iTween.RotateTo(gameObject, new Vector3(-90.0f, gameObject.transform.rotation.eulerAngles.y, 0.0f), 0.5f);
	}

	void Update () 
	{
		if(!playingDeath)
			Fight();
	}

	virtual public void OnGettingHit(float damage)
	{
		health -= damage*Time.deltaTime;
	}

	override public void PlayDeath()
	{
		playingDeath = true;
		Death();
	}
}
