using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject follow;
	public float speed;

	Vector3 initialPos;
	Vector3 initialOffset;
	// Use this for initialization
	void Start () {
	
		initialPos = transform.position;
		initialOffset = follow.transform.position - initialPos;
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 target = initialPos;
		target.x = follow.transform.position.x - initialOffset.x;

		transform.position = Vector3.Lerp(transform.position, target, speed*Time.deltaTime);
	}
}
