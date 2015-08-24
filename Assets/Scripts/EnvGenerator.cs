using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvGenerator : MonoBehaviour 
{
	public float baseOffsetMin = 0.0f;
	public float baseOffsetMax = 0.0f;

	public int maxObjectsAhead = 4;	
	public GameObject [] prefabs;

	GameObject [] currentObjects;

	GameObject GetInstance(int index)
	{
		GameObject obj = Instantiate(prefabs[index]) as GameObject;
		obj.transform.parent = transform;
		obj.transform.localRotation = prefabs[index].transform.rotation;
		
		return obj;
	}
	void MarkDeleted(GameObject obj)
	{
		Destroy(obj);
	}
	
	void Start()
	{
		currentObjects = new GameObject[maxObjectsAhead];
		
		Vector3 offset = Vector3.zero;
		for(int i = 0; i < maxObjectsAhead; ++i)
		{
			if(i != 0)
			{
				offset += Vector3.right * (currentObjects[i - 1].GetComponent<Collider>().bounds.extents.x*
					currentObjects[i - 1].transform.localScale.x + Random.Range(baseOffsetMin, baseOffsetMax));
			}

			int index = Random.Range(0, prefabs.Length);
			
			currentObjects[i] = GetInstance(index);
			currentObjects[i].transform.localPosition = offset;

			float width = currentObjects[i].GetComponent<Collider>().bounds.extents.x * currentObjects[i].transform.localScale.x;
			width += Random.Range(baseOffsetMin, baseOffsetMax);

			offset += Vector3.right * width;
		}
	}
	
	void Update()
	{
		int middleObject = maxObjectsAhead / 2;
		if(currentObjects[middleObject].transform.position.x < Camera.main.transform.position.x)
		{
			MarkDeleted(currentObjects[0]);
			for(int i = 0; i < maxObjectsAhead - 1; ++i)
			{
				currentObjects[i] = currentObjects[i + 1];
			}
			
			int index = Random.Range(0, prefabs.Length);
			GameObject newOne = GetInstance(index);
					
			float width = currentObjects[maxObjectsAhead - 2].GetComponent<Collider>().bounds.extents.x*
				currentObjects[maxObjectsAhead - 2].transform.localScale.x;
			width += newOne.GetComponent<Collider>().bounds.extents.x * newOne.transform.localScale.x;
			width += Random.Range(baseOffsetMin, baseOffsetMax);
			
			Vector3 anchor = currentObjects[maxObjectsAhead - 2].transform.localPosition;
			Vector3 pos = anchor + Vector3.right * width;
			
			currentObjects[maxObjectsAhead - 1] = newOne;
			currentObjects[maxObjectsAhead - 1].transform.localPosition = pos;
		}
		
	}

}
