using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSight : MonoBehaviour
{
    public float DistanceToPlayer = 5.0f;
    public Material TransparentMaterial = null;
    public Material CullingMaterial = null;
    public float FadeInTimeout = 0.6f;
    public float FadeOutTimeout = 0.2f;
    public float TargetTransparency = 0.3f;
    public Transform DestPos;

    public Transform player;

    private void Start()
    {
        DistanceToPlayer = (transform.position - player.position).magnitude;
    }

    private void Update()
    {
        TransparentMaterial.SetVector("_P1", transform.position);
        TransparentMaterial.SetVector("_P2", player.position);
        CullingMaterial.SetVector("_P1", transform.position);
        CullingMaterial.SetVector("_P2", player.position);
        transform.position = Vector3.Lerp(transform.position, DestPos.position, 10f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, DestPos.rotation, 10f * Time.deltaTime);
    }
    /*
        RaycastHit[] hits; // you can also use CapsuleCastAll() 
                           // TODO: setup your layermask it improve performance and filter your hits. 
        hits = Physics.CapsuleCastAll(player.position, transform.position, 0.4f, transform.position - player.position);
        //hits = Physics.RaycastAll(transform.position, transform.forward, DistanceToPlayer);
        foreach (RaycastHit hit in hits)
        {
            MeshRenderer R = hit.collider.GetComponent<MeshRenderer>();
            if (R == null || hit.collider.gameObject.name == "Player")
            {
                Debug.Log("err");
                continue;
            }
            // no renderer attached? go to next hit 
            // TODO: maybe implement here a check for GOs that should not be affected like the player
            AutoTransparent AT = R.GetComponent<AutoTransparent>();
            if (AT == null) // if no script is attached, attach one
            {
                AT = R.gameObject.AddComponent<AutoTransparent>();
                AT.TransparentMaterial = TransparentMaterial;
                AT.FadeInTimeout = FadeInTimeout;
                AT.FadeOutTimeout = FadeOutTimeout;
                AT.TargetTransparency = TargetTransparency;
            }
            AT.BeTransparent(); // get called every frame to reset the falloff
        }
    }*/
}
