using UnityEngine;
using System.Collections;

public class CowBehaviour : EnemyBehaviour 
{
	enum State
	{
		Hiding,
		ShowingUp,
		Finished,
		Cought
	}

	public Vector3 initialOffset;

	public float heal = 50.0f;

	public float showUpDistance = 10.0f;
	public float showUpTime = 1.0f;
	public float hideTime = 1.0f;
	public float showUpHeight = 0.0f;
	public float waitingTime = 2.0f;
	public float eatingTime = 0.2f;
	public float eatingRotationSpeed = 10.0f;

	Vector3 initialPos;
	float hidingTime;
	State state;

	void Start()
	{
		transform.position = transform.position + initialOffset;

		initialPos = transform.position;
		state = State.Hiding;
	}

	void Update()
	{
		if(state == State.Hiding)
		{
			if(transform.position.x - targetObject.transform.position.x < showUpDistance)
			{
				state = State.ShowingUp;

				Vector3 targetPos = transform.position;
				targetPos.y = showUpHeight;
				iTween.MoveTo(gameObject, targetPos, showUpTime);

				hidingTime = Time.time + waitingTime;
			}
		}
		if(state == State.ShowingUp && Time.time > hidingTime)
		{
			state = State.Finished;
			iTween.MoveTo(gameObject, initialPos, hideTime);
		}
	}
	
	static IEnumerator KillSelf(float waitTime, GameObject go)
	{
		yield return new WaitForSeconds(waitTime);
		
		Destroy(go);
	}
	
	override public void PlayDeath()
	{
		MonsterController.Heal(heal);
		MonsterController.AddPoints(points);

		state = State.Cought;
		iTween.MoveTo(gameObject, iTween.Hash("position", MonsterController.MouthPosition(), 
		                                      "time", eatingTime,
		                                      "easetype", iTween.EaseType.easeInQuad));

		iTween.RotateAdd(gameObject, iTween.Hash("z", eatingRotationSpeed,
		                                        "time", eatingTime,
		                                        "easetype", iTween.EaseType.easeInQuad));

		StartCoroutine(KillSelf(eatingTime, gameObject));
	}
}
