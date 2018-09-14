using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace MainProject {
    public class Bagolini : MonoBehaviour {

        // Use this for initialization
        public MainScript mainScript;
        public baseTestClass baseTest;
        public GameObject leftWhiteCross;
        public GameObject rightWhiteCross;
        public GameObject leftRedCross;
        public GameObject rightRedCross;
        public GameObject graduations;
        public GameObject centerLight;
        //this is what gets rotated and moved back/forth relative to head pos
        public GameObject TestCrosses;
        //this is what gets attached to the head and it contains the testCrosses.
        public GameObject TestContainer;
        //these are what we return the test container to if its unattached from the head
        private Vector3 TestContainerOrigPos;
        private Quaternion TestContainerOrigLocalRot;
        private Quaternion TestContainerOrigRot;
        private Vector3 TestCrossesOrigLocalPos;
        private Quaternion TestCrossesOrigLocalRot;
        private int count;
        public string[] updateString;

        public Text distText;
        public Text rotText;
        public genericSlider rotationSlider;
        public genericSlider distanceSlider;
        public Button ninetyDegreesButton;
        public Button fourtyfiveDegreesButton;
        public Toggle GradsToggle;
        public Toggle AttachToHead;
        public Toggle ShowRedSide;
        public float maxDist;
        public float minDist;
        public float medDist;
        public float maxRot;
        public float minRot;
        public float medRot;

        public float defaultRotationSliderValue;
        public float defaultDistanceSliderValue;
        public bool defaultAttachIsOn;
        public bool defaultGradsToggleIsOn;
        public bool defaultShowRedSideIsOn;


        void Awake() {
            TestContainerOrigPos = TestContainer.transform.position;
            TestContainerOrigRot = TestContainer.transform.rotation;
            TestCrossesOrigLocalPos = TestCrosses.transform.localPosition;
            TestCrossesOrigLocalRot = TestCrosses.transform.localRotation;
            updateString = new string[5];
            defaultRotationSliderValue = 0;
            defaultDistanceSliderValue = 0;
            defaultGradsToggleIsOn = GradsToggle.isOn;
            defaultShowRedSideIsOn = ShowRedSide.isOn;
            defaultAttachIsOn = AttachToHead.isOn;
        }
        public void synchUpdateString() {
            //register EVERY button in this list so that it is recordable in the protocol recorder.
            updateString[0] = distanceSlider.sliderValue.ToString();
            updateString[1] = rotationSlider.sliderValue.ToString();
            updateString[2] = AttachToHead.isOn.ToString();
            updateString[3] = GradsToggle.isOn.ToString();
            updateString[4] = ShowRedSide.isOn.ToString();
            baseTest.updateSettings(updateString);
        }
        public BagoliniSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            BagoliniSettings mySettings = new BagoliniSettings();
            if (settingsString.Length > 0) {
                mySettings.setting0 = float.Parse(settingsString[0]);
                mySettings.setting1 = float.Parse(settingsString[1]);
                mySettings.setting2 = bool.Parse(settingsString[2]);
                mySettings.setting3 = bool.Parse(settingsString[3]);
                mySettings.setting4 = bool.Parse(settingsString[4]);
            }
            else {
                mySettings.setting0 = 0;
                mySettings.setting1 = 0;
                mySettings.setting2 = false;
                mySettings.setting3 = false;
                mySettings.setting4 = false;
            }
            return mySettings;
        }
        public void setSettings(BagoliniSettings mySettings) {
            distanceSlider.setSliderValue(mySettings.setting0);
            rotationSlider.setSliderValue(mySettings.setting1);
            AttachToHead.isOn = mySettings.setting2;
            GradsToggle.isOn = mySettings.setting3;
            ShowRedSide.isOn = mySettings.setting4;
            setUpProps();
        }
        public void initSettings() {
            resetCrossesPos();
            rotationSlider.setSliderValue(defaultRotationSliderValue);
            distanceSlider.setSliderValue(defaultDistanceSliderValue);
            AttachToHead.isOn = defaultAttachIsOn;
            GradsToggle.isOn = defaultGradsToggleIsOn;
            ShowRedSide.isOn = defaultShowRedSideIsOn;
            
        }
        public void initTest() {
            rotationSlider.dD = doRotationSliderAction;
            distanceSlider.dD = doDistanceSliderAction;
            initSettings();
            setUpProps();
            count = 0;
        }
        void doRotationSliderAction() {
            synchUpdateString();
            setRotation();
        }
        void doDistanceSliderAction() {
            synchUpdateString();
            setDistance();
        }
        public void setDistance() {
            //Move TestCrosses within a range
            float perc = distanceSlider.sliderValue / 100;
            float newDist = ((maxDist - minDist) * perc) + minDist;
            Vector3 newPos = TestCrosses.transform.localPosition;
            newPos.z = newDist;
            TestCrosses.transform.localPosition = newPos;
            distText.text = newDist.ToString() + "m";
        }
        public void setDegrees(int percentVal) {
            rotationSlider.setSliderValue(percentVal);
            setRotation();
        }
        public void setDistance(int percentVal) {
            distanceSlider.setSliderValue(percentVal);
            setDistance();
        }
        public void setRotation() {
            float perc = rotationSlider.sliderValue / 100;
            float newRot = ((maxRot - minRot) * perc) + minRot;
            Vector3 myRot = TestCrosses.transform.localRotation.eulerAngles;
            myRot.z = newRot;
            TestCrosses.transform.localRotation = Quaternion.Euler(myRot);
            rotText.text = newRot.ToString();
        }
        public void doAttachHeadToggle() {
            resetCrossesPos();
            setUpProps();
            synchUpdateString();
        }
        public void resetCrossesPos() {

            TestContainer.transform.position = TestContainerOrigPos;
            TestContainer.transform.rotation = TestContainerOrigRot;
            TestCrosses.transform.localRotation = TestCrossesOrigLocalRot;
            TestCrosses.transform.localPosition = TestCrossesOrigLocalPos;

        }
        public void doGradsToggle() {
            synchUpdateString();
            setUpProps();
        }
        public void doShowRedToggle() {
            synchUpdateString();
            setUpProps();
        }
        public void setUpProps() {
            leftRedCross.SetActive(false);
            rightRedCross.SetActive(false);
            leftWhiteCross.SetActive(false);
            rightWhiteCross.SetActive(false);
            if (ShowRedSide.isOn) {
                leftRedCross.SetActive(true);
            }
            else {
                leftWhiteCross.SetActive(true);
            }
            rightWhiteCross.SetActive(true);
            graduations.SetActive(GradsToggle.isOn);
            setDistance();
            setRotation();
        }


        public void exitTest() {
            //
        }
        public void doInUpdate() {
            if (mainScript.myDataRecorder.logging) {
                count++;
                baseTest.dataString = AttachToHead.isOn.ToString() + "-" + GradsToggle.isOn.ToString() + "-" + ShowRedSide.isOn.ToString() + "-" + rotationSlider.sliderValue.ToString() + "-" + distanceSlider.sliderValue.ToString() + "-" + count.ToString();
            }
            if (mainScript.myPlaybackDevice.isPlaying) {
                //Debug.Log(baseTest.dataString);
                string[] sArr = Regex.Split(baseTest.dataString, "-");
                if (sArr.Length > 3) {
                    if (baseTest.StringToBool(sArr[0]) != AttachToHead.isOn) {
                        AttachToHead.isOn = baseTest.StringToBool(sArr[0]);
                    }
                    if (baseTest.StringToBool(sArr[1]) != GradsToggle.isOn) {
                        GradsToggle.isOn = baseTest.StringToBool(sArr[1]);
                    }
                    if (baseTest.StringToBool(sArr[2]) != ShowRedSide.isOn) {
                        ShowRedSide.isOn = baseTest.StringToBool(sArr[2]);
                    }
                    if (baseTest.StringToFloat(sArr[3]) != rotationSlider.sliderValue) {
                        rotationSlider.setSliderValue(baseTest.StringToFloat(sArr[3]));
                    }
                    if (baseTest.StringToFloat(sArr[4]) != distanceSlider.sliderValue) {
                        distanceSlider.setSliderValue(baseTest.StringToFloat(sArr[4]));
                    }
                    setUpProps();
                }

            }
            if (AttachToHead.isOn) {
                if (mainScript.myPlaybackDevice.isPlaying) {
                    TestContainer.transform.position = mainScript.playbackHead.transform.position;
                    TestContainer.transform.rotation = mainScript.playbackHead.transform.rotation;
                }
                else {
                    TestContainer.transform.position = mainScript.LeftEyeCamera.transform.position;
                    TestContainer.transform.rotation = mainScript.LeftEyeCamera.transform.rotation;
                }

            }
        }
        public void doTriggerPress(string myName) {
            //Debug.Log("PRESSED myName:" + myName);
        }
        public void doTriggerRelease(string myName) {
            //Debug.Log("RELEASED  myName:" + myName);
        }
    }
}
