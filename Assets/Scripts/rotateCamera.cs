using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateCamera : MonoBehaviour {

	public float speedYaw = 200;
	public float speedPitch = 200;

    private Vector3 rotation;
    private MediaController controller;


    // Use this for initialization
    void Start () {
		controller = GameObject.Find("360VideoPlayer").GetComponent<MediaController>();
		rotation.x = transform.eulerAngles.x;
		rotation.y = transform.eulerAngles.y;
		rotation.z = transform.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            rotation.y += speedYaw * Input.GetAxis("Mouse X");
            rotation.x -= speedPitch * Input.GetAxis("Mouse Y");
            if (rotation.y < 0)
            {
                rotation.y += 360;
            }
            else if (rotation.y > 360)
            {
                rotation.y -= 360;
            }
            if (rotation.x < 0)
            {
                rotation.x += 360;
            }
            else if (rotation.x > 360)
            {
                rotation.x -= 360;
            }
            transform.eulerAngles = rotation;
            controller.SetCameraRotation(rotation);
        }
    }

    public Vector3 GetRotation()
    {
        return rotation;
    }

    public void SetRotation(Vector3 rot)
    {
        rotation = rot;
    }

}
