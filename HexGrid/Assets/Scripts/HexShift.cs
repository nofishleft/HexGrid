using nz.Rishaan.HexGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexShift : MonoBehaviour {

    public void shift (int dir) {
        this.transform.position += HexGrid.DIR[dir];
    }

    public static Transform BuildWall(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Material mat)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector3 l1 = Vector3.Lerp(a, b, 0.5f);
        Vector3 l2 = Vector3.Lerp(c, d, 0.5f);
        Vector3 l = Vector3.Lerp(l1, l2, 0.5f);

        a -= l;
        b -= l;
        c -= l;
        d -= l;

        vertices[0] = c; //b - par;
        vertices[1] = d; // b + par;
        vertices[2] = a; // a - par;
        vertices[3] = b; //a + par;

        mesh.vertices = vertices;

        int[] tri = new int[6];

        //  Lower left triangle.
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        //  Upper right triangle.   
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        Vector3 diff = -(l1 - l2).normalized;

        normals[0] = diff;
        normals[1] = diff;
        normals[2] = diff;
        normals[3] = diff;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        GameObject o = new GameObject();
        o.name = "Wall";
        MeshFilter mf = o.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        MeshRenderer mr = o.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        o.transform.position = l;
        return o.transform;

    }

    public static void BuildWall(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector3 l1 = Vector3.Lerp(a, b, 0.5f);
        Vector3 l2 = Vector3.Lerp(c, d, 0.5f);
        Vector3 l = Vector3.Lerp(l1, l2, 0.5f);

        a -= l;
        b -= l;
        c -= l;
        d -= l;

        vertices[0] = c; //b - par;
        vertices[1] = d; // b + par;
        vertices[2] = a; // a - par;
        vertices[3] = b; //a + par;

        mesh.vertices = vertices;

        int[] tri = new int[6];

        //  Lower left triangle.
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        //  Upper right triangle.   
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        Vector3 diff = -(l1 - l2).normalized;

        normals[0] = diff;
        normals[1] = diff;
        normals[2] = diff;
        normals[3] = diff;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        GameObject o = new GameObject();
        o.name = "Wall";
        MeshFilter mf = o.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        MeshRenderer mr = o.AddComponent<MeshRenderer>();
        o.transform.position = l;

    }

    public static void BuildWallBetween(Vector3 a, Vector3 b, Vector3 dir) {
        Mesh mesh = new Mesh();
        Vector3 par = (Quaternion.Euler(0,90,0) * dir).normalized * HexGrid.SIDE/2;
        Vector3[] vertices = new Vector3[4];
        Vector3 l =  Vector3.Lerp(a,b,0.5f);

        a -= l;
        b -= l;

        vertices[0] = b - par;
        vertices[1] = b + par;
        vertices[2] = a - par;
        vertices[3] = a + par;

        mesh.vertices = vertices;

        int[] tri = new int[6];

        //  Lower left triangle.
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        //  Upper right triangle.   
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];

        Vector3 diff = -(a - b);

        normals[0] = diff;
        normals[1] = diff;
        normals[2] = diff;
        normals[3] = diff;

        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.uv = uv;

        GameObject o = new GameObject();
        o.layer = 9;
        MeshFilter mf = o.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        MeshRenderer mr = o.AddComponent<MeshRenderer>();
        o.transform.position = l;

    }

    public HexShift dupe() {
        return (Instantiate(this.gameObject, this.transform.position, Quaternion.identity, this.transform.parent.transform) as GameObject).GetComponent<HexShift>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
