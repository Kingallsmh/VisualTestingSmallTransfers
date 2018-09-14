using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class StandardEyeChart : MonoBehaviour {

        // Use this for initialization
        public GameObject ChartContainer;
        public MainScript mainScript;
        public string[] updateString;

        void Awake() {
            updateString = new string[5];
        }
        public StandardEyeChartSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            StandardEyeChartSettings mySettings = new StandardEyeChartSettings();

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
        public void setSettings(StandardEyeChartSettings mySettings) {
            /*distanceSlider.setSliderValue(float.Parse(settingsString[0]));
            rotationSlider.setSliderValue(float.Parse(settingsString[1]));
            AttachToHead.isOn = bool.Parse(settingsString[2]);
            GradsToggle.isOn = bool.Parse(settingsString[3]);
            ShowRedSide.isOn = bool.Parse(settingsString[4]);
            setUpProps();
            */
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
            ChartContainer.transform.position = mainScript.LeftEyeCamera.transform.position;
            ChartContainer.transform.rotation = mainScript.LeftEyeCamera.transform.rotation;
        }
        public void doTriggerPress(string myName) {
            Debug.Log("PRESSED myName:" + myName);
        }
        public void doTriggerRelease(string myName) {
            Debug.Log("RELEASED  myName:" + myName);
        }
    }
}
