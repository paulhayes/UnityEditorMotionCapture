using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PointTowards : MonoBehaviour 
{

    public Transform target;

	void Start () 
	{
		
	}
	
	void Update () 
	{
        if (target) {
            transform.LookAt(target,target.forward);
            
        }

    }
}
