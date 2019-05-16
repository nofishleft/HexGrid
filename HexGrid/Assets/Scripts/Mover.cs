using nz.Rishaan.HexGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Mover : MonoBehaviour
{

    public Camera cam;
    public Transform camPos;

    NavMeshPath path;

    public LayerMask mask;

    public LinesGR LineCreator;

    Vector3 dir;

    bool Moving = false;

    Vector3 MoveTo;

    public Material SelectedMat;
    Material preserveMaterial1;
    public Material HoverMat;
    Material preserveMaterial2;
    public MeshRenderer last = null;
    public MeshRenderer last2 = null;

    public void Move(Vector3 to)
    {
        MoveTo = to;
        path = new NavMeshPath();
        Moving = NavMesh.CalculatePath(transform.position - new Vector3(0, transform.localScale.y / 2, 0), MoveTo, 1, path);
    }

    float aTime = 0;

    // Use this for initialization
    void Start()
    {
        destRot = camPos.parent.localRotation;
    }

    Quaternion destRot;
    public Transform PlayerPos;
    Vector3 PlayerDest;


    public void CreatePath(Vector3 to)
    {
        List<Vector3> list = new List<Vector3>();
        Vector3 calcPos = transform.position - new Vector3(0, transform.localScale.y / 2, 0);
        list.Add(calcPos);
        NavMeshPath patha = new NavMeshPath();
        bool m = NavMesh.CalculatePath(calcPos, MoveTo, 1, patha);
        while (m)
        {
            if (patha.corners.Length <= 1) break;
            Vector3 dirv = new Vector3();
            Vector3 dira = patha.corners[1] - calcPos;
            float sqr = 100000f;
            foreach (Vector3 v in HexGrid.DIR)
            {
                float s = (dira - v).sqrMagnitude;
                if (s < sqr)
                {
                    sqr = s;
                    dirv = v;
                }
            }

            calcPos += dirv;

            Collider[] colliders;
            if ((colliders = Physics.OverlapSphere(new Vector3(calcPos.x, calcPos.y, calcPos.z), 0.5f /* Radius */)).Length >= 1) //Presuming the object you are testing also has a collider 0 otherwise
            {
                foreach (var collider in colliders)
                {
                    GameObject go = collider.gameObject; //This is the game object you collided with
                    if (go == gameObject) continue; //Skip the object itself
                    Vector3 pos = calcPos;
                    pos.y = go.transform.position.y;
                    calcPos = pos;

                }
            }

            list.Add(calcPos);
            patha = new NavMeshPath();
            m = NavMesh.CalculatePath(calcPos, MoveTo, 1, patha);
            if ((calcPos - MoveTo).sqrMagnitude <= 1f)
            {
                break;
            }
        }

        LineCreator.Create(list.ToArray(), 0.2f);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            destRot = Quaternion.Euler(0, 360f * Time.deltaTime, 0) * camPos.parent.localRotation;
            camPos.parent.localRotation = Quaternion.Lerp(destRot, camPos.parent.localRotation, 0.5f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            destRot = Quaternion.Euler(0, -360f * Time.deltaTime, 0) * camPos.parent.localRotation;
            camPos.parent.localRotation = Quaternion.Lerp(destRot, camPos.parent.localRotation, 0.5f);
        }


        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if (last != null) last.material = preserveMaterial1;
                last = hit.transform.GetComponent<MeshRenderer>();
                if (last == last2)
                {
                    last2 = null;
                    last.material = preserveMaterial2;
                }
                preserveMaterial1 = last.sharedMaterial;
                last.sharedMaterial = SelectedMat;
                Move(hit.transform.position);
                CreatePath(hit.transform.position);
            }
        }
        else
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.GetComponent<MeshRenderer>().sharedMaterial != SelectedMat)
                {
                    if (last2 != null)
                    {
                        last2.material = preserveMaterial2;
                    }
                    last2 = hit.transform.GetComponent<MeshRenderer>();
                    preserveMaterial2 = last2.sharedMaterial;
                    last2.sharedMaterial = HoverMat;
                }
            }
        }
        if (PlayerDest != null) PlayerPos.position = Vector3.Lerp(PlayerPos.position, PlayerDest, 0.3f);
        if (Moving)
        {
            //transform.position = Vector3.Lerp(transform.position, PlayerDest, 0.5f);
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

                LineCreator.PopPoint();

                Collider[] colliders;
                if ((colliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z), 0.5f /* Radius */)).Length >= 1) //Presuming the object you are testing also has a collider 0 otherwise
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
                PlayerDest = transform.position;
                if ((transform.position - MoveTo).sqrMagnitude <= 1f)
                {
                    Moving = false;
                }
                else Move(MoveTo);
            }
        }
    }
}
