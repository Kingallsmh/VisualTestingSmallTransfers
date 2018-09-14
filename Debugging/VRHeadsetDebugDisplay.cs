using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRHeadsetDebugDisplay : MonoBehaviour {

    public static VRHeadsetDebugDisplay Instance;
    public Text text1, text2;

    private void Awake() {
        Instance = this;
    }

    public void SetText(string newText) {
        text1.text = newText;
        text2.text = newText;
    }

    public void SetText(string newText, int num) {
        if(num == 1) {
            text1.text = newText;
        }
        else if(num == 2) {
            text2.text = newText;
        }
    }
}
