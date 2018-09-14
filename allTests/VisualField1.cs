using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class VisualField1 : MonoBehaviour {

        // Use this for initialization

        public MainScript mainScript;
        public GameObject LeftEyeSphere;
        public GameObject RightEyeSphere;
        public GameObject LeftInnerSphereBlack;
        public GameObject LeftInnerSphereWhite;
        public GameObject RightInnerSphereBlack;
        public GameObject RightInnerSphereWhite;

        public GameObject LeftOccludingSphere;
        public GameObject RightOccludingSphere;
        public string[] updateString;

        void Awake() {
            updateString = new string[5];
        }
        public VisualField1Settings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            VisualField1Settings mySettings = new VisualField1Settings();

            return mySettings;
        }
        public void synchUpdateString() {
            //register EVERY button in this list so that it is recordable in the protocol recorder.
            /*updateString[0] = distanceSlider.sliderValue.ToString();
            updateString[1] = rotationSlider.sliderValue.ToString();
            updateString[2] = AttachToHead.isOn.ToString();
            updateString[3] = GradsToggle.isOn.ToString();
            updateString[4] = ShowRedSide.isOn.ToString();
            baseTest.updateSettings(updateString);
            */
        }
        public void setSettings(VisualField1Settings mySettings) {

        }

        public void initTest() {
            //
        }
        public void exitTest() {
            //
        }
        public void doInUpdate() {
           LeftEyeSphere.transform.rotation = mainScript.LeftEyeball.transform.rotation;
            RightEyeSphere.transform.rotation = mainScript.RightEyeball.transform.rotation;
            LeftEyeSphere.transform.position = mainScript.CameraHead.transform.position;
            RightEyeSphere.transform.position = mainScript.CameraHead.transform.position;
            
        }
        public void doTriggerPress(string myName) {

        }
        public void doTriggerRelease(string myName) {

        }
        void Start() {

        }
    }
}