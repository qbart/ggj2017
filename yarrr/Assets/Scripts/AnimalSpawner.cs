﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public Transform backSpawnPoint;
    public Transform frontSpawnPoint;
    public Wave wave;

    public GameObject[] animals;

    private float spawnForce = 4;

    public void spawnAnimal(bool fromBack)
    {
        Transform spawnPoint = fromBack ? backSpawnPoint : frontSpawnPoint;
		int animalIndex = Random.Range (0, 3);
		GameObject animal = Instantiate(animals[animalIndex]) as GameObject;
        animal.GetComponent<SpawnedAnimal>().wave = wave;
        Rigidbody2D body = animal.GetComponent<Rigidbody2D>();
        body.transform.position = spawnPoint.position;
        Vector3 impulse = spawnPoint.rotation * Vector3.one;
        body.AddForce(impulse * spawnForce, ForceMode2D.Impulse);
    }
}
