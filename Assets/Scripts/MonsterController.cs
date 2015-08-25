using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour 
{
	public float health = 500.0f;
	float currentHealth;

	public float shakeDelayMin;
	public float shakeDelayMax;

	public float shakeTimeMin;
	public float shakeTimeMax;

	public float shakeSpeed;
	public float shakeAngleMin;
	public float shakeAngleMax;

	public float resetShakeTime;
	float inputTime;

	float shakeDelay;

	public float stepTime;
	public float stepLength;
	public float eyeMinX;
	public float eyeMaxX;
	public float damage;

	public Transform mouth;

	public GameObject redLaserPrefab;
	public GameObject redGlow;
	public GameObject greenLaserPrefab;
	public GameObject greenGlow;
	public GameObject gun;
	public float scrollSpeed;

	public GameObject redLaserEffectPrefab;
	public GameObject greenLaserEffectPrefab;

	public RectTransform healthUI;
	public GameObject finishUI;
	public Text pointsText;

	public int points = 0;

	public AudioClip [] fireClips;

	float initialHealthLength;

	bool moveForward;
	bool moveBackward;
	bool wrongInput = false;

	GameObject laser;
	bool wasRed;

	float laserInitialLength;
	Vector3 target;
	float currentScroll;

	float moveShakeRotationMult = 1.0f;
	bool shaking = false;
	float shakingStopTime;
	float nextShaking;

	KeyCode expectedKey = KeyCode.None;
	float expectedTime;

	GameObject redLaserEffect;
	GameObject greenLaserEffect;
	bool dead = false;

	float nextFireSoundsTime;
	bool nextShortSoundsTime = true;

	static MonsterController This;

	static public Vector3 MouthPosition()
	{
		return This.mouth.position;
	}

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

	void UpdateUI()
	{
		healthUI.sizeDelta = new Vector2(initialHealthLength * currentHealth / health, healthUI.sizeDelta.y);
		pointsText.text = points.ToString();
	}

	public static void BroadcastAll(string fun) 
	{
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach (GameObject go in gos) {
			if (go && go.transform.parent == null) {
				go.gameObject.BroadcastMessage(fun, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

	void PlayFireSounds()
	{
		GetComponent<AudioSource>().clip = fireClips[Random.Range(0, fireClips.Length)];
		GetComponent<AudioSource>().Play();

		nextFireSoundsTime = Time.time + (nextShortSoundsTime ? Random.Range(0.3f, 0.7f) : Random.Range(1.0f, 2.0f));
		nextShortSoundsTime = Random.value > 0.5f;
    }

	static IEnumerator ShowFinishUI(float waitTime, MonsterController mc)
	{
		yield return new WaitForSeconds(waitTime);
		mc.finishUI.SetActive(true);
	}
    
    public void DoDamageImpl(float damage)
	{
		if(dead) return;

		currentHealth -= damage;
		if(currentHealth < 0.0f) currentHealth = 0.0f;

		UpdateUI();

		if(currentHealth <= 0.0f)
        {
			dead = true;

           if(laser != null)
				Destroy(laser);

			redGlow.SetActive(false);
			greenGlow.SetActive(false);

			redLaserEffect.SetActive(false);
			greenLaserEffect.SetActive(false);

			Destroy(GetComponent<iTween>());
			iTween.ShakePosition(gameObject, new Vector3(1.0f, 4.0f, 0.5f), 1.5f);
            iTween.MoveTo(gameObject, iTween.Hash("position", transform.position + Vector3.up * 100.0f,
			                                      "delay", 1.5f,
			                                      "time", 2.0f));

			StartCoroutine(ShowFinishUI(3.0f, this));
			BroadcastAll("OnGameOver");
		}
	}

	public void Restart()
	{
		Application.LoadLevel(0);
	}
	static public void DoDamage(float damage)
	{
		This.DoDamageImpl(damage);
	}

	static public void AddPoints(int points)
	{
		This.points += points;
		This.UpdateUI();
	}

	public void HealImpl(float heal)
	{
		if(dead) return;
	
		currentHealth += heal;
		currentHealth = Mathf.Clamp(currentHealth, currentHealth, health);

		UpdateUI();
	}

	static public void Heal(float heal)
	{
		This.HealImpl(heal);
	}
	
	void Start () 
	{
		pointsText.text = "0";

		currentHealth = health;
		initialHealthLength = healthUI.sizeDelta.x;

		This = this;
		target = transform.position;

		redGlow.SetActive(false);
		greenGlow.SetActive(false);

		redLaserEffect = Instantiate(redLaserEffectPrefab);
		redLaserEffect.SetActive(false);

		greenLaserEffect = Instantiate(greenLaserEffectPrefab);
		greenLaserEffect.SetActive(false);
	}

	void Move()
	{
		if(Time.time > shakingStopTime)
		{
			iTween.RotateTo(gameObject, iTween.Hash("z", 0.0f, "time", 0.0f));
        }
		else
		{
			iTween.RotateTo(gameObject, iTween.Hash("z", Random.Range(shakeAngleMin, shakeAngleMax)*moveShakeRotationMult, "time", shakeSpeed, 
			                                        "oncomplete", "Move"));
			moveShakeRotationMult = -moveShakeRotationMult;
		}
    }

	void ShakeReset()
	{
		target = transform.position;

		Destroy(GetComponent<iTween>());
		iTween.ShakePosition(gameObject, new Vector3(1.0f, 2.0f, 0.5f), resetShakeTime);
	}

	void DoStep()
	{
		if(Time.time > nextShaking)
			shaking = false;

		if (moveForward)
		{
			target = transform.position + Vector3.right*stepLength;

			if(!shaking)
			{
				shakingStopTime = Time.time + Random.Range(shakeTimeMin, shakeTimeMax);
				shaking = true;

				Move();
				nextShaking = Time.time + Random.Range(shakeDelayMin, shakeDelayMax);
			}
			/*
			if(!doingShake)
			{
				Hashtable shakeParams = new Hashtable();
				shakeParams["amount"] = new Vector3(0.0f, 0.0f, 2.0f);
				shakeParams["oncomplete"]  = "OnShakeDone";
				shakeParams["time"] = stepTime * 0.5f;
				iTween.ShakeRotation(gameObject, shakeParams);

				doingShake = true;
			}
			*/
		}
		else if (moveBackward)
		{
			target = transform.position - Vector3.right*stepLength;

			if(!shaking)
			{
				shakingStopTime = Time.time + Random.Range(shakeTimeMin, shakeTimeMax);
				shaking = true;
				
				Move();
				nextShaking = Time.time + Random.Range(shakeDelayMin, shakeDelayMax);
            }
			/*
			if(!doingShake)
			{
				Hashtable shakeParams = new Hashtable();
				shakeParams["amount"] = new Vector3(0.0f, 0.0f, 2.0f);
				shakeParams["oncomplete"]  = "OnShakeDone";
				shakeParams["time"] = stepTime * 0.5f;
				iTween.ShakeRotation(gameObject, shakeParams);
				
				doingShake = true;
			}
*/
		}

		Hashtable moveParams = new Hashtable();
		moveParams["position"] = target;
		moveParams["oncomplete"]  = "OnStepDone";
		moveParams["time"] = stepTime;
	//	moveParams["ease"];
		iTween.MoveUpdate(gameObject, moveParams);

		moveForward = false;
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
		if(dead)
			return;

		bool expected = false;

		if(Time.time > inputTime)
		{
			if(Input.GetKeyDown(KeyCode.A))
			{
				expectedTime = Time.time + 0.3f;

				expected = true;
				if(expectedKey == KeyCode.None)
				{
					expectedKey = KeyCode.S;
					moveForward = true;
					moveBackward = false;
				}
				else if(expectedKey != KeyCode.A)
				{
					wrongInput = true;
					moveForward = false;
					moveBackward = false;
					expectedKey = KeyCode.None;
				}
			}
			if(Input.GetKeyDown(KeyCode.D))
			{
				expectedTime = Time.time + 0.3f;
				
				expected = true;
				if(expectedKey != KeyCode.D)
				{
					wrongInput = true;
					moveForward = false;
					moveBackward = false;
					expectedKey = KeyCode.None;
				}
                else
                {
                    expectedKey = KeyCode.None;
                    // if(moveBackward) expectedKey = KeyCode.A;
                }
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
				expectedTime = Time.time + 0.3f;
	            
				expected = true;
				if(expectedKey != KeyCode.S)
				{
					wrongInput = true;
					moveForward = false;
					moveBackward = false;
					expectedKey = KeyCode.None;
	            }
				else
				{
					expectedKey = KeyCode.D;
					//if(moveBackward) expectedKey = KeyCode.A;
				}
	        }
		}
        
		if(!expected && expectedKey != KeyCode.None)
        {
            if(Input.anyKeyDown || Time.time > expectedTime)
			{
				wrongInput = true;
				moveForward = false;
                moveBackward = false;
				expectedKey = KeyCode.None;
	        }
		}

		if(wrongInput)
		{
			inputTime = Time.time + resetShakeTime;
			ShakeReset();
			wrongInput = false;
		}
		else
		{
			DoStep();
		}
        
        Vector3 hitTarget = GetMouseWorld();
		RaycastHit hit;
		if(Physics.Raycast(new Ray(Camera.main.transform.position, (GetMouseWorld() - Camera.main.transform.position).normalized), out hit))
		{
			hitTarget = hit.point;
		}


		Vector3 dir = hitTarget - gun.transform.position;
		Vector3 eyePos = gun.transform.localPosition;
		eyePos.x = Mathf.Clamp(dir.x * 0.1f - 0.5f, eyeMinX, eyeMaxX);
		gun.transform.localPosition = eyePos;

//		dir.z = 0.0f;
		dir.Normalize();
		if(Input.GetMouseButton(0))
		{
			EnemyBehaviour enemyBeh = null;

			Vector3 hitPoint;
			if(Physics.Raycast(new Ray(gun.transform.position, dir), out hit))
			{
				enemyBeh = hit.collider.gameObject.GetComponent<EnemyBehaviour>();
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

			bool needRed = enemyBeh == null || enemyBeh.redLaser;
			if(laser != null && (wasRed && !needRed) || (!wasRed && needRed))
				Destroy(laser);

			if(laser == null)
			{
				laser = Instantiate(needRed ? redLaserPrefab : greenLaserPrefab);
				laser.GetComponent<Renderer>().sortingOrder = 1000;
				laserInitialLength = laser.GetComponent<Renderer>().bounds.extents.x*2.0f;
			}

			laser.transform.position = gun.transform.position + Vector3.back * 0.0f;

			float dist = Vector3.Distance (laser.transform.position, hitPoint);
			laser.transform.localScale = new Vector3(dist / laserInitialLength, 1.0f, 1.0f);
			laser.GetComponent<Renderer>().material.mainTextureScale = new Vector2(dist / laserInitialLength, 1.0f);
			laser.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(currentScroll, 0.0f);
			currentScroll += Time.deltaTime * scrollSpeed;
			laser.transform.right = dir;

			if(needRed)
			{
				redLaserEffect.SetActive(true);
				greenLaserEffect.SetActive(false);
				redLaserEffect.transform.position = hitPoint + Vector3.up*0.1f + Vector3.back*0.2f;

				redGlow.SetActive(true);
				greenGlow.SetActive(false);
				redGlow.transform.GetChild(0).gameObject.SetActive(Random.value > 0.5f);
					
				redGlow.GetComponent<Renderer>().sortingOrder = 2000;
			}
			else
			{
				greenLaserEffect.SetActive(true);
				redLaserEffect.SetActive(false);
				greenLaserEffect.transform.position = hitPoint + Vector3.up*0.1f + Vector3.back*0.2f;

				greenGlow.SetActive(true);
				redGlow.SetActive(false);
				greenGlow.transform.GetChild(0).gameObject.SetActive(Random.value > 0.5f);
				
				greenGlow.GetComponent<Renderer>().sortingOrder = 2000;
			}

			wasRed = needRed;

			if(Time.time > nextFireSoundsTime)
				PlayFireSounds();
		}
		if(Input.GetMouseButtonUp(0))
		{
			if(laser != null)
			{
				Destroy(laser);
				redLaserEffect.SetActive(false);
				greenLaserEffect.SetActive(false);
			}

			redGlow.SetActive(false);
			greenGlow.SetActive(false);
		}
	}

	void OnStepDone()
	{
	}

	void OnShakeDone()
	{
		//doingShake = false;
	}
}
