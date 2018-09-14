using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class DepthPractice : MonoBehaviour {

        public GameObject leftLoop;
        public GameObject rightLoop;
        public MainScript myMainScript;
        Quaternion LorigLoopRot;
        Vector3 LorigLoopPos;
        Quaternion RorigLoopRot;
        Vector3 RorigLoopPos;
        public string[] updateString;

        public GameObject[] surgeonCubes;
        public GameObject[] controllers;

        // Use this for initialization
        void Awake() {
            LorigLoopPos = leftLoop.transform.position;
            LorigLoopRot = leftLoop.transform.rotation;
            RorigLoopPos = rightLoop.transform.position;
            RorigLoopRot = rightLoop.transform.rotation;
            updateString = new string[5];
        }
        public DepthPracticeSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            DepthPracticeSettings mySettings = new DepthPracticeSettings();

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
        public void setSettings(DepthPracticeSettings mySettings) {
            /*distanceSlider.setSliderValue(float.Parse(settingsString[0]));
            rotationSlider.setSliderValue(float.Parse(settingsString[1]));
            AttachToHead.isOn = bool.Parse(settingsString[2]);
            GradsToggle.isOn = bool.Parse(settingsString[3]);
            ShowRedSide.isOn = bool.Parse(settingsString[4]);
            setUpProps();
            */
        }
        public void initTest() {
            //
        }
        public void exitTest() {
            //
        }
        public void doInUpdate() {
            leftLoop.transform.position = myMainScript.LeftEyeCamera.transform.position;
            leftLoop.transform.rotation = myMainScript.LeftEyeCamera.transform.rotation;
        }
        public void doTriggerPress(string myName) {
            Debug.Log("PRESSED myName:" + myName);
        }
        public void doTriggerRelease(string myName) {
            Debug.Log("RELEASED  myName:" + myName);
        }

        public void ToggleSurgeonCubes() {
            for (int i = 0; i < surgeonCubes.Length; i++) {
                surgeonCubes[i].SetActive(!surgeonCubes[i].activeInHierarchy);
            }
            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].SetActive(!controllers[i].activeInHierarchy);
            }
        }
    }
}