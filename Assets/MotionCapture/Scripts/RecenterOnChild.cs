using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterOnChild : MonoBehaviour 
{
    [SerializeField]
    string recenterButton;
    [SerializeField]
    Transform childTarget;
    [SerializeField]
    Vector3 initalChildOffset;
    [SerializeField]
    Quaternion initalChildRotation;

    void Awake () 
	{
        initalChildOffset = childTarget.position;
       // initalChildRotation = childTarget.rotation;		
	}
	
	void Update () 
	{
        if (Input.GetButtonDown(recenterButton)) {
            transform.position = childTarget.position - initalChildOffset;
            //transform.rotation = Quaternion.Inverse(initalChildRotation) * childTarget.rotation;
        }
	}
}
