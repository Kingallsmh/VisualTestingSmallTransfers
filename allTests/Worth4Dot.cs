using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace MainProject {
    public class Worth4Dot : MonoBehaviour {

        // Use this for initialization
        public MainScript mainScript;
        public GameObject CyanImages;
        public GameObject WFDLights;
        public GameObject WFDSmallLights;
        public GameObject WFDLargeLights;
        public GameObject WFDSmallLightsContainer;
        public GameObject WFDLargeLightsContainer;
        public float minDist;
        public float midDist;
        public float maxDist;
        public Toggle attachToggle;
        public Toggle greenToggle;
        public Toggle redSwapToggle;
        public Toggle glassesOn;
        public Toggle useLargeToggle;
        public baseTestClass baseTest;
        public genericSlider mySlider;
        public Text sliderText;
        private int count;

        private float defaultSliderValue;
        private bool defaultAttachToggleValue;
        private bool defaultGreenToggleValue;
        private bool defaultRedSwapValue;
        private bool defaultGlassesOnValue;
        private bool defaultUseLargeValue;

        private Vector3 WFDSmallLightsOrigPos;
        private Quaternion WFDSmallLightsOrigRot;

        private Vector3 WFDLargeLightsOrigPos;
        private Quaternion WFDLargeLightsOrigRot;

        public string[] updateString;
        public Transform originalParent;

        void Awake() {
            updateString = new string[6];
            defaultSliderValue = 0;
            defaultAttachToggleValue = attachToggle.isOn;
            defaultGreenToggleValue = greenToggle.isOn;
            defaultRedSwapValue = redSwapToggle.isOn;
            defaultGlassesOnValue = glassesOn.isOn;
            defaultUseLargeValue = useLargeToggle.isOn;
            WFDSmallLightsOrigPos = WFDSmallLights.transform.localPosition;
            WFDSmallLightsOrigRot = WFDSmallLights.transform.localRotation;
            WFDLargeLightsOrigPos = WFDLargeLights.transform.localPosition;
            WFDLargeLightsOrigRot = WFDLargeLights.transform.localRotation;
        }
        public Worth4DotSettings createSettingsObject(string[] settingsString) {
            //this converts a string to a much faster to access object.
            Worth4DotSettings mySettings = new Worth4DotSettings();
            if (settingsString.Length > 0) {
                mySettings.setting0 = float.Parse(settingsString[0]);
                mySettings.setting1 = bool.Parse(settingsString[1]);
                mySettings.setting2 = bool.Parse(settingsString[2]);
                mySettings.setting3 = bool.Parse(settingsString[3]);
                mySettings.setting4 = bool.Parse(settingsString[4]);
                mySettings.setting5 = bool.Parse(settingsString[5]);
            }
            else {
                mySettings.setting0 = 0;
                mySettings.setting1 = false;
                mySettings.setting2 = false;
                mySettings.setting3 = false;
                mySettings.setting4 = false;
                mySettings.setting5 = false;
            }
            return mySettings;
        }
        public void genericToggleFunction() {
            synchUpdateString();
            setUpProps();
        }
        public void synchUpdateString() {
            //register EVERY button in this list so that it is recordable in the protocol recorder.
            updateString[0] = mySlider.sliderValue.ToString();
            updateString[1] = attachToggle.isOn.ToString();
            updateString[2] = greenToggle.isOn.ToString();
            updateString[3] = redSwapToggle.isOn.ToString();
            updateString[4] = glassesOn.isOn.ToString();
            updateString[5] = useLargeToggle.isOn.ToString();
            baseTest.updateSettings(updateString);
        }
        public void setSettings(Worth4DotSettings mySettings) {
            mySlider.setSliderValue(mySettings.setting0);
            attachToggle.isOn = mySettings.setting1;
            greenToggle.isOn = mySettings.setting2;
            redSwapToggle.isOn = mySettings.setting3;
            glassesOn.isOn = mySettings.setting4;
            useLargeToggle.isOn = mySettings.setting5;
            setUpProps();
        }
        public void initSettings() {
            mySlider.setSliderValue(defaultSliderValue);
            attachToggle.isOn = defaultAttachToggleValue;
            greenToggle.isOn = defaultGreenToggleValue;
            redSwapToggle.isOn = defaultRedSwapValue;
            glassesOn.isOn = defaultGlassesOnValue;
            useLargeToggle.isOn = defaultUseLargeValue;
            //WFDSmallLights.transform.position= WFDSmallLightsOrigPos;
            //WFDSmallLights.transform.rotation = WFDSmallLightsOrigRot;
            //WFDLargeLights.transform.position = WFDLargeLightsOrigPos;
            //WFDLargeLights.transform.rotation = WFDLargeLightsOrigRot;
        }

        public void initTest() {
            initSettings();            
            mySlider.dD = updateDistance;
            mainScript.don3DGlasses(glassesOn.isOn, redSwapToggle.isOn, greenToggle.isOn);
            count = 0;
            setUpProps();
        }
        public void setUpProps() {
            if (greenToggle.isOn) {
                CyanImages.SetActive(false);
                WFDLights.SetActive(true);
            }
            else {
                CyanImages.SetActive(true);
                WFDLights.SetActive(false);
            }
            showCurrentBox();
            mainScript.don3DGlasses(glassesOn.isOn, redSwapToggle.isOn, greenToggle.isOn);
            updateDistance();
        }
        public void showCurrentBox() {
            if (useLargeToggle.isOn) {
                WFDLargeLights.SetActive(true);
                WFDSmallLights.SetActive(false);
            }
            else {
                WFDLargeLights.SetActive(false);
                WFDSmallLights.SetActive(true);
            }
        }
        public void exitTest() {
            SetToParent(WFDSmallLights.transform, WFDLargeLights.transform, originalParent);
            mainScript.don3DGlasses(false, redSwapToggle.isOn, greenToggle.isOn);
        }

        public void setMidDistance() {
            float myVal = ((midDist - minDist) / (maxDist - minDist)) * 100;
            mySlider.setSliderValue(myVal);
            updateDistance();
        }
        public void setMaxDistance() {
            float myVal = 100;
            mySlider.setSliderValue(myVal);
            updateDistance();
        }

        void SetToParent(Transform smallVersion, Transform bigVersion, Transform parentToBe) {
            if(smallVersion != null) {
                smallVersion.SetParent(parentToBe);
                smallVersion.localPosition = WFDSmallLightsOrigPos;
                smallVersion.localRotation = WFDSmallLightsOrigRot;
            }
            if(bigVersion != null) {
                bigVersion.SetParent(parentToBe);
                bigVersion.localPosition = WFDLargeLightsOrigPos;
                bigVersion.localRotation = WFDLargeLightsOrigRot;
            }            
        }

        public void updateDistance() {
            float perc = mySlider.sliderValue / 100;
            float newDist = ((maxDist - minDist) * perc) + minDist;
            //Vector3 newSmallPos = WFDSmallLightsContainer.transform.position;
            //newSmallPos.z = newDist;
            //Vector3 newLargePos = WFDLargeLightsContainer.transform.position;
            //newLargePos.z = newDist;
            //WFDSmallLightsContainer.transform.position = newSmallPos;
            //WFDLargeLightsContainer.transform.position = newLargePos;
            if (attachToggle.isOn) {
                if (mainScript.myPlaybackDevice.isPlaying) {
                    SetToParent(WFDSmallLights.transform, WFDLargeLights.transform, mainScript.playbackHead.transform);
                }
                else {
                    SetToParent(WFDSmallLights.transform, WFDLargeLights.transform, mainScript.LeftEyeCamera.transform);
                }
                WFDSmallLights.transform.localPosition = new Vector3(0, 0, 0);
                WFDSmallLights.transform.localRotation = Quaternion.identity;
                WFDLargeLights.transform.localPosition = new Vector3(0, 0, 0);
                WFDLargeLights.transform.localRotation = Quaternion.identity;
            }
            else {
                //TODO Would have to adjust this section to make use of more attachment options
                SetToParent(WFDSmallLights.transform, WFDLargeLights.transform, originalParent);
            }
            
            WFDSmallLightsContainer.transform.localPosition = new Vector3(0, 0, newDist);
            WFDLargeLightsContainer.transform.localPosition = new Vector3(0, 0, newDist);
            sliderText.text = newDist.ToString() + "m";
            synchUpdateString();
        }
        public void doInUpdate() {
            if (mainScript.myDataRecorder.logging) {
                count++;
                baseTest.dataString = mySlider.sliderValue.ToString() + "-" + attachToggle.isOn.ToString() + "-" + greenToggle.isOn.ToString() + "-" + redSwapToggle.isOn.ToString() + "-" + glassesOn.isOn.ToString() + "-" + useLargeToggle.isOn.ToString() + "-" + count.ToString();
            }
            if (mainScript.myPlaybackDevice.isPlaying) {
                //Debug.Log(baseTest.dataString);
                string[] sArr = Regex.Split(baseTest.dataString, "-");
                if (sArr.Length > 3) {
                    bool somethingChanged = false;
                    if (baseTest.StringToFloat(sArr[0]) != mySlider.sliderValue) {
                        mySlider.setSliderValue(baseTest.StringToFloat(sArr[0]));
                        somethingChanged = true;
                    }
                    if (baseTest.StringToBool(sArr[1]) != attachToggle.isOn) {
                        attachToggle.isOn = baseTest.StringToBool(sArr[1]);
                        somethingChanged = true;
                    }
                    if (baseTest.StringToBool(sArr[2]) != greenToggle.isOn) {
                        greenToggle.isOn = baseTest.StringToBool(sArr[2]);
                        somethingChanged = true;
                    }
                    if (baseTest.StringToBool(sArr[3]) != redSwapToggle.isOn) {
                        redSwapToggle.isOn = baseTest.StringToBool(sArr[3]);
                        somethingChanged = true;
                    }
                    if (baseTest.StringToBool(sArr[4]) != glassesOn.isOn) {
                        glassesOn.isOn = baseTest.StringToBool(sArr[4]);
                        somethingChanged = true;
                    }
                    if (baseTest.StringToBool(sArr[5]) != useLargeToggle.isOn) {
                        useLargeToggle.isOn = baseTest.StringToBool(sArr[5]);
                        somethingChanged = true;
                    }
                    if (somethingChanged) {
                        setUpProps();
                    }
                }

            }

            //TODO Does not reset to proper valued place
            //if (attachToggle.isOn) {
            //    if (mainScript.myPlaybackDevice.isPlaying) {
            //        WFDLargeLights.transform.position = mainScript.playbackHead.transform.position;
            //        WFDLargeLights.transform.rotation = mainScript.playbackHead.transform.rotation;
            //        WFDSmallLights.transform.position = mainScript.playbackHead.transform.position;
            //        WFDSmallLights.transform.rotation = mainScript.playbackHead.transform.rotation;
            //    }
            //    else {
            //        WFDLargeLights.transform.position = mainScript.LeftEyeCamera.transform.position;
            //        WFDLargeLights.transform.rotation = mainScript.LeftEyeCamera.transform.rotation;
            //        WFDSmallLights.transform.position = mainScript.LeftEyeCamera.transform.position;
            //        WFDSmallLights.transform.rotation = mainScript.LeftEyeCamera.transform.rotation;
            //    }

            //}
            //else {
            //    WFDLargeLights.transform.position = WFDLargeLightsOrigPos;
            //    WFDLargeLights.transform.rotation = WFDLargeLightsOrigRot;
            //    WFDSmallLights.transform.position = WFDSmallLightsOrigPos;
            //    WFDSmallLights.transform.rotation = WFDSmallLightsOrigRot;
            //}
        }
        public void doTriggerPress(string myName) {
            Debug.Log("PRESSED myName:" + myName);
        }
        public void doTriggerRelease(string myName) {
            Debug.Log("RELEASED  myName:" + myName);
        }
    }
}
