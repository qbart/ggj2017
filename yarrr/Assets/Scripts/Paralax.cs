using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
	public GameObject[] objects;

	private float singleItemWidth = 0;

	void Start()
	{
		for (int i = 0; i < objects.Length; ++i)
		{
			objects[i].GetComponent<Rigidbody2D>().velocity = new Vector3(objects[i].GetComponent<ParalaxItem>().speed, 0, 0);
		}

		// all objects need to have the same size
		singleItemWidth = objects[0].GetComponent<BoxCollider2D>().size.x;
	}

	public void onParalaxItemNeedsUpdate(GameObject item)
	{
		for (int i = 0; i < objects.Length; ++i)
		{
			if (objects[i].GetInstanceID() == item.GetInstanceID())
			{
				Vector3 oldPosition = objects[i].transform.position;
				Vector3 newPosition = oldPosition;
				newPosition.x += objects.Length * singleItemWidth;
				if (objects [i].CompareTag("Cloud")) {
					newPosition.y = Random.Range (-3, 3);
				}
					
				objects[i].transform.position = newPosition;
				break;
			}
		}

	}
}