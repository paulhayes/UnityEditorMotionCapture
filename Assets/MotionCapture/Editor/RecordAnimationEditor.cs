using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor.Animations;

[InitializeOnLoad]
public class RecordAnimationEditor : EditorWindow 
{
    RecordAnimation[] recordAnimComponents;

    [MenuItem("Window/MotionRecorder")]
    static void Init() {
        var window = EditorWindow.GetWindow<RecordAnimationEditor>();
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Store Animations", EditorStyles.boldLabel);
        
    }

    static RecordAnimationEditor()
    {
        EditorApplication.playmodeStateChanged += OnPlaymodeChanged;
    }


    private void OnPlayStarted()
    {
        recordAnimComponents = GameObject.FindObjectsOfType<RecordAnimation>();
    }

    private void OnPlayStopped()
    {
        if (recordAnimComponents == null)
            return;
        recordAnimComponents = null;
    }

    private void SaveAnim(RecordAnimation recordAnim) {
        AnimatorController ac = recordAnim.animator.runtimeAnimatorController as AnimatorController;
        string controllerPath = AssetDatabase.GetAssetPath(ac);
        string path = Path.Combine(Path.GetDirectoryName(controllerPath), string.Format("{0}.asset", recordAnim.filename));
        var clip = new AnimationClip();
        //float lastTime = 0f;
        //int lastIndex = 0;

        var numTransforms = recordAnim.transformsToTrack.Length;
        

        for (int i = 0; i < numTransforms; i++)
        {
            Keyframe[,] keyframes = new Keyframe[recordAnim.recorded,7];
            for (int j = 0; j < recordAnim.recorded; j++)
            {
                Quaternion rot = recordAnim.rotations[i][j];
                Vector3 pos = recordAnim.positions[i][j];

                float time = recordAnim.times[j];
                for (int n = 0; n < 3; n++)
                {
                    keyframes[j,n] = new Keyframe(time, pos[n]);
                }
                for (int n = 0; n < 4; n++)
                {
                    keyframes[j, 3 + n] = new Keyframe(time, rot[n]);

                }

            }

            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localPosition.x", new AnimationCurve(keyframes.GetCol(0)));
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localPosition.y", new AnimationCurve(keyframes.GetCol(1)));
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localPosition.z", new AnimationCurve(keyframes.GetCol(2)));
            
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localRotation.x", new AnimationCurve(keyframes.GetCol(3)));
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localRotation.y", new AnimationCurve(keyframes.GetCol(4)));
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localRotation.z", new AnimationCurve(keyframes.GetCol(5)));
            clip.SetCurve(recordAnim.paths[i], typeof(Transform), "localRotation.w", new AnimationCurve(keyframes.GetCol(6)));


        }
        

        clip.EnsureQuaternionContinuity();
        var animPath = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(clip, animPath);

        string stateName = Path.GetFileNameWithoutExtension(animPath);
        var state = ac.layers[0].stateMachine.AddState(stateName);
        state.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
        
    }

    private static void OnPlaymodeChanged()
    {
        var window = EditorWindow.GetWindow<RecordAnimationEditor>();
        if (window == null)
            return;

        if (EditorApplication.isPlaying) {
            window.OnPlayStarted();
        }

        if (EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling)
            return;

        window.OnPlayStopped();
    }

    private void Update()
    {
        if (!EditorApplication.isPlaying || recordAnimComponents==null) {
            return;
        }
        foreach (var recordAnimComponent in recordAnimComponents)
        {
            if (recordAnimComponent.SaveReady) {
                SaveAnim(recordAnimComponent);
                recordAnimComponent.ClearRecording();
            }
        }
    }

}

public static class ArrayExt
{
    public static T[] GetCol<T>(this T[,] matrix, int col)
    {
        var colLength = matrix.GetLength(0);
        var colVector = new T[colLength];

        for (var i = 0; i < colLength; i++)
            colVector[i] = matrix[i, col];

        return colVector;
    }

    public static T[] GetRow<T>(this T[,] matrix, int col)
    {
        var rowLength = matrix.GetLength(1);
        var rowVector = new T[rowLength];

        for (var i = 0; i < rowLength; i++)
            rowVector[i] = matrix[i, col];

        return rowVector;
    }


}

