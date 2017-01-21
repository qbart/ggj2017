using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public Transform backSpawnPoint;
    public Transform frontSpawnPoint;

    public GameObject[] animals;

    private float spawnForce = 5;

    public void spawnAnimal(bool fromBack)
    {
        Transform spawnPoint = fromBack ? backSpawnPoint : frontSpawnPoint;

        GameObject animal = Instantiate(animals[0]) as GameObject;
        Rigidbody2D body = animal.GetComponent<Rigidbody2D>();
        body.transform.position = spawnPoint.position;
        Vector3 impulse = spawnPoint.rotation * Vector3.one;
        body.AddForce(impulse * spawnForce, ForceMode2D.Impulse);
    }
}
