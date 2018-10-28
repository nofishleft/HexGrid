using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace nz.Rishaan.HexGrid {
    public class HexGrid : MonoBehaviour {
        //Properties
        public Mesh BASE;

        public Material DefaultWallMaterial = null;

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
            foreach (MeshRenderer mr in GameObject.FindObjectsOfType<MeshRenderer>()) {
                if (mr.gameObject.name == "Wall") {
                    mr.material = DefaultWallMaterial;
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
            //Merge();
            //Bake();
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

    }
}