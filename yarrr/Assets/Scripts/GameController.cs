using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{

	public Text scoreCounterText;
	public Text animalCounterText;
	private int animalCount;
    private int animalsInTheWorld;
    private int pointCount;

	int interval = 1; 
	float nextTime = 0;

	void Start()
    {
		animalCount = 10;
        animalsInTheWorld = animalCount;
        pointCount = 0;
	}
	
	void Update () {
		
		if (Time.time >= nextTime)
        {
			incrementPoints ();
			nextTime += interval;
            updateCounters();
        }
	}

    public void addAnimals(int value)
    {
        this.animalCount += value;
        updateAnimalsQuantityText();
    }

    public void animalKilled(int count)
    {
        animalsInTheWorld -= count;
        updateAnimalsQuantityText();

        if (animalsInTheWorld <= 0)
            SceneManager.LoadScene("Game Over");
    }

    public int getAnimalsCount()
    {
        return animalCount;
    }

	void updateCounters()
    {
		scoreCounterText.text = "" + pointCount;
	}

	void incrementPoints()
    {
		pointCount += 10 * animalCount;
	}

    void updateAnimalsQuantityText()
    {
        animalCounterText.text = animalCount + " / " + animalsInTheWorld;

    }
}
