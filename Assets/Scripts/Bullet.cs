using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float damage;
	public float speed;
	[HideInInspector]
	public Vector3 direction;

	// Use this for initialization
	void Start () {
	
		direction = -transform.right;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = transform.position + direction * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		MonsterController mc = MonsterController.FindMonster(other.gameObject);
		if(mc != null)
		{
			mc.DoDamage(damage);
		}

		Destroy(gameObject);
	}
}
