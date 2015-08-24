using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour 
{
	bool doingStep = false;
	bool doingShake = false;

	public float stepTime;
	public float stepLength;
	public float eyeMinX;
	public float eyeMaxX;
	public float damage;

	public GameObject redLaserPrefab;
	public GameObject redGlow;
	public GameObject greenLaserPrefab;
	public GameObject greenGlow;
	public GameObject gun;
	public float scrollSpeed;

	GameObject laser;
	float laserInitialLength;
	Vector3 target;
	float currentScroll;

	static public MonsterController FindMonster(GameObject inGO)
	{
		if(inGO == null)
		{
			return null;
		}
		
		MonsterController health = inGO.GetComponentInChildren(typeof(MonsterController)) as MonsterController;
		if (health == null)
		{
			GameObject parent = inGO;
			while (parent.transform.parent != null)
			{
				parent = parent.transform.parent.gameObject;
				health = parent.GetComponent(typeof(MonsterController)) as MonsterController;
				if (health != null)
				{
					break;
				}
			}
		}
		
		return health;
	}

	public void DoDamage(float damage)
	{

	}

	// Use this for initialization
	void Start () 
	{
		target = transform.position;

		redGlow.SetActive(false);
		greenGlow.SetActive(false);
	}

	void DoStep()
	{
		if (Input.GetKey (KeyCode.D))
		{
			target = transform.position + Vector3.right*stepLength;

			if(!doingShake)
			{
				Hashtable shakeParams = new Hashtable();
				shakeParams["amount"] = new Vector3(0.0f, 0.0f, 2.0f);
				shakeParams["oncomplete"]  = "OnShakeDone";
				shakeParams["time"] = stepTime * 0.5f;
				iTween.ShakeRotation(gameObject, shakeParams);

				doingShake = true;
			}
		}
		if (Input.GetKey (KeyCode.A))
		{
			target = transform.position - Vector3.right*stepLength;
			
			if(!doingShake)
			{
				Hashtable shakeParams = new Hashtable();
				shakeParams["amount"] = new Vector3(0.0f, 0.0f, 2.0f);
				shakeParams["oncomplete"]  = "OnShakeDone";
				shakeParams["time"] = stepTime * 0.5f;
				iTween.ShakeRotation(gameObject, shakeParams);
				
				doingShake = true;
			}
		}

		Hashtable moveParams = new Hashtable();
		moveParams["position"] = target;
		moveParams["oncomplete"]  = "OnStepDone";
		moveParams["time"] = stepTime;
		iTween.MoveUpdate(gameObject, moveParams);
    }

	Vector3 GetMouseWorld()
	{
		var v3 = Input.mousePosition;
		v3.z = transform.position.z - Camera.main.transform.position.z;
		return Camera.main.ScreenToWorldPoint(v3);
	}
    // Update is called once per frame
	void Update () 
	{
		DoStep();

		Vector3 dir = GetMouseWorld() - transform.position;
		Vector3 eyePos = gun.transform.localPosition;
		eyePos.x = Mathf.Clamp(dir.x * 0.1f - 0.5f, eyeMinX, eyeMaxX);
		gun.transform.localPosition = eyePos;

		dir.z = 0.0f;
		dir.Normalize();
		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Vector3 hitPoint;
			if(Physics.Raycast(new Ray(gun.transform.position, dir), out hit))
			{
				EnemyBehaviour enemyBeh = hit.collider.gameObject.GetComponent<EnemyBehaviour>();
				if(enemyBeh != null)
				{
					enemyBeh.OnGettingHit(damage);
				}

				hitPoint = hit.point;
			}
			else
			{
				hitPoint = gun.transform.position + dir * 100.0f;
			}

			if(laser == null)
			{
				laser = Instantiate(redLaserPrefab);
				laserInitialLength = laser.GetComponent<Renderer>().bounds.extents.x*2.0f;
			}

			laser.transform.position = gun.transform.position + Vector3.back * 0.1f;

			float dist = Vector3.Distance (laser.transform.position, hitPoint);
			laser.transform.localScale = new Vector3(dist / laserInitialLength, 1.0f, 1.0f);
			laser.GetComponent<Renderer>().material.mainTextureScale = new Vector2(dist / laserInitialLength, 1.0f);
			laser.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(currentScroll, 0.0f);
			currentScroll += Time.deltaTime * scrollSpeed;
			laser.transform.right = dir;

			redGlow.SetActive(true);
		}
		if(Input.GetMouseButtonUp(0))
		{
			if(laser != null)
				Destroy(laser);

			redGlow.SetActive(false);
			greenGlow.SetActive(false);
		}
	}

	void OnStepDone()
	{
	}

	void OnShakeDone()
	{
		doingShake = false;
	}
}
