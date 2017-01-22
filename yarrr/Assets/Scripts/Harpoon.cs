using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    const float MIN_PLUNGER_ALIVE_TIME = 1.0f;


    public LayerMask animalLayer;
    public Pirate pirate;
    public Transform plungerSpawnPoint;
    public GameObject plunger;
    public SpringJoint2D ropeJoint;
	AudioSource audio;

	public AudioClip sndWithdraw;

    float plungerMinimumAliveTimeBeforeDetaching;
    GameObject activeBullet = null;
    LineRenderer lineRenderer;
    bool withdrawRope;
    bool playReelAnim;
    int framePirateReel;

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

        if (framePirateReel < 12 && playReelAnim)
        {
            pirate.setFrame(pirate.spriteReel[framePirateReel / 2]);
            framePirateReel++;
        }
        else
        {
            playReelAnim = false;

            if (angle <= 0)
                pirate.setFrame(pirate.spriteDown[clampi((int)Mathf.Abs(angle / 7.5f), 0, 5)]);
            else
                pirate.setFrame(pirate.spriteUp[clampi((int)(angle / 7.5f), 0, 5)]);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (activeBullet == null)
            {
                playReelAnim = false;
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
                framePirateReel = 0;
            }
            else
            {
                playReelAnim = true;
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

    int clampi(int x, int a, int b)
    {
        if (x < a)
            return a;
        if (x > b)
            return b;

        return x;
    }
}
