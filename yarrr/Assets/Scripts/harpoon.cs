using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class harpoon : MonoBehaviour {
	// Use this for initialization
	public LayerMask animalLayer;

	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Input.mousePosition;

		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = objectPos.x - mousePos.x;
		mousePos.y = objectPos.y - mousePos.y;

		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		angle = Mathf.Clamp (angle, -45, 45);
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle)); 

		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition, 10, animalLayer);

			Debug.DrawRay(transform.position, mousePosition, Color.red);
			if(hit.collider != null){
				hit.rigidbody.MovePosition(hit.rigidbody.position * Time.deltaTime);
			}

		}
	}
}
