using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour 
{
	public float movingSpeed = 5.0f;
	public float health = 100.0f;

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
