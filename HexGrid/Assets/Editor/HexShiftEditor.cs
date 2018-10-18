using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using nz.Rishaan.HexGrid;

[CustomEditor(typeof(HexShift))]
public class HexShiftEditor : Editor
{
    public static bool selectionState = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexShift h = target as HexShift;

        string label = "";
        if (selectionState) label = "Move mode: duplicate and move";
        else label = "Move mode: Move selected";
        GUILayout.Label(label);
        if (GUILayout.Button("Toggle move mode")) selectionState = !selectionState;

        if (GUILayout.Button("Merge"))
        {
            h.GetComponentInParent<HexGrid>().Merge();
        }

        if (GUILayout.Button("Show Children"))
        {
            for (int i = 0; i < h.transform.childCount; ++i)
            {
                h.GetComponentInParent<HexGrid>().transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        
    }

    private void OnSceneGUI()
    {
        HexShift h = target as HexShift;
        if (Handles.Button(h.transform.position + new Vector3(0,1f,0), Quaternion.identity, 0.5f, 0.5f, Handles.SphereHandleCap)) selectionState = !selectionState;
        for (int i = 0; i < 6; ++i) {
            if (!selectionState)
            {
                if (Handles.Button(h.transform.position + new Vector3(0, 1f, 0), Quaternion.LookRotation(HexGrid.DIR[i]), 1f, 1f, Handles.ArrowHandleCap))
                {
                    h.shift(i);
                }
            } else {
                if (Handles.Button(h.transform.position + new Vector3(0, 1f, 0), Quaternion.LookRotation(HexGrid.DIR[i]), 2f, 2f, Handles.ArrowHandleCap))
                {
                    HexShift n = h.dupe();
                    n.shift(i);
                    Selection.activeTransform = n.transform;
                }
            }
        }
    }
}
