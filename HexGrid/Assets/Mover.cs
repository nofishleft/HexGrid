using nz.Rishaan.HexGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Mover : MonoBehaviour
{

    public Camera cam;

    NavMeshPath path;

    public LayerMask mask;

    Vector3 dir;

    bool Moving = false;

    Vector3 MoveTo;

    public Material SelectedMat;
    public MeshRenderer last = null;

    public void Move(Vector3 to) {
        MoveTo = to;
        path = new NavMeshPath();
        Moving = NavMesh.CalculatePath(transform.position - new Vector3(0, transform.localScale.y / 2, 0), MoveTo, 1, path);
    }

    float aTime = 0;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A)) {
            cam.transform.parent.localRotation = Quaternion.Euler(0, 360f * Time.deltaTime, 0) * cam.transform.parent.localRotation;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cam.transform.parent.localRotation = Quaternion.Euler(0, -360f * Time.deltaTime, 0) * cam.transform.parent.localRotation;
        }

        if (Input.GetMouseButtonDown(1)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.collider.gameObject.name != "Wall") {
                if (last != null) last.material = hit.transform.GetComponent<MeshRenderer>().material;
                last = hit.transform.GetComponent<MeshRenderer>();
                last.material = SelectedMat;
                Move(hit.transform.position);
            }
        }

        if (Moving)
        {
            aTime += Time.deltaTime;
            if (aTime > 0.5f)
            {
                aTime = 0;
                Vector3 dira = path.corners[1] - transform.position;
                float sqr = 100000f;
                foreach (Vector3 v in HexGrid.DIR)
                {
                    float s = (dira - v).sqrMagnitude;
                    if (s < sqr)
                    {
                        sqr = s;
                        dir = v;
                    }
                }

                transform.position += dir;

                Collider[] colliders;
                if ((colliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z), 0.5f /* Radius */)).Length > 1) //Presuming the object you are testing also has a collider 0 otherwise
                {
                    foreach (var collider in colliders)
                    {
                        GameObject go = collider.gameObject; //This is the game object you collided with
                        if (go == gameObject) continue; //Skip the object itself
                        Vector3 pos = transform.position;
                        pos.y = go.transform.position.y + transform.localScale.y / 2;
                        transform.position = pos;

                    }
                }
                if ((transform.position - MoveTo).sqrMagnitude <= 1f) {
                    Moving = false;
                } else Move(MoveTo);
            }
        }
    }
}
