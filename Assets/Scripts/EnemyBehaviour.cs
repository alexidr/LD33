using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
	[HideInInspector]
	public GameObject targetObject;

	public float attackDistance;
	public float movingSpeed;
	public float flipDistance = 5.0f;
	public float health = 100.0f;
	
	bool playingDeath = false;

	// Use this for initialization
	void Start () 
	{
	
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
		}
		
		bool needFlip = (direction.x > 0.0f && transform.right.x < 0.0f) || (direction.x < 0.0f && transform.right.x > 0.0f);
		
		if(dist > flipDistance && needFlip)
			transform.right = direction.x > 0.0f ? Vector3.right : Vector3.left;
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

	public void OnGettingHit(float damage)
	{
		health -= damage*Time.deltaTime;
	}

	public void PlayDeath()
	{
		playingDeath = true;
		Death();
	}
}
