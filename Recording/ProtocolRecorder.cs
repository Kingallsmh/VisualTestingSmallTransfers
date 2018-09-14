using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Xml; //Needed for XML functionality


namespace MainProject {
    public class ProtocolRecorder: MonoBehaviour {

        //This happens on its own and cannot be activated during recording or playing back of logs.
        StreamWriter sw;
        public MainScript mainScript;
        public ManifestLoader myManifest;
        public DataRecorder myDataRecorder;
        public PlaybackScript myPlaybackScript;
        public bool isRecording;
        public bool allowRecording;
        public string lastLine;
        public TestChooser myTests;
        public EnvironmentChooser myEnvironments;
        public int timeCount;
        public string separator;
        public string separator2;
        public string separator3;
        public string separator4;
        public string testDataSeparator;
        public Button StartRecordingButton;
        public Button StopRecordingButton;
        public Text statusMessage;
        private int lastTimeCount;

        private XmlDocument xmlDoc;

        void Awake() {
            //xmlDoc = new XmlDocument();
            //xmlDoc.Load("Manifest.xml");
            isRecording = false;
            timeCount = 0;
            //this is the separator for the protocol event items between tests
            separator = "^^^";
            separator2 = "@#@";
            separator3 = "&*&";
            separator4 = "::";
            //this is the separator for the protocol event items inside the tests so dont use it here
            testDataSeparator = "~~";
            setAllowRecording(true);
        }
        public void setAllowRecording(bool allow) {
            //this is handled in the mainScript fixed update
            allowRecording = allow;
            setRecordingButs();
        }
        public void setRecordingButs() {
            if (allowRecording) {
                StartRecordingButton.gameObject.SetActive(true);
                StopRecordingButton.gameObject.SetActive(false);
                statusMessage.text = "RP enabled";
            }
            else {
                StartRecordingButton.gameObject.SetActive(false);
                StopRecordingButton.gameObject.SetActive(false);
                statusMessage.text = "RP disabled";
            }
        }
        public void startRecording() {
            mainScript.resetEverything();
            statusMessage.text = "Recording a Protocol...";
            StartRecordingButton.gameObject.SetActive(false);
            StopRecordingButton.gameObject.SetActive(true);
            if (myDataRecorder.logging) {
                myDataRecorder.StopLogging();
            }
            if (myPlaybackScript.isPlaying) {
                myPlaybackScript.closeTheFile();
            }
            else {
                myPlaybackScript.showLoadOpenButtons(false);
            }
            myDataRecorder.enableLoggingButtons(false);
            isRecording = true;
            sw = new StreamWriter("Protocols\\" + myDataRecorder.formatLegalPath("TestProtocol" + System.DateTime.Now + ".txt"));   //The file is created or Overwritten outside the Assests Folder.

            Debug.Log("Started new Stream");
            //For tracking number of streams
            DebugManager.instance.trackingList.Add(sw);

            string pString = "ProtocolRecorded data - Tests and Environments:";
            pString+=myDataRecorder.getTestsAndEnvironmentsNameStrings();
            sw.WriteLine(pString);
            sw.WriteLine("Data: TimeIndex "+separator+" Current Test "+separator2+" test data strings(test name "+separator3+" protocol settings x "+testDataSeparator+" y etc)"+separator+" Current Environment");

            Debug.Log("Wrote Line to current Stream");

            //open a new file io stream to write to 
        }
        public void stopRecording() {
            if (isRecording) {
                mainScript.resetEverything();
                StartRecordingButton.gameObject.SetActive(true);
                StopRecordingButton.gameObject.SetActive(false);
                isRecording = false;
                timeCount = 0;

                DebugManager.instance.trackingList.Remove(sw);
                sw.Close();

                Debug.Log("Close Stream");

                setRecordingButs();
                myPlaybackScript.showLoadOpenButtons(true);
                myDataRecorder.enableLoggingButtons(true);
            }
            //close/save the file.
        }
        public void updateSettings() {
            //while in isRecording mode:
            //add a line to the log when any setting is changed in any test.
            if (lastTimeCount == timeCount) {
                timeCount++;
                Debug.Log("Current time count = " + timeCount);
            }
            string mString = timeCount + separator;
            mString += makeTestLine();
            if (isRecording) {
                //write settingsString to file +timeCount.ToString();
                sw.WriteLine(mString, false);

                Debug.Log("Wrote line to Stream");

                sw.Flush();

                Debug.Log("Flushed Stream");

                lastTimeCount = timeCount;
            }

        }
        public string makeTestLine() {
            string mString = myTests.currentTest.ToString() + separator2;
            int totalTests = myTests.baseTests.Length;
            for (int i = 0; i < totalTests - 1; i++) {
                mString += myTests.baseTests[i].name;
                mString += separator4;
                mString += myTests.baseTests[i].protocolSettingsString;
                mString += separator3;
            }
            mString += myTests.baseTests[myTests.baseTests.Length - 1].name;
            mString += separator4;
            mString += myTests.baseTests[myTests.baseTests.Length - 1].protocolSettingsString;
            mString += separator;
            //write the environments
            mString += myEnvironments.currentEnvironment;
            return mString;
        }
        void OnApplicationQuit() {
            if(isRecording) {
                DebugManager.instance.trackingList.Remove(sw);
                sw.Close();

                Debug.Log("Closed Stream");

            }
        }
        public void doInUpdate() {
            if (isRecording) {
                timeCount++;
            }
        }
    }
}