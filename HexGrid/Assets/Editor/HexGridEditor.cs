using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using nz.Rishaan.HexGrid;

[CustomEditor (typeof (HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGrid h = target as HexGrid;

        if (GUILayout.Button("Replace Hex Tile Colliders With Mesh Colliders"))
        {
            h.SetToMeshCol();
        }

        if (GUILayout.Button("Merge"))
        {
            h.Merge();
        }

        if (GUILayout.Button("Show Children"))
        {
            for (int i = 0; i < h.transform.childCount; ++i)
            {
                h.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private void OnSceneGUI()
    {
        HexGrid h = target as HexGrid;
        if (Handles.Button(h.transform.position + Vector3.up * 1.5f, Quaternion.LookRotation(Vector3.up), 1f, 1f, Handles.ArrowHandleCap)) {
            h.Merge();
        }
        if (Handles.Button(h.transform.position + Vector3.up * 1.5f, Quaternion.LookRotation(Vector3.up), 0.5f, 0.5f, Handles.SphereHandleCap))
        {
            for (int i = 0; i < h.transform.childCount; ++i)
            {
                h.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
