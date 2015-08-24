﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldersBehaviour : EnemyBehaviour 
{
	static List<float> distances = new List<float>();

	public float attackDistanceMin;
	public float attackDistanceMax;
	float attackDistance;

	public float flipDistance = 5.0f;
	public float shakeTime = 0.15f;
	public float shakeMinAngle = 5.0f;
	public float shakeMaxAngle = 10.0f;

	public float damageShakeTime = 0.15f;
	public float damageShakeMinAngle = 5.0f;
	public float damageShakeMaxAngle = 10.0f;

	public float damage;

	public Transform firePoint;
	public GameObject bullet;
	public float shootFrequency = 1.0f;

	bool playingDeath = false;
	float nextShootTime;
	bool moving = false;

	float moveShakeRotationMult = 1.0f;
	int fireShakeState = 0;

	static void RemoveDistance(float dist)
	{
		distances.Remove(dist);
	}
	static float PickDistance(float min, float max, float size)
	{
		for(int i = 50; i > 0; --i)
		{
			float dist = Random.Range(min, max);
			bool found = true;
			foreach(float used in distances)
			{
				if(dist + size > used && dist - size < used)
				{
					found = false;
					break;
				}
			}

			if(found) 
			{
				distances.Add(dist);
				return dist;
			}
		}

		Debug.LogError("here");
		return Random.Range(min, max);
	}

    // Use this for initialization
	void Start () 
	{
		//iTween.RotateTo(gameObject, new iTween. new Vector3(0.0f, 0.0f, 90.0f), 100000.0f);
		nextShootTime = 0.0f;

		attackDistance = PickDistance(attackDistanceMin, attackDistanceMax, 2.0f);
	}

	void StopMove()
	{
		Destroy(GetComponent<iTween>());
		iTween.RotateTo(gameObject, iTween.Hash("z", 0.0f, "time", 0.0f));
	}
	
	void Move()
	{
		iTween.RotateTo(gameObject, iTween.Hash("z", Random.Range(shakeMinAngle, shakeMaxAngle)*moveShakeRotationMult, "time", shakeTime, 
		                                        "oncomplete", "Move"));
		moveShakeRotationMult = -moveShakeRotationMult;
	}

	void FireShake()
	{
		if(fireShakeState == -1) return;

		if(fireShakeState == 0)
		{
			iTween.RotateTo(gameObject, iTween.Hash("z", Random.Range(damageShakeMinAngle, damageShakeMaxAngle), "time", damageShakeTime, 
			                                        "oncomplete", "FireShake"));
			fireShakeState = 1;
		}
		else if(fireShakeState == 1)
		{
			iTween.RotateTo(gameObject, iTween.Hash("z", 0.0f, "time", damageShakeTime, 
			                                        "oncomplete", "FireShake"));
			fireShakeState = -1;
		}
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

			if(!moving)
				Move();

			moving = true;
			fireShakeState = -1;
		}
		else
		{
			if(moving)
				StopMove();

			if(Time.time > nextShootTime)
			{
				nextShootTime = Time.time + 1.0f / shootFrequency;

				fireShakeState = 0;
				FireShake();

				if(bullet != null)
					Instantiate(bullet, firePoint.position, firePoint.rotation);
				else
					MonsterController.FindMonster(targetObject).DoDamage(damage);
			}

			moving = false;
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

		RemoveDistance(attackDistance);
	}
}
