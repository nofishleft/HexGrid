using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using nz.Rishaan.HexGrid;
using System;

[CustomEditor(typeof(HexShift))]
public class HexShiftEditor : Editor
{
    public static string s = "step height";

    public static Vector3[] WallBuildingObjs;
    public static bool Clear = true;

    public static bool selectionState = true;

    public static float floor = 0f;

    public static Vector3 StoreDir;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexShift h = target as HexShift;

        GUILayout.Label("Step Height");
        s = GUILayout.TextField(s);
        if (GUILayout.Button("Set Step Height")) {
            try
            {
                HexGrid.StepHeight = float.Parse(s);
            }
            catch (FormatException e)
            {
                Debug.LogError("Invalid Step Height \"" + s + "\"");
            }
        }

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
        GUILayout.Label("Floor Level");
        try
        {
            floor = float.Parse(GUILayout.TextField(floor + ""));
        }
        catch (FormatException e) {
            
        }

        if (!Clear) {
            if (GUILayout.Button("Build To Floor")) {
                WallBuildingObjs[2] = new Vector3(WallBuildingObjs[0].x, floor, WallBuildingObjs[0].z);
                WallBuildingObjs[3] = new Vector3(WallBuildingObjs[1].x, floor, WallBuildingObjs[1].z);

                HexShift.BuildWall(WallBuildingObjs[0], WallBuildingObjs[1], WallBuildingObjs[2], WallBuildingObjs[3]);
                Clear = true;
                WallBuildingObjs = new Vector3[4];
            }
        }
        
    }

    private void OnSceneGUI()
    {
        HexShift h = target as HexShift;
        if (Handles.Button(h.transform.position + new Vector3(0, 1f, 0), Quaternion.LookRotation(Vector3.up), 1f, 1f, Handles.ArrowHandleCap)) {
            h.transform.position += new Vector3(0,HexGrid.StepHeight,0);
        }

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

            if (Handles.Button(h.transform.position + HexGrid.DIR[i] / 2, Quaternion.LookRotation(HexGrid.DIR[i]), 0.1f, 0.1f, Handles.SphereHandleCap))
            {
                if (Clear)
                {
                    if (WallBuildingObjs == null) WallBuildingObjs = new Vector3[4];
                    StoreDir = HexGrid.DIR[i];
                    Vector3 par = (Quaternion.Euler(0, 90, 0) * HexGrid.DIR[i]).normalized * HexGrid.SIDE / 2;
                    WallBuildingObjs[0] = h.transform.position + HexGrid.DIR[i] / 2 + par;
                    WallBuildingObjs[1] = h.transform.position + HexGrid.DIR[i] / 2 - par;

                    Clear = false;

                    Repaint();
                }
                else {
                    Vector3 par = (Quaternion.Euler(0, 90, 0) * HexGrid.DIR[i]).normalized * HexGrid.SIDE / 2;
                    WallBuildingObjs[2] = h.transform.position + HexGrid.DIR[i] / 2 - par;
                    WallBuildingObjs[3] = h.transform.position + HexGrid.DIR[i] / 2 + par;

                    HexShift.BuildWall(WallBuildingObjs[0], WallBuildingObjs[1], WallBuildingObjs[2], WallBuildingObjs[3]);
                    Clear = true;
                    WallBuildingObjs = new Vector3[4];
                }
            }
        }
    }
}
