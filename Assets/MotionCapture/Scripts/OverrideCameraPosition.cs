using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideCameraPosition : MonoBehaviour 
{

    Vector3 position;
    Quaternion rotation;     

	void Awake () 
	{
        position = transform.position;
        rotation = transform.rotation;
	}
	
	void OnPreCull() 
	{
        transform.position = position;
        transform.rotation = rotation;
	}
}
