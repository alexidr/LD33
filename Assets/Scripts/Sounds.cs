using UnityEngine;
using System.Collections;

public class Sounds: MonoBehaviour
{
	static public void PlaySounds(GameObject obj, AudioClip[] clips)
	{
		if(clips == null || clips.Length == 0) return;

		obj.GetComponent<AudioSource>().clip = clips[Random.Range(0, clips.Length)];
		obj.GetComponent<AudioSource>().Play();
	}

}
