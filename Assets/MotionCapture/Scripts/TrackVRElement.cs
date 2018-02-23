using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
public class TrackVRElement : MonoBehaviour 
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public VRNode vrNode;

	void Start () 
	{
		
	}
	
	void Update () 
	{
        

        transform.position = InputTracking.GetLocalPosition(vrNode);
        transform.rotation = InputTracking.GetLocalRotation(vrNode);
	}

#endif
}
