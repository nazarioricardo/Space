using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGate : MonoBehaviour {

	private Animation animations;
	private bool isOpen = false;

	// Use this for initialization
	void Start () {
		animations = GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OpenClose () {
		if (!isOpen) {
			animations.Play ("Open Gate");
			isOpen = true;
		} else {
			animations.Play ("Close Gate");
			isOpen = false;
		}
	}
}
