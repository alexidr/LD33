using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour 
{
	bool doingStep = false;
	bool doingShake = false;

	public float stepTime;
	public float stepLength;
	public float gunRotationTime;
	public float damage;

	public GameObject gun;

	Vector3 target;

	// Use this for initialization
	void Start () 
	{
		target = transform.position;
	}

	Vector3 drawTo;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(gun.transform.position, drawTo);
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
		//if (!doingStep)
		DoStep();

		Vector3 dir = GetMouseWorld() - gun.transform.position;
		dir.z = 0.0f;
		dir.Normalize();
		gun.transform.up = -dir;
	
		if(Input.GetMouseButton(0))
		{
			RaycastHit hit;
			if(Physics.Raycast(new Ray(gun.transform.position, dir), out hit))
			{
				drawTo = hit.point;
				EnemyBehaviour enemyBeh = hit.collider.gameObject.GetComponent<EnemyBehaviour>();
				if(enemyBeh != null)
				{
					enemyBeh.OnGettingHit(damage);
				}
			}
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
