using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TwoDirectionRoad))]
public class TwoDirectionRoadEditor : Editor
{

    // SerializedProperty roadConnections;

    // void OnEnable()
    // {
    //     roadConnections = serializedObject.FindProperty("roadConnections");
    // }

    // public override void OnInspectorGUI()
    // {
    //     EditorGUILayout.ObjectField(null, )
    //     // TwoDirectionRoad road = target as TwoDirectionRoad;
    //     // serializedObject.Update();

    //     // EditorGUILayout.PropertyField(roadConnections);

    //     // serializedObject.ApplyModifiedProperties();
    // }
    // // public override void OnInspectorGUI()
    // // {
    // //     RoadPieceEditor.RunForChild(target as TwoDirectionRoad);
    // // }
}
