using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour 
{
	bool doingStep = false;
	bool doingShake = false;

	public float stepTime;
	public float stepLength;
	public float gunRotationTime;

	public GameObject gun;

	Vector3 target;

	// Use this for initialization
	void Start () 
	{
		target = transform.position;
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
		//Debug.LogError(dir);
		//iTween.RotateUpdate(gun, Quaternion.LookRotation(dir).eulerAngles, gunRotationTime);

		gun.transform.up = -dir;
	}

	void OnStepDone()
	{
	}

	void OnShakeDone()
	{
		doingShake = false;
	}
}
