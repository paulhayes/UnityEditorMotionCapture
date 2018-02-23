using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RecordAnimation : MonoBehaviour 
{
    
    public string filename;
    [SerializeField]
    string recordButton = "Fire1";

    [System.NonSerialized]
    public Animator animator;

    [SerializeField]
    public Transform[] transformsToTrack;

    [SerializeField]
    int capacity = 1024;

    [SerializeField]
    int framerate = 18;

    [System.NonSerialized]
    public int recorded = 0;

    [System.NonSerialized]
    public string[] paths;
    public List<Vector3>[] positions;
    public List<Quaternion>[] rotations;
    [System.NonSerialized]
    public List<float> times;

    bool toSave = false;
    float time;
    float lastTime;
    float interval;

    public bool SaveReady {
        get {
            return toSave;
        }     
    }


    void Start () 
	{
        animator = GetComponent<Animator>();
        interval = 1.0f / framerate;
        toSave = false;
        recorded = 0;
        if (filename == string.Empty) {
            filename = "animation";
        }
        paths = new string[transformsToTrack.Length];
        times = new List<float>(capacity);
        positions = new List<Vector3>[transformsToTrack.Length];
        rotations = new List<Quaternion>[transformsToTrack.Length];

        for (int i = 0; i < transformsToTrack.Length; i++) {
            var path = string.Empty;
            RecurseUpward(transformsToTrack[i],ref path);
            paths[i] = path;
            positions[i] = new List<Vector3>(capacity);
            rotations[i] = new List<Quaternion>(capacity);
            
        }
	}

    void LateUpdate()
    {
        if (Input.GetButtonUp(recordButton))
        {
            toSave = true;
        }

        if (Input.GetButtonDown(recordButton))
        {
            time = 0;
            lastTime = 0;
        }
        else {
            time += Time.deltaTime;

            if ((time - lastTime) < interval) {
                return;
            }
        }

        if (Input.GetButton(recordButton)) {
            
            times.Add(time);

            for (int i = 0; i < transformsToTrack.Length; i++)
            {
                var t = transformsToTrack[i];
                positions[i].Add(t.localPosition);
                rotations[i].Add(t.localRotation);
                

            }

            recorded++;
        }

        lastTime = time;
    }

    public void ClearRecording()
    {
        recorded = 0;
        time = 0;
        toSave = false;
        times.Clear();
        for (int i = 0; i < transformsToTrack.Length; i++)
        {
            positions[i].Clear();
            rotations[i].Clear();
        }
    }

    void RecurseUpward(Transform transform, ref string path) {
        if (transform == this.transform)
        {
            return;
        }
        else {
            path = transform.name + (path==string.Empty?"":"/"+path);
            RecurseUpward(transform.parent, ref path);
        }
    }
}
