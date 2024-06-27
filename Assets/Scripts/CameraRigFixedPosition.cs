using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraRigFixedPosition : MonoBehaviour {

	Vector3 Pos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Pos = InputTracking.GetLocalPosition(XRNode.Head);
		Vector3 newPos = new Vector3(0.0f - Pos.x, 0.0f - Pos.y, 0.0f - Pos.z);
		transform.position = newPos;
	}
}
