using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class DepthTest : MonoBehaviour {

        

        // Use this for initialization
        public string[] updateString;
        void Awake() {
            updateString = new string[5];
        }
        public DepthTestSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            DepthTestSettings mySettings = new DepthTestSettings();

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
        public void setSettings(DepthTestSettings mySettings) {

        }
        void Start() {

        }
        public void initTest() {
            //
        }
        public void exitTest() {
            //
        }
        public void doInUpdate() {

        }
        public void doTriggerPress(string myName) {
            Debug.Log("PRESSED myName:" + myName);
        }
        public void doTriggerRelease(string myName) {
            Debug.Log("RELEASED  myName:" + myName);
        }

        
    }
}
