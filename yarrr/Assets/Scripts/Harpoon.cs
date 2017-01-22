using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    const float MIN_PLUNGER_ALIVE_TIME = 1.0f;

    public LayerMask animalLayer;
    public Transform plungerSpawnPoint;
    public GameObject plunger;
    public SpringJoint2D ropeJoint;
	AudioSource audio;

	public AudioClip sndWithdraw;

    float plungerMinimumAliveTimeBeforeDetaching;
    GameObject activeBullet = null;
    LineRenderer lineRenderer;
    bool withdrawRope;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
		audio = GetComponent<AudioSource>();
        activeBullet = null;
        withdrawRope = false;
    }

	void Update()
    {
        if (withdrawRope)
        {
            if (ropeJoint.distance >= 0.01f)
            {
                ropeJoint.distance = 0.005f;
            }
            else
            {

                withdrawRope = false;
            }
        }

        if (activeBullet != null)
        {
            plungerMinimumAliveTimeBeforeDetaching -= Time.deltaTime;
            Vector3 point = ropeJoint.transform.position;
            point.z = -5;
            lineRenderer.SetPosition(0, point);
            point = activeBullet.transform.position;
            point.z = -5;
            lineRenderer.SetPosition(1, point);
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 objectPos = plungerSpawnPoint.position;
        Vector3 position;
        position.x = objectPos.x - mousePosition.x;
        position.y = objectPos.y - mousePosition.y;

        float angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, -45, 45);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        
        if (Input.GetMouseButtonDown(0))
        {
            if (activeBullet == null)
            {
                // spawn plunger
                plungerMinimumAliveTimeBeforeDetaching = MIN_PLUNGER_ALIVE_TIME;
                withdrawRope = false;
                Vector3 direction = mousePosition - plungerSpawnPoint.position;
                direction.Normalize();

                GameObject bullet = Instantiate(plunger, plungerSpawnPoint.position, transform.rotation);
                Rigidbody2D body = bullet.GetComponent<Rigidbody2D>();
                ropeJoint.connectedBody = body;
                ropeJoint.autoConfigureConnectedAnchor = false;
                ropeJoint.distance = 6;
                ropeJoint.connectedAnchor = new Vector2(1, 0);
                body.AddForce(direction * 150, ForceMode2D.Impulse);
                activeBullet = bullet;
                lineRenderer.enabled = true;
            }
            else
            {
				audio.PlayOneShot (sndWithdraw);
                withdrawRope = true;
            }
        }
	}

    public bool canPlungerBeDetached()
    {
        return plungerMinimumAliveTimeBeforeDetaching <= 0;
    }

    public void detachPlunger()
    {
		audio.Stop();
        activeBullet = null;
        lineRenderer.enabled = false;
    }
}
