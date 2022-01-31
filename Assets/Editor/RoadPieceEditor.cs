using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadPiece))]
[CanEditMultipleObjects]
public class RoadPieceEditor : Editor
{
    SerializedProperty roadConnections;

    void OnEnable()
    {
        roadConnections = serializedObject.FindProperty("roadConnections");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(roadConnections);

        serializedObject.ApplyModifiedProperties();
    }

    static public void RunForChild(TwoDirectionRoad road)
    {
        // RoadPiece rp = road;
        // foreach (RoadPiece.RoadConnection connect in rp.roadConnections)
        // {
        //     EditorGUILayout.LabelField("test");
        // }
    }
}
