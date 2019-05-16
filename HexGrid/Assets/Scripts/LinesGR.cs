using UnityEngine;
using System.Collections.Generic;

public class LinesGR : MonoBehaviour {

	
	private Mesh ms;
	private Material smat;

    Vector3 offset;

	void Start () {	
        offset = new Vector3(0, 0.2f, 0);
    }

    public void Create(Vector3[] points, float width) {
        ms = new Mesh();
        for (int i = 1; i < points.Length; ++i) {
            AddLine(ms, MakeQuad(points[i-1] + offset, points[i] + offset, width), false);
        }
        this.GetComponent<MeshFilter>().sharedMesh = ms;
    }


	
	Vector3[] MakeQuad(Vector3 s, Vector3 e, float w) {
		w = w / 2;
		Vector3[] q = new Vector3[4];
        Vector3 dir = e - s;
        Vector3 par = (Quaternion.Euler(0,90,0)*dir).normalized;
		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();
		
		q[0] = transform.InverseTransformPoint(s + par * w);
		q[1] = transform.InverseTransformPoint(s + par * -w);
		q[2] = transform.InverseTransformPoint(e + par * w);
		q[3] = transform.InverseTransformPoint(e + par * -w);

		return q;
	}
	
	void AddLine(Mesh m, Vector3[] quad, bool tmp) {
			int vl = m.vertices.Length;
			
			Vector3[] vs = m.vertices;
			if(!tmp || vl == 0) vs = resizeVertices(vs, 4);
			else vl -= 4;
			
			vs[vl] = quad[0];
			vs[vl+1] = quad[1];
			vs[vl+2] = quad[2];
			vs[vl+3] = quad[3];
			
			int tl = m.triangles.Length;
			
			int[] ts = m.triangles;
			if(!tmp || tl == 0) ts = resizeTriangles(ts, 6);
			else tl -= 6;
			ts[tl] = vl;
			ts[tl+1] = vl+1;
			ts[tl+2] = vl+2;
			ts[tl+3] = vl+1;
			ts[tl+4] = vl+3;
			ts[tl+5] = vl+2;
			
			m.vertices = vs;
			m.triangles = ts;
			m.RecalculateBounds();
	}

	public void PopPoint() {
		var verts = ms.vertices;
		var tris = ms.triangles;

		ms.Clear();

		if (verts.Length > 4) {
			ms.vertices = downshiftVertices(verts, 4);
			ms.triangles = downshiftTriangles(tris, 6);
		} else {
			ms.vertices = null;
			ms.triangles = null;
		}

		ms.RecalculateBounds();
	}
	
	Vector3[] downshiftVertices(Vector3[] ovs, int shift) {
		Vector3[] nvs = new Vector3[ovs.Length - shift];
		for (int i = 0; i < nvs.Length; ++i) nvs[i] = ovs[i+shift];
		return nvs;
	}

	int[] downshiftTriangles(int[] ovs, int shift) {
		int[] nvs = new int[ovs.Length - shift];
		for (int i = 0; i < nvs.Length; ++i) nvs[i] = ovs[i+shift] - 4;
		return nvs;
	}

	Vector3[] resizeVertices(Vector3[] ovs, int ns) {
		Vector3[] nvs = new Vector3[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
	int[] resizeTriangles(int[] ovs, int ns) {
		int[] nvs = new int[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
}







