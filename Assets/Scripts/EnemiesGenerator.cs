using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesGenerator : MonoBehaviour {

	public GameObject monster;
	public float groundHeight;

	public GameObject[] levelEnemies;
	public int maxSpawnedUnits;

	public float spawnDistance;
	public float maxZOffset;
	public float attackDistance;

	public float minSpawnTime;
	public float maxSpawnTime;

	float nextSpawnTime;
	List<EnemyBehaviour> spawned = new List<EnemyBehaviour>();

	// Use this for initialization
	void Start () 
	{
		nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > nextSpawnTime && spawned.Count < maxSpawnedUnits)
		{
			nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

			GameObject enemy = levelEnemies[Random.Range(0, levelEnemies.Length)];
			GameObject spawnedEnemy = Instantiate(enemy);
			Vector3 startPos = monster.transform.position + 
				Vector3.right * spawnDistance + 
				Vector3.forward*Random.Range(-maxZOffset,maxZOffset) +
				Vector3.up*0.1f;

			startPos.y = groundHeight;

			spawnedEnemy.transform.position = startPos;

	
			EnemyBehaviour eb = spawnedEnemy.GetComponent<EnemyBehaviour>();
			eb.targetObject = monster;
			spawned.Add(eb);
		}

		for(int i = 0; i < spawned.Count; ++i)
		{
			if(spawned[i].health <= 0.0f)
			{
				spawned[i].PlayDeath();
				spawned.RemoveAt(i);
				--i;
			}
		}
	}
}
