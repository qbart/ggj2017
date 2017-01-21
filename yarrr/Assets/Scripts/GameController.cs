using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public Text scoreCounterText;
	public Text animalCounterText;
	public int animalCount;
	public int pointCount;

	int interval = 1; 
	float nextTime = 0;

	// Use this for initialization
	void Start () {
		animalCount = 3;
		pointCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Time.time >= nextTime) {
			incrementPoints ();
			nextTime += interval; 
		}
		updateCounters ();
	}

	void updateCounters(){
		scoreCounterText.text = "" + pointCount;
		animalCounterText.text = "" + animalCount;
	}

	void incrementPoints(){
		pointCount += 10 * animalCount;
	}
}
