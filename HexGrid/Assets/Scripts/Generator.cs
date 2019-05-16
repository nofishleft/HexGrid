using nz.Rishaan.HexGrid;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Generator : MonoBehaviour {

    public GameObject TilePrefab;
    public Material mat;

     int X;
     int Z;
     int bot;
     int top;
     float seedX;
     float seedZ;
    float[,] map;

    public void Generate() {
        Vector3 e = Quaternion.Euler(0, 60, 0) * new Vector3(1, 0, 0);
        Vector3 f = Quaternion.Euler(0, 120, 0) * new Vector3(0, 0, HexGrid.SIDE / 2);
        Vector3 g = new Vector3(1, 0, -3f * HexGrid.GAP);
        Vector3 k = e;
        k.z = -k.z;
        Vector3 l = f;
        l.x = -l.x;
        Vector3 m = new Vector3(1, 0, 0);
        for (int z = 0; z < Z; ++z)
            
        {
            for (int x = 0; x < X; ++x)
            {
                if ((x + z) % 2 != 0) continue;
                float h = (top - bot) * Mathf.PerlinNoise(seedX + x * 0.05f, seedZ + z * 0.05f) + bot;
                float xPos = x * HexGrid.HEIGHT / 2;
                float zPos = z * 3f * HexGrid.GAP;
                GameObject o = Instantiate(TilePrefab, new Vector3(x * HexGrid.HEIGHT / 2, h, z * 3f * HexGrid.GAP), Quaternion.identity, transform);
                map[x,z] = h;

                if (x - 2 >= 0) {
                    Vector3 a = new Vector3(xPos-1 , h , zPos - HexGrid.SIDE / 2);
                    Vector3 b = new Vector3(xPos-1, h, zPos + HexGrid.SIDE / 2);
                    Vector3 c = new Vector3(xPos-1, map[x - 2, z], zPos - HexGrid.SIDE / 2);
                    Vector3 d = new Vector3(xPos-1, map[x - 2, z], zPos + HexGrid.SIDE / 2);
                    HexShift.BuildWall(c, d, a, b, mat).parent = transform;
                }

                if (x - 1 >= 0 && z-1 >= 0) {
                    Vector3 a = new Vector3(xPos - 1, h, zPos) - e + f + g;
                    Vector3 b = new Vector3(xPos - 1, h, zPos) - e - f + g;
                    Vector3 c = new Vector3(xPos - 1, map[x-1,z-1], zPos) - e + f + g;
                    Vector3 d = new Vector3(xPos - 1, map[x-1,z-1], zPos) - e - f + g;
                    HexShift.BuildWall(c, d, a, b, mat).parent = transform;
                }

                if (x + 1 < X && z - 1 >= 0)
                {
                    Vector3 a = new Vector3(xPos, h, zPos) - k + l + m;
                    Vector3 b = new Vector3(xPos, h, zPos) - k - l + m;
                    Vector3 c = new Vector3(xPos, map[x+1,z-1], zPos) - k + l + m;
                    Vector3 d = new Vector3(xPos, map[x+1,z-1], zPos) - k - l + m;
                    HexShift.BuildWall(a, b, c, d, mat).parent = transform;
                }

            }
        }
    }

    

	// Use this for initialization
	void Start () {
        Z = Random.Range(50, 100);
        X = (int)(1.5f*Z);
        map = new float[X, Z];
        bot = 0;
        top = 20;
        seedX = Random.Range(0f, 1000f);
        seedZ = Random.Range(0f, 1000f);
        Generate();
        //this.GetComponent<HexGrid>().Merge();
        
        /*this.GetComponent<NavMeshSurface>().BuildNavMesh();

        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        this.GetComponent<MeshRenderer>().enabled = false;*/

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
