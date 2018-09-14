using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MainProject;
using Tobii.Research;

public class CalibrationManagement : MonoBehaviour, ILoggingItem {

    bool inputFinished = false;
    public InputField IDField, ageField, valueField;
    public Image headsetRend, leftRemoteRend, rightRemoteRend;
    public Text heightText;
    public GameObject startButton;
    bool finishCheck = false;

    [Header("DEBUG")]
    public bool overrideEquipCheck = false;
    MainScript main;    

    public void StartCalibrationStage(MainScript _main) {
        transform.GetChild(0).gameObject.SetActive(true);
        main = _main;
        SetHeight();
        StartCoroutine(CheckForFinishedState());
    }

    IEnumerator CheckForFinishedState() {
        //Check through all the required fields
        //if yes, then activate Start button
        //if no, deactivate start button
        startButton.SetActive(false);
        bool routineCheck = false;
        while (!overrideEquipCheck && !finishCheck) {
            routineCheck = true;
            //HeadsetCheck
            if (!SetImageForVRHeadset(main.eyeTrackInterpret, main.eyeTrackInterpret.RightEyeInfo, main.eyeTrackInterpret.LeftEyeInfo)) {
                routineCheck = false;
            }
            //Check right remote
            if(!SetImageForVRRemotes(true, main.ControllerRight.gameObject)) {
                routineCheck = false;
            }
            //Check left remote
            if (!SetImageForVRRemotes(false, main.ControllerLeft.gameObject)) {
                routineCheck = false;
            }
            //Check inout fields
            //stuff
            inputFinished = CheckInputFields();
            if (!inputFinished) {
                routineCheck = false;
            }
            if (routineCheck) {
                startButton.SetActive(true);
            }
            else {
                startButton.SetActive(false);
            }
            yield return new WaitForSeconds(0.2f);
        }
        if (overrideEquipCheck) {
            startButton.SetActive(true);
        }
    }

    bool SetImageForVRHeadset(EyeTrackingInterpret _eyeTracker, EyeInfo rightEyeGaze, EyeInfo leftEyeGaze) {
        //if it's connected and tracking, create green image
        if(_eyeTracker == null) {
            //Debug.LogWarning("EyeTracker not set up or working");
            headsetRend.color = new Color(1, 0, 0);
            return false;
        }
        if(rightEyeGaze == null || leftEyeGaze == null) {
            headsetRend.color = new Color(1, 0, 0);
            return false;
        }
        if(rightEyeGaze.EyeNorm == Vector3.zero || leftEyeGaze.EyeNorm == Vector3.zero) {
            //Debug.LogWarning("Eyes are not being tracked");
            headsetRend.color = new Color(1, 0, 0);
            return false;
        }

        headsetRend.color = new Color(0, 1, 0);
        return true;
        //else, turn black/red
    }

    bool SetImageForVRRemotes(bool isRight, GameObject controller) {
        //if it's connected and tracking, create green image
        //else, turn black/red
        if (!controller.activeInHierarchy) {
            if (isRight) {
                //Debug.LogWarning("Right Controller not on or tracking");
                rightRemoteRend.color = new Color(1, 0, 0);
            }
            else {
                //Debug.LogWarning("Left Controller not on or tracking");
                leftRemoteRend.color = new Color(1, 0, 0);
            }
            return false;
        }
        else {
            if (isRight) {
                rightRemoteRend.color = new Color(0, 1, 0);
            }
            else {
                leftRemoteRend.color = new Color(0, 1, 0);
            }
            return true;
        }
    }    

    bool CheckInputFields() {
        if(IDField.text == "") {
            return false;
        }
        if(ageField.text == "") {
            return false;
        }
        if(valueField.text == "") {
            return false;
        }
        return true;
    }

    public void SetHeight() {
        float offset = 0.122f; //Guesstimation but should be close
        heightText.text = (offset + main.RightEyeCamera.transform.position.y).ToString("F3") + "m";
    }

    public void StartButtonPress() {
        //Add info that taken to a log file
        main.myDataRecorder.patientLog = new PatientLogging(IDField.text, ageField.text, heightText.text);
        main.UICanvas.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
        finishCheck = true;
    }

    //Output for logged items
    public string OutputString() {
        if (inputFinished) {
            return "ID : " + IDField.text + " name: " + valueField.text + " Age: " + ageField;
        }
        else {
            return "No Available Data";
        }
    }

    //Read logged values and interpret them
    public void ReadInput() {
        
    }

    public bool InputFinished
    {
        get
        {
            return inputFinished;
        }

        set
        {
            inputFinished = value;
        }
    }
}
