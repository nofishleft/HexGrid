using nz.Rishaan.HexGrid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexShift : MonoBehaviour {

    public void shift (int dir) {
        this.transform.position += HexGrid.DIR[dir];
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
