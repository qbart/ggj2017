using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxItem : MonoBehaviour
{
	public float speed = -2.0f;

	void Start()
	{
	}

	void Update()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// if paralax item reached point where it should respawn (moved at the end)
		if (other.CompareTag("RespawnTrigger"))
		{
			Paralax paralax = GetComponentInParent<Paralax>();
			paralax.onParalaxItemNeedsUpdate(gameObject);
		}
	}
}