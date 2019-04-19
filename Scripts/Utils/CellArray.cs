using UnityEngine;
using System.Collections;

/*
 * Basic class for serialization purposes, helps the GameManager in  
 * organizing the sets of track prefabs and sets up getters for instantiation.
 */

[System.Serializable]
public class CellArray {

    public GameObject[] prefabs;

    public GameObject getPrefab(int index) {
        return prefabs[index];
    }

    public int getLength()
    {
        return prefabs.Length;
    }
}
