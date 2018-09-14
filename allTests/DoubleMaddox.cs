using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class DoubleMaddox : MonoBehaviour {

        public GameObject LeftArrow;
        public GameObject RightArrow;
        private string dir;
        private float myVal;
        private Quaternion startLPos;
        private Quaternion startRPos;
        public string[] updateString;
        // Use this for initialization
        private void Awake() {
            startLPos = LeftArrow.transform.rotation;
            startRPos = RightArrow.transform.rotation;
            updateString = new string[5];
        }

        public DoubleMaddoxSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            DoubleMaddoxSettings mySettings = new DoubleMaddoxSettings();

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
        public void setSettings(DoubleMaddoxSettings mySettings) {
            /*
            */
        }
        void Start() {
            dir = "none";
            myVal = 0;
        }
        public void initTest() {
            RightArrow.transform.rotation = startRPos;
            LeftArrow.transform.rotation = startLPos;
        }
        public void exitTest() {
            //
        }
        public void doInUpdate() {
            if (dir == "right") {
                myVal = .08f;
                RightArrow.transform.Rotate(0, 0, myVal);
            }
            else if (dir == "left") {
                myVal = -.08f;
                RightArrow.transform.Rotate(0, 0, myVal);
            }
        }
        public void doTriggerPress(string myName) {
            Debug.Log("PRESSED myName:"+myName);
            if (myName.IndexOf("right") != -1) {
                dir = "right";
            }
            else if(myName.IndexOf("left") != -1) {
                dir = "left";
            }
        }
        public void doTriggerRelease(string myName) {
            Debug.Log("RELEASED  myName:" + myName);
            dir = "none";
        }
    }
}


