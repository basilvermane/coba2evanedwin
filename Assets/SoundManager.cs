using UnityEngine.Audio;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public Sound[] sounds;

	public static SoundManager instance;

	// Use this for initialization
	void Awake () {

		if (instance == null)
			instance = this;
		else {
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);

		foreach (Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource> ();

			s.source.clip = s.clip;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;

		}
		
	}
	
	public void Play (string name){
		Sound s = Array.Find (sounds, sound => sound.name == name); 

		if (s == null) {
			Debug.Log ("Sound : " + name +" Not Found!");
			return;
		}
		s.source.Play ();
	}
}
