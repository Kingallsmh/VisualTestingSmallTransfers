using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MainProject {
    public class PlaybackScript : MonoBehaviour {

        public MainScript mainScript;
        public GameObject CameraHead;
        public GameObject LeftEye;
        public GameObject RightEye;
        public GameObject LeftPupil;
        public GameObject RightPupil;
        public GameObject LeftController;
        public GameObject RightController;
        public GameObject LeftTrigger;
        public GameObject RightTrigger;
        public GameObject LeftTopButton;
        public GameObject LeftTopButtonDown;
        public GameObject RightTopButton;
        public GameObject RightTopButtonDown;
        public GameObject LeftGripButton;
        public GameObject LeftGripButtonDown;
        public GameObject RightGripButton;
        public GameObject RightGripButtonDown;
        public GameObject LeftTopAxis;
        public GameObject RightTopAxis;
        public GameObject LeftTopAxisOrienter;
        public GameObject RightTopAxisOrienter;
        public ProtocolRecorder myProtocolRecorder;
        public ProtocolReader myProtocolReader;

        public Camera leftCamera;
        public Camera rightCamera;

        public Quaternion leftCameraOrigRot;
        public Quaternion rightCameraOrigRot;

        public Vector3 leftCameraOrigPos;
        public Vector3 rightCameraOrigPos;

        public float AxialRadialExtent;

        public Text fileTitle;
        public Button fileOpenButton;
        public Button closeFileButton;
        public Button pauseButton;
        public Button playButton;
        public genericSlider myRCSlider;
        public GameObject mySliderStuff;
        public ManifestLoader myManifest;
        public Toggle myLoop;
        public Button[] frameBtns = new Button[2];
        public bool isPlaying;
        public bool inPlayMode;

        public float LTAxisTrackedValue;
        public float RTAxisTrackedValue;

        public TestChooser myTests;
        public EnvironmentChooser myEnvironments;

        public DataRecorder DataRecorder;

        private ArrayList srArray;

        private int srLength;
        private bool timerAllowed;


        public LoggingInfoDisplay logInfoDisplay;
        public List<Texture2D> LEyeImgList, REyeImgList;
        public VideoPlayer vpL, vpR;

        [Header("----------------------")]
        [Header("Streamreader timeindex")]
        StreamReader sr;
        public int timeIndex;

        void Awake() {            
            fileTitle.text = "";
            isPlaying = false;
            inPlayMode = false;
            AxialRadialExtent = 0.12f;
            leftCameraOrigRot = leftCamera.transform.rotation;
            rightCameraOrigRot = rightCamera.transform.rotation;
            leftCameraOrigPos = leftCamera.transform.position;
            rightCameraOrigPos = rightCamera.transform.position;
            myRCSlider.dD = rcSliderFunction;
        }
        public void rcSliderFunction() {
            if (srArray != null) {
                pausePlayBack();
                goToPercent(myRCSlider.sliderValue);
            }
        }
        void Start() {
            hidePlaybackUI();
        }
        public void hidePlaybackUI() {
            mySliderStuff.SetActive(false);
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            closeFileButton.gameObject.SetActive(false);
            myLoop.gameObject.SetActive(false);
            logInfoDisplay.gameObject.SetActive(false);
            frameBtns[0].gameObject.SetActive(false);
            frameBtns[1].gameObject.SetActive(false);
        }
        void OnApplicationQuit() {
            isPlaying = false;
            timerAllowed = false;
        }
        public void showLoadOpenButtons(bool enableMe) {
            fileOpenButton.gameObject.SetActive(enableMe);
        }
        public void doLoadPressed() {
            isPlaying = false;
            mySliderStuff.SetActive(false);
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            fileOpenButton.gameObject.SetActive(false);
            myLoop.gameObject.SetActive(false);
            DataRecorder.EnableLogging(false);
            srArray = new ArrayList();
            StartCoroutine(waitToLoad());
            timeIndex = 0;
            updateSlider();
            myProtocolRecorder.setAllowRecording(false);
        }
        public IEnumerator waitToLoad() {
            yield return new WaitForSeconds(0.2f);
            loadTheFile();
        }
        public void closeTheFile() {
            isPlaying = false;
            inPlayMode = false;
            mainScript.resetEverything();
            DataRecorder.EnableLogging(true);
            hidePlaybackUI();
            fileOpenButton.gameObject.SetActive(true);
            fileTitle.text = "";
            sr.Close();

            Debug.Log("Closed reader");

            leftCamera.transform.rotation = leftCameraOrigRot;
            rightCamera.transform.rotation = rightCameraOrigRot;
            leftCamera.transform.position = leftCameraOrigPos;
            rightCamera.transform.position = rightCameraOrigPos;
            myProtocolRecorder.setAllowRecording(true);
            REyeImgList.Clear();            
            LEyeImgList.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            logInfoDisplay.ResetAll();
        }
        public void enableLoop() {

        }

        string GetTxtPath(string[] paths) {
            foreach(string path in paths) {
                if (path.EndsWith(".txt")) {
                    return path;
                }
            }
            return null;
        }

        void GetPngInFolder(string[] paths) {
            int i = 0;
            foreach (string path in paths) {
                    if (path.EndsWith("R.png")) {
                        Debug.Log(path);
                        Texture2D img = new Texture2D(320, 240);
                        ImageConversion.LoadImage(img, File.ReadAllBytes(path));
                        REyeImgList.Add(img);
                    }
                    else if (path.EndsWith("L.png")) {
                        Texture2D img = new Texture2D(320, 240);
                        ImageConversion.LoadImage(img, File.ReadAllBytes(path));
                        LEyeImgList.Add(img);
                    }
                i++;
                if(i % 100 == 0) {
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
            }
            Debug.Log(REyeImgList.Count);
            Debug.Log(LEyeImgList.Count);
        }

        void GetVideo(string[] paths) {
            foreach(string path in paths) {
                if (path.EndsWith("R.mp4")) {
                    vpR.url = path;
                    //vpR.Play();
                }
                if (path.EndsWith("L.mp4")) {
                    vpL.url = path;
                    //vpL.Play();
                }
            }
             
        }

        public void loadTheFile() {
            //open the file browser and let user pick a log
            //string path = EditorUtility.OpenFilePanel("Select a log file", "Assets\\Resources\\Logs", "txt");

            //Get txtfile path or return
            string path = EditorUtility.OpenFolderPanel("Select A Log Folder", "Logs", "");
            string txtPath = GetTxtPath(Directory.GetFiles(path));
            if(txtPath == null) {
                Debug.LogError("There is no log available in the selected folder");
                return;
            }
            string[] s = Directory.GetFiles(path, "*.mp4");
            if (s == null || s.Length == 0) {
                Debug.LogWarning("There are no images!");
            }
            else {
                GetVideo(s);
                //GetPngInFolder(s);
            }
            timerAllowed = false;
            isPlaying = false;
            if (txtPath.Length != 0) {
                sr = new StreamReader(txtPath);

                Debug.Log("Created new reader");

                srLength = 0;
                timeIndex = 0;
                fileTitle.text = myProtocolReader.GetFilename(txtPath);
                bool eofReached = false;
                while (!eofReached) {
                    srArray.Add(sr.ReadLine());
                    if (srArray[srArray.Count - 1] == null) {
                        eofReached = true;
                    }
                }
                srLength = srArray.Count;
                if (srArray.Count > 0) {
                    inPlayMode = true;
                    playButton.gameObject.SetActive(true);
                    myLoop.gameObject.SetActive(true);
                    mySliderStuff.SetActive(true);
                    closeFileButton.gameObject.SetActive(true);
                    fileOpenButton.gameObject.SetActive(false);
                    logInfoDisplay.gameObject.SetActive(true);
                    frameBtns[0].gameObject.SetActive(true);
                    frameBtns[1].gameObject.SetActive(true);
                }
            }
            else {
                DataRecorder.EnableLogging(true);
                fileOpenButton.gameObject.SetActive(true);
                myProtocolRecorder.setAllowRecording(true);
            }

        }
        public void playTheFile() {
            //parse loaded file and apply transforms to each of the objects above\
            timerAllowed = true;
            isPlaying = true;
            if (srLength > 0) {
                goToIndex(timeIndex, true);
                ChangeShowingFrameInVideo(timeIndex);
            }            
            PlayPauseVideos(true);
            mainScript.resetEverything();
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            myLoop.gameObject.SetActive(true);
        }
        public void applyTheTransforms(int e) {
            //apply transforms at the current time index to all the objects
            //These have to match what is in Manifest.XML AND manifest loader in terms of order mytype etc.
            string st = GetLine(e);
            string[] stA = Regex.Split(st, myManifest.getSeparator());
            string quatString;
            string[] valArray;
            int myType = -1;
            int startString;
            int endString;
            int staLength = stA.Length;
            logInfoDisplay.ChangeIndexText(e);            
            
            for (int i = 0; i < staLength; i++) {
                if (stA[i].IndexOf("(") != -1) {
                    string label = (stA[i].Substring(0, stA[i].IndexOf("(")));
                    myType = myManifest.getTypeByLabel(label);
                }
                startString = stA[i].IndexOf("(") + 1;
                endString = stA[i].Length - (1 + startString);
                if (startString < 0) {
                    startString = 0;
                }
                if (endString < 1) {
                    endString = 1;
                }
                quatString = "";
                if (stA[i].Length > 0) {
                    quatString = stA[i].Substring(startString, endString);
                    valArray = Regex.Split(quatString, ",");                    
                    switch (myType) {
                        case 1:
                            Quaternion HeadRot = new Quaternion(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]), GetFloat(valArray[3]));
                            CameraHead.transform.rotation = HeadRot;
                            leftCamera.transform.rotation = HeadRot;
                            rightCamera.transform.rotation = HeadRot;
                            break;
                        case 2:
                            Vector3 HeadPos = new Vector3(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]));
                            CameraHead.transform.position = HeadPos;
                            leftCamera.transform.position = HeadPos;
                            rightCamera.transform.position = HeadPos;
                            break;
                        case 3:                            
                            Vector3 LE = new Vector3(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]));
                            //LeftEye.transform.rotation = mainScript.GetEyeRotQuat(LE, CameraHead.transform.rotation);
                            //Kyle's entry
                            LeftEye.transform.localRotation = mainScript.eyeTrackInterpret.AssignRotation(LE, 1);
                            logInfoDisplay.ChangeLEyeText(LE);
                            //Quaternion LE = new Quaternion(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]), GetFloat(valArray[3]));                            
                            break;
                        case 4:                            
                            Vector3 RE = new Vector3(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]));
                            //RightEye.transform.rotation = mainScript.GetEyeRotQuat(RE, CameraHead.transform.rotation);
                            //Kyle's entry
                            RightEye.transform.localRotation = mainScript.eyeTrackInterpret.AssignRotation(RE, 0);
                            logInfoDisplay.ChangeREyeText(RE);
                            //Quaternion RE = new Quaternion(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]), GetFloat(valArray[3]));
                            break;
                        case 5:
                            Vector3 LContPos = new Vector3(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]));
                            LeftController.transform.position = LContPos;
                            break;
                        case 6:
                            Vector3 RContPos = new Vector3(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]));
                            RightController.transform.position = RContPos;
                            break;
                        case 7:
                            Quaternion LContRot = new Quaternion(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]), GetFloat(valArray[3]));
                            LeftController.transform.rotation = LContRot;
                            break;
                        case 8:
                            Quaternion RContRot = new Quaternion(GetFloat(valArray[0]), GetFloat(valArray[1]), GetFloat(valArray[2]), GetFloat(valArray[3]));
                            RightController.transform.rotation = RContRot;
                            break;
                        case 9:
                            float LTAxis = GetFloat(quatString);
                            LeftTrigger.transform.rotation = mainScript.getQuatFromTriggerValue(LTAxis, mainScript.LeftTriggerOrienter.transform.rotation.eulerAngles);
                            if (LTAxis > 0) {
                                if (LTAxisTrackedValue == 0) {
                                    mainScript.DoTriggerPress("left");
                                }
                            }
                            else
                            if (LTAxis == 0) {
                                if (LTAxisTrackedValue > 0) {
                                    mainScript.DoTriggerRelease("left");
                                }
                            }
                            LTAxisTrackedValue = LTAxis;
                            break;
                        case 10:
                            float RTAxis = GetFloat(quatString);
                            RightTrigger.transform.rotation = mainScript.getQuatFromTriggerValue(RTAxis, mainScript.RightTriggerOrienter.transform.rotation.eulerAngles);
                            if (RTAxis > 0) {
                                if (RTAxisTrackedValue == 0) {
                                    mainScript.DoTriggerPress("right");
                                }
                            }
                            if (RTAxis == 0) {
                                if (RTAxisTrackedValue > 0) {
                                    mainScript.DoTriggerRelease("right");
                                }
                            }
                            RTAxisTrackedValue = RTAxis;
                            break;
                        case 11:
                            Vector2 LCtopAxisPos = new Vector2(GetFloat(valArray[0]), GetFloat(valArray[1]));
                            Vector3 newPos = LeftTopAxisOrienter.transform.localPosition;
                            newPos.x = LeftTopAxisOrienter.transform.localPosition.x + (LCtopAxisPos.x * AxialRadialExtent);
                            newPos.z = LeftTopAxisOrienter.transform.localPosition.z - (LCtopAxisPos.y * AxialRadialExtent);
                            LeftTopAxis.transform.localPosition = newPos;
                            break;
                        case 12:
                            Vector2 RCtopAxisPos = new Vector2(GetFloat(valArray[0]), GetFloat(valArray[1]));
                            Vector3 newRPos = RightTopAxisOrienter.transform.localPosition;
                            newRPos.x = RightTopAxisOrienter.transform.localPosition.x + (RCtopAxisPos.x * AxialRadialExtent);
                            newRPos.z = RightTopAxisOrienter.transform.localPosition.z - (RCtopAxisPos.y * AxialRadialExtent);
                            RightTopAxis.transform.localPosition = newRPos;
                            break;
                        case 13:
                            //Left top button pressed
                            if (quatString.ToLower().IndexOf("true") != -1) {
                                LeftTopButton.SetActive(false);
                                LeftTopButtonDown.SetActive(true);
                            }
                            else {
                                LeftTopButton.SetActive(true);
                                LeftTopButtonDown.SetActive(false);
                            }
                            break;
                        case 14:
                            //Right top button pressed
                            if (quatString.ToLower().IndexOf("true") != -1) {
                                RightTopButton.SetActive(false);
                                RightTopButtonDown.SetActive(true);
                            }
                            else {
                                RightTopButton.SetActive(true);
                                RightTopButtonDown.SetActive(false);
                            }
                            break;
                        case 15:
                            //Left Grip button pressed
                            if (quatString.ToLower().IndexOf("true") != -1) {
                                LeftGripButton.SetActive(false);
                                LeftGripButtonDown.SetActive(true);
                            }
                            else {
                                LeftGripButton.SetActive(true);
                                LeftGripButtonDown.SetActive(false);
                            }
                            break;
                        case 16:
                            //Right Grip button pressed
                            if (quatString.ToLower().IndexOf("true") != -1) {
                                RightGripButton.SetActive(false);
                                RightGripButtonDown.SetActive(true);
                            }
                            else {
                                RightGripButton.SetActive(true);
                                RightGripButtonDown.SetActive(false);
                            }
                            break;
                        case 17:
                            //LeftPupil.GetComponent<RectTransform>().localScale = new Vector3(GetFloat(quatString), LeftPupil.GetComponent<RectTransform>().localScale.y, GetFloat(quatString));
                            mainScript.setPupilWidth(LeftPupil, GetFloat(quatString));
                            logInfoDisplay.ChangeLEyeWidthText(GetFloat(quatString));
                            break;
                        case 18:
                            //RightPupil.GetComponent<RectTransform>().localScale = new Vector3(GetFloat(quatString), RightPupil.GetComponent<RectTransform>().localScale.y, GetFloat(quatString));
                            mainScript.setPupilWidth(RightPupil, GetFloat(quatString));
                            logInfoDisplay.ChangeREyeWidthText(GetFloat(quatString));
                            break;
                        case 19:
                            int endSpot = quatString.IndexOf(":");
                            int theCurrentTest;
                            if (endSpot > 0) {
                                theCurrentTest = int.Parse(quatString.Substring(0, endSpot));
                            }
                            else {
                                theCurrentTest = int.Parse(quatString);
                            }
                            //put a colon in there so its plus 2 to skip the colon in manifest loader.
                            string testDataString = stA[i].Substring(startString + 2, endString - 1);
                            //this is test specific data stored in an open ended string that the specific test script can record and playback.
                            if (myTests.currentTest != -1) {
                                myTests.baseTests[myTests.currentTest].dataString = testDataString;
                            }
                            if (theCurrentTest != myTests.currentTest) {
                                myTests.showTest(theCurrentTest);
                            }

                            break;
                        case 20:
                            int theCurrentEnvironment = int.Parse(stA[i].Substring(startString, endString));
                            if (theCurrentEnvironment != myEnvironments.currentEnvironment) {
                                myEnvironments.showObj(theCurrentEnvironment);
                            }
                            break;
                        case 21:
                            logInfoDisplay.ChangeTimeText(stA[i]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private float getTriggerValue(float compA, float compB) {
            if (Mathf.Abs(compA) > Mathf.Abs(compB)) {
                compA = Mathf.Abs(compA);
                compB = Mathf.Abs(compB);
            }
            else {
                compB = Mathf.Abs(compA);
                compA = Mathf.Abs(compB);
            }
            float compC = Mathf.Round(((compA - compB) / mainScript.TriggerMaxExtend) * 10000);
            compC = compC / 10000;
            if (compC < 0) {
                compC = compC * -1;
            }
            return compC;
        }

        public float GetFloat(string e) {
            float myFloat = 0;
            e = e.Trim();
            float.TryParse(e, out myFloat);
            return myFloat;
        }

        public int GetInt(float e) {
            int myInt = 0;
            myInt = (int)e;
            return myInt;
        }

        public void pausePlayBack() {
            timerAllowed = false;
            isPlaying = false;
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            PlayPauseVideos(false);
        }

        public void NextFrameButton() {
            if (!isPlaying && timeIndex < srLength) {
                timeIndex++;
                goToIndex(timeIndex, false);
                ChangeShowingFrameInVideo(timeIndex);
            }
        }

        public void PreviousFrameButton() {
            if (!isPlaying && timeIndex > 0) {
                timeIndex--;
                goToIndex(timeIndex, false);
                ChangeShowingFrameInVideo(timeIndex);
            }
        }

        public void goToIndex(int e, bool doContinue) {
            //for scrubbing to a particular index in the timeline
            applyTheTransforms(e);
            updateSlider();
            if (doContinue) {
                timeIndex++;
                if (timeIndex < srLength) {
                    //StartCoroutine(waitAndRunAgain());
                }
                else {
                    timeIndex = 0;
                    mainScript.resetEverything();
                    if (!myLoop.isOn) {
                        goToIndex(timeIndex, false);
                        isPlaying = false;
                        playButton.gameObject.SetActive(true);
                        pauseButton.gameObject.SetActive(false);
                    }
                    else {
                        goToIndex(timeIndex, true);
                        playButton.gameObject.SetActive(false);
                        pauseButton.gameObject.SetActive(true);
                    }
                }
            }

        }

        public void goToPercent(float e) {
            if (e > 0 && e < 101) {
                timerAllowed = false;
                float mFact = (float)e / 100;
                int newIndex = (int)Mathf.Round(srArray.Count * mFact);
                timeIndex = newIndex;
                goToIndex(timeIndex, false);
                ChangeShowingFrameInVideo(timeIndex);
                playButton.gameObject.SetActive(true);
                isPlaying = false;
                pauseButton.gameObject.SetActive(false);
            }
        }
        public IEnumerator waitAndRunAgain() {
            if (timerAllowed) {
                yield return new WaitForSeconds(0.0001f);
                goToIndex(timeIndex, true);
            }
        }
        public void updateSlider() {
            //scrubbing to a percentage of the entire file
            float tI = (float)timeIndex;
            float sL = (float)srLength;
            float sValue = (tI / sL) * 100;
            float sValueInt = Mathf.Round(sValue);
            setSliderValue(sValueInt);
        }
        string GetLine(int line) {
            string myRet = "";
            if (line < srArray.Count - 1) {
                myRet = srArray[line].ToString();
            }
            return myRet;
        }
        void setSliderValue(float e) {
            myRCSlider.setSliderValue(e);
        }

        bool ImagesExist(string path) {
            Texture2D firstImg = Resources.Load<Texture2D>(path + "EyeImage0L.png");
            if (firstImg) {
                return true;
            }
            else {
                return false;
            }
        }

        //Texture2D GetImageFromPatientFolder() {

        //}
        void PlayPauseVideos(bool play) {
            if (play) {
                vpR.Play();
                vpL.Play();
            }
            else {
                vpR.Pause();
                vpL.Pause();
            }
        }

        void ChangeShowingFrameInVideo(int frame) {
            vpR.frame = frame;
            vpL.frame = frame;
            PlayPauseVideos(true);
            PlayPauseVideos(false);
        }


    }
    
}
