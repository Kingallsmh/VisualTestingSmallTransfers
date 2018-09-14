using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


namespace MainProject {
    public class ProtocolReader : MonoBehaviour {

        public MainScript mainScript;
        public ManifestLoader myManfest;
        public TestChooser myTests;
        public EnvironmentChooser myEnvironments;
        public GameObject ProtocolChooserStuff;
        public DataRecorder myDataRecorder;
        public ProtocolRecorder myProtocolRecorder;
        public string[] currentProtocolData;
        public bool isPlaying;
        public string lastLine;
        public int lastLineNumber;
        public Text ProtocolName;

        public ProtocolSettings[] pSettingsArray;

        private ArrayList srArray;

        private int srLength;
        private bool timerAllowed;

        [Header("----------------------")]
        [Header("Streamreader timeindex")]
        StreamReader sr;
        public int timeIndex;

        //This plays back DURING recording of a log.
        public void createProtocolObject() {
            for (int i = 0; i < srArray.Count - 1; i++) {
                ProtocolSettings mySettings = new ProtocolSettings();
                string protocolString = srArray[i].ToString();
                //make sure the line is a valid data line.
                if (protocolString.IndexOf(myProtocolRecorder.separator) != -1 && protocolString.IndexOf("Data: TimeIndex") != 0) {
                    //set the timeCount
                    string intString = protocolString.Substring(0, protocolString.IndexOf(myProtocolRecorder.separator));
                    mySettings.timeCount = int.Parse(intString);
                    //trim the index off
                    protocolString = protocolString.Substring(protocolString.IndexOf(myProtocolRecorder.separator) + myProtocolRecorder.separator.Length);
                    //set the current test
                    string[] stringSeparators = new string[] { myProtocolRecorder.separator2 };
                    string[] envArray = protocolString.Split(stringSeparators, System.StringSplitOptions.None);
                    mySettings.currentTest = int.Parse(envArray[0]);
                    protocolString = envArray[1].ToString();
                    //set the current environment
                    string[] stringSeparators2 = new string[] { myProtocolRecorder.separator };
                    string[] envArray2 = protocolString.Split(stringSeparators2, System.StringSplitOptions.None);
                    mySettings.currentEnvironment = int.Parse(envArray2[1]);
                    mySettings.initSettings();
                    //set individual test settings up
                    protocolString = envArray2[0];
                    for (int j = 0; j < myTests.baseTests.Length; j++) {
                        string[] stringSeparators3 = new string[] { myProtocolRecorder.separator3 };
                        string[] envArray3 = protocolString.Split(stringSeparators3, System.StringSplitOptions.None);
                        for (int k = 0; k < envArray3.Length; k++) {
                            //each of the tests has a name (envArray4[0]) and the data for that test (envArray[1]) which is split by the testDataSeparator.
                            string[] stringSeparators4 = new string[] { myProtocolRecorder.separator4 };
                            string[] envArray4 = envArray3[k].Split(stringSeparators4, System.StringSplitOptions.None);
                            if (myTests.baseTests[j].name == envArray4[0] && envArray4[1].Length>0) {
                                string[] testSeparators = new string[] { myProtocolRecorder.testDataSeparator };
                                string[] testStringArray = envArray4[1].Split(testSeparators, System.StringSplitOptions.None);
                                if (j == 0) {
                                    mySettings.testSettings0 = myTests.baseTests[0].test0.createSettingsObject(testStringArray);
                                }
                                if (j == 1) {
                                    mySettings.testSettings1 = myTests.baseTests[1].test1.createSettingsObject(testStringArray);
                                    
                                }
                                if (j == 2) {
                                    mySettings.testSettings2 = myTests.baseTests[2].test2.createSettingsObject(testStringArray);
                                }
                                if (j == 3) {
                                    mySettings.testSettings3 = myTests.baseTests[3].test3.createSettingsObject(testStringArray);
                                }
                                if (j == 4) {
                                    mySettings.testSettings4 = myTests.baseTests[4].test4.createSettingsObject(testStringArray);
                                }
                                if (j == 5) {
                                    mySettings.testSettings5 = myTests.baseTests[5].test5.createSettingsObject(testStringArray);
                                }
                                if (j == 6) {
                                    mySettings.testSettings6 = myTests.baseTests[6].test6.createSettingsObject(testStringArray);
                                }
                                if (j == 7) {
                                    mySettings.testSettings7 = myTests.baseTests[7].test7.createSettingsObject(testStringArray);
                                }
                                if (j == 8) {
                                    mySettings.testSettings8 = myTests.baseTests[8].test8.createSettingsObject(testStringArray);
                                }
                            }
                        }
                    }
                }

                //Add item to the array and continue;
                pSettingsArray = AddItemToArray(pSettingsArray, mySettings);
            }
        }
        public void loadProtocol() {
            string path = EditorUtility.OpenFilePanel("Select a log file", "Protocols", "txt");
            if (path.Length != 0) {
                sr = new StreamReader(path);
                srLength = 0;
                timeIndex = 0;
                bool eofReached = false;
                while (!eofReached) {
                    srArray.Add(sr.ReadLine());
                    if (srArray[srArray.Count - 1] == null) {
                        eofReached = true;
                    }
                }
                srLength = srArray.Count;
                if (srArray.Count > 0) {
                    //Protocol was loaded - proceed.
                    createProtocolObject();
                    StartRecordingLog(path);
                }
            }
            else {
                //No protocol Loaded
                showProtocolChooserStuff(true);
            }
        }
        public string GetFilename(string myPath) {
            //string myRet = "";
            //myRet = myPath.Substring(myPath.LastIndexOf("/") + 1, myPath.Length - (myPath.LastIndexOf("/") + 1));
            //return myRet;
            return Path.GetFileName(myPath);
        }
        public void showProtocolChooserStuff(bool myState) {
            ProtocolChooserStuff.SetActive(myState);
        }
        void Start() {
            isPlaying = false;
            ProtocolName.gameObject.SetActive(false);
            showProtocolChooserStuff(false);
        }
        public void ChooseProtocol() {
            showProtocolChooserStuff(false);
            srArray = new ArrayList();
            StartCoroutine(waitToLoad());
            timeIndex = 0;

        }
        public IEnumerator waitToLoad() {
            yield return new WaitForSeconds(0.2f);
            loadProtocol();
        }
        public void SkipRecordProtocol() {
            //maybe add a diag "are you sure?"
            StartRecordingLog("NONE");
        }
        public void StartRecordingLog(string myPath) {
            showProtocolChooserStuff(false);
            ProtocolName.gameObject.SetActive(true);
            ProtocolName.text = "Using Protocol:" + GetFilename(myPath);
            lastLineNumber = 2;
            myDataRecorder.StartLog();
        }
        public void StopRecordingLog() {
            ProtocolName.gameObject.SetActive(false);
        }
        public void doInUpdate() {
            if (mainScript.myDataRecorder.logging) {
                int compareIndex = mainScript.myDataRecorder.indexCount;
                int protocolIndex = 0;
                if (lastLineNumber < pSettingsArray.Length - 1) {
                    protocolIndex = pSettingsArray[lastLineNumber].timeCount;
                    Debug.Log("compareIndex:"+compareIndex+" protocolIndex:"+protocolIndex);
                    if (compareIndex == protocolIndex) {
                        Debug.Log("Setting settings pSettings Length:"+pSettingsArray.Length);
                        if (pSettingsArray.Length > 0) {
                            setUpSettings(lastLineNumber);
                            lastLineNumber++;
                        }   
                    }
                }
                timeIndex++;
            }
        }
        public void setUpSettings(int currentIndex) {
            ProtocolSettings cpSettings = pSettingsArray[currentIndex];
            if (myTests.currentTest != cpSettings.currentTest) { 
                myTests.showTest(cpSettings.currentTest);
            }
            if(myEnvironments.currentEnvironment != cpSettings.currentEnvironment) { 
                myEnvironments.showObj(cpSettings.currentEnvironment);
            }
            for (int i = 0; i < myTests.baseTests.Length; i++) {
                if (i == 0) {
                    myTests.baseTests[i].test0.setSettings(cpSettings.testSettings0);
                }
                if (i == 1) {
                    myTests.baseTests[i].test1.setSettings(cpSettings.testSettings1);
                }
                if (i == 2) {
                    myTests.baseTests[i].test2.setSettings(cpSettings.testSettings2);
                }
                if (i == 3) {
                    myTests.baseTests[i].test3.setSettings(cpSettings.testSettings3);
                }
                if (i == 4) {
                    myTests.baseTests[i].test4.setSettings(cpSettings.testSettings4);
                }
                if (i == 5) {
                    myTests.baseTests[i].test5.setSettings(cpSettings.testSettings5);
                }
                if (i == 6) {
                    myTests.baseTests[i].test6.setSettings(cpSettings.testSettings6);
                }
                if (i == 7) {
                    myTests.baseTests[i].test7.setSettings(cpSettings.testSettings7);
                }
                if (i == 8) {
                    myTests.baseTests[i].test8.setSettings(cpSettings.testSettings8);
                }
            }
        }
        public bool checkLine() {
            bool myRet = false;
            int currentIndex = mainScript.myDataRecorder.indexCount;
            //check current TimeIndex For Line With New Settings
            return myRet;
        }
        public ProtocolSettings[] AddItemToArray(ProtocolSettings[] original, ProtocolSettings itemToAdd) {

            ProtocolSettings[] finalArray = new ProtocolSettings[original.Length + 1];
            for (int i = 0; i < original.Length; i++) {
                finalArray[i] = original[i];
            }

            finalArray[finalArray.Length - 1] = itemToAdd;

            return finalArray;
        }
    }
}