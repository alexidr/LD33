using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
	public bool redLaser = true;
	public float movingSpeed = 5.0f;
	public float health = 100.0f;

	[HideInInspector]
	public float attackDistance;
	[HideInInspector]
	public float fallHeight;

	[HideInInspector]
	public GameObject targetObject;

	protected bool dead;

	virtual public void OnGettingHit(float damage)
	{
		health -= damage*Time.deltaTime;
	}
	
	virtual public void PlayDeath()
	{
	}
}
