using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    public static DebugManager instance;
    public List<object> trackingList;

    private void Awake() {
        instance = this;
        trackingList = new List<object>();
    }

    private void Update() {
        if(trackingList.Count > 0)
        Debug.Log("List size = " + trackingList.Count);
    }
}
