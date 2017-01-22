using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour
{
    public GameController gameController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isAnimal = collision.CompareTag("Animal");
        bool isBullet = collision.CompareTag("Bullet");

        if (isAnimal || isBullet)
            Destroy(collision.gameObject);

        if (isAnimal)
            gameController.animalKilled(1);
    }
}
