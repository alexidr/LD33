using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesGenerator : MonoBehaviour {

	public GameObject monster;
	public float groundHeight;

	public GameObject[] levelEnemies;
	public int maxSpawnedUnits;

	public float spawnDistance;
	public float minZOffset;
	public float maxZOffset;

	public float attackDistanceMin;
	public float attackDistanceMax;
	public float unitSize;

	public float minSpawnTime;
	public float maxSpawnTime;

	float nextSpawnTime;
	List<EnemyBehaviour> spawned = new List<EnemyBehaviour>();
	GameObject [] slots;

	float GetAttackDistance(GameObject go)
	{
		List<int> freeSlots = new List<int>();
		for(int i = 0; i < slots.Length; ++i)
			if(slots[i] == null)
				freeSlots.Add(i);

		if(freeSlots.Count == 0)
		{
			Debug.LogError("oops");
			return Random.Range(attackDistanceMin, attackDistanceMax);
		}


		int index = freeSlots[Random.Range(0, freeSlots.Count)];
		slots[index] = go;
		return attackDistanceMin + index * unitSize;
	}

	void ReleaseSlot(GameObject go)
	{
		for(int i = 0; i < slots.Length; ++i)
		{
			if(slots[i] == go)
			{
				slots[i] = null;
				break;
			}
		}
	}

	// Use this for initialization
	void Start () 
	{
		nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

		int slotsCount = (int)((attackDistanceMax - attackDistanceMin) / unitSize);
		slots = new GameObject[slotsCount];
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(spawned.Count >= maxSpawnedUnits)
			nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

		if(Time.time > nextSpawnTime && spawned.Count < maxSpawnedUnits)
		{
			nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);

			GameObject enemy = levelEnemies[Random.Range(0, levelEnemies.Length)];
			GameObject spawnedEnemy = Instantiate(enemy);
			Vector3 startPos = monster.transform.position + 
				Vector3.right * spawnDistance + 
				Vector3.forward*Random.Range(minZOffset,maxZOffset) +
				Vector3.up*0.1f;

			startPos.y = groundHeight;

			spawnedEnemy.transform.position = startPos;

	
			EnemyBehaviour eb = spawnedEnemy.GetComponent<EnemyBehaviour>();
			eb.targetObject = monster;
			eb.attackDistance = GetAttackDistance(spawnedEnemy);

			spawned.Add(eb);
		}

		for(int i = 0; i < spawned.Count; ++i)
		{
			if(spawned[i] == null)
			{
				spawned.RemoveAt(i);
				--i;
				continue;
			}

			if(spawned[i].health <= 0.0f)
			{
				spawned[i].PlayDeath();
				ReleaseSlot(spawned[i].gameObject);

				spawned.RemoveAt(i);
				--i;
			}
		}
	}
}
