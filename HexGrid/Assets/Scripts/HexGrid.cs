using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace nz.Rishaan.HexGrid {
    public class HexGrid : MonoBehaviour {
        //Properties
        public Mesh BASE;

        public Material DefaultWallMaterial = null;
        public Material TransparentClear = null;

        public static float StepHeight = 0.2f;

        public static float STEP;
        public static float AGENTHEIGHT;
        public static float AGENTRADIUS;
        public static float AGENTSLOPE;
        public static int AGENTTYPEID;

        public static float HEIGHT = 2f;
        public static float SIDE = HEIGHT / Mathf.Sqrt(3);
        public static float GAP = SIDE / 2;
        public static float WIDTH = 2 * SIDE;

        public static Vector3 NORTH = new Vector3(2f, 0, 0);
        public static Vector3 NORTHEAST = new Vector3(1f, 0, 3f * GAP);
        public static Vector3 SOUTHEAST = new Vector3(-1f, 0, 3f * GAP);
        public static Vector3 SOUTH = new Vector3(-2f, 0, 0);
        public static Vector3 SOUTHWEST = new Vector3(-1f, 0, -3f * GAP);
        public static Vector3 NORTHWEST = new Vector3(1f, 0, -3f * GAP);

        public static Vector3[] DIR = { NORTH, NORTHEAST, SOUTHEAST, SOUTH, SOUTHWEST, NORTHWEST };

        
        
        public static Vector3 Next(NavMeshAgent agent) {
            return agent.path.corners[0];
        }

        public void SetToMeshCol() {
            foreach (Collider col in Object.FindObjectsOfType<Collider>()) {
                if (col.gameObject.layer == 10) {
                    GameObject obj = col.gameObject;
                    if (obj.GetComponent<BoxCollider>() != null) DestroyImmediate(col);
                    if (obj.GetComponent<MeshCollider>() == null) obj.AddComponent<MeshCollider>();
                }
            }
        }

        public void AddWallMesh()
        {
            foreach (MeshRenderer mr in Object.FindObjectsOfType<MeshRenderer>())
            {
                if (mr.gameObject.layer == 10)
                {
                    if (mr.GetComponent<BoxCollider>() != null) DestroyImmediate(mr.GetComponent<BoxCollider>());
                    if (mr.GetComponent<MeshCollider>() == null) mr.gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        public static Vector3 ClosestVector(Vector3 v) {
            float a = Vector3.Angle(v, DIR[0]);
            int j = 0;
            for (int i = 1; i < 6; ++i) {
                float b = Vector3.Angle(v, DIR[i]);
                if (b < a)
                {
                    a = b;
                    j = i;
                }
            }
            return DIR[j];
        }

        private void Paint() {
            Material[] mats = new Material[2];
            mats[0] = DefaultWallMaterial;
            mats[1] = TransparentClear;

            foreach (MeshRenderer mr in GameObject.FindObjectsOfType<MeshRenderer>()) {
                if (mr.gameObject.name == "Wall") {
                    mr.gameObject.layer = 9;
                    mr.sharedMaterials = mats;
                    //mr.material = DefaultWallMaterial;
                    BoxCollider col = mr.gameObject.GetComponent<BoxCollider>();
                    if (col == null) {
                        mr.gameObject.AddComponent<BoxCollider>();
                    }
                }
            }
        }

        private void Start()
        {
            Paint();
            Merge();
            GetComponent<MeshFilter>().sharedMesh = AutoWeld(GetComponent<MeshFilter>().sharedMesh,0.0001f,1f);
            Bake();
        }

        private void Update()
        {
            //NavMeshPath path = new NavMeshPath();
            //NavMesh.CalculatePath(transform.position, new Vector3(2, 2), 1, path);
        }

        public static void UpdateHeight(float h)
        {
            HEIGHT = h;
            SIDE = HEIGHT / Mathf.Sqrt(3);
            GAP = SIDE / 2;
            WIDTH = 2 * SIDE;
            NORTH = new Vector3(0, 0, -2f);
            NORTHEAST = new Vector3(3f * GAP, 0, -1f);
            SOUTHEAST = new Vector3(3f * GAP, 0, 1f);
            SOUTH = new Vector3(0, 0, 2f);
            SOUTHWEST = new Vector3(-3f * GAP, 0, 1f);
            NORTHWEST = new Vector3(-3f * GAP, 0, -1f);
        }

        public void TEST() {
            //MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            NavMeshBuildSource src = new NavMeshBuildSource();
            src.transform = transform.localToWorldMatrix;
            src.shape = NavMeshBuildSourceShape.Mesh;
            src.sourceObject = GetComponent<MeshFilter>().sharedMesh;
            sources.Add(src);
            NavMeshBuildSettings set = new NavMeshBuildSettings();
            set.agentClimb = STEP;
            set.agentHeight = AGENTHEIGHT;
            set.agentRadius = AGENTRADIUS;
            set.agentSlope = AGENTSLOPE;
            set.agentTypeID = AGENTTYPEID;
            NavMeshData data = NavMeshBuilder.BuildNavMeshData(set, sources, new Bounds(), Vector3.up, Quaternion.identity);
            NavMesh.AddNavMeshData(data);
        }

        public void Bake()
        {
            this.GetComponent<NavMeshSurface>().BuildNavMesh();

           for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            this.GetComponent<MeshRenderer>().enabled = false;

        }

        public void Merge() {
            Quaternion oldRot = transform.rotation;
            Vector3 oldPos = transform.position;

            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;

            MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
            Mesh final = new Mesh();

            CombineInstance[] combiners = new CombineInstance[filters.Length];

            for (int i = 0; i < filters.Length; ++i) {
                if (filters[i].transform == transform) continue;
                combiners[i].subMeshIndex = 0;
                combiners[i].mesh = BASE; //filters[i].sharedMesh;
                combiners[i].transform = filters[i].transform.localToWorldMatrix;
            }

            final.CombineMeshes(combiners);

            GetComponent<MeshFilter>().sharedMesh = final;

            transform.rotation = oldRot;
            transform.position = oldPos;

            for (int i = 0; i < transform.childCount; ++i) {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            
        }

        public static Mesh AutoWeld(Mesh mesh, float threshold, float bucketStep)
        {
            Vector3[] oldVertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[oldVertices.Length];
            int[] old2new = new int[oldVertices.Length];
            int newSize = 0;

            // Find AABB
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < oldVertices.Length; i++)
            {
                if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
                if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
                if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
                if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
                if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
                if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
            }

            // Make cubic buckets, each with dimensions "bucketStep"
            int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
            int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
            int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
            List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

            // Make new vertices
            for (int i = 0; i < oldVertices.Length; i++)
            {
                // Determine which bucket it belongs to
                int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
                int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
                int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

                // Check to see if it's already been added
                if (buckets[x, y, z] == null)
                    buckets[x, y, z] = new List<int>(); // Make buckets lazily

                for (int j = 0; j < buckets[x, y, z].Count; j++)
                {
                    Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                    if (Vector3.SqrMagnitude(to) < threshold)
                    {
                        old2new[i] = buckets[x, y, z][j];
                        goto skip; // Skip to next old vertex if this one is already there
                    }
                }

                // Add new vertex
                newVertices[newSize] = oldVertices[i];
                buckets[x, y, z].Add(newSize);
                old2new[i] = newSize;
                newSize++;

                skip:;
            }

            // Make new triangles
            int[] oldTris = mesh.triangles;
            int[] newTris = new int[oldTris.Length];
            for (int i = 0; i < oldTris.Length; i++)
            {
                newTris[i] = old2new[oldTris[i]];
            }

            Vector3[] finalVertices = new Vector3[newSize];
            for (int i = 0; i < newSize; i++)
                finalVertices[i] = newVertices[i];

            mesh.Clear();
            mesh.vertices = finalVertices;
            mesh.triangles = newTris;
            mesh.RecalculateNormals();
            //mesh.Optimize();
            //MeshUtility.Optimize(mesh);
            return mesh;
        }

    }
}