using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Media;

namespace MainProject {
    public class DataRecorder : MonoBehaviour {
        StreamWriter sw;        
        public ProtocolRecorder myProtocolRecorder;
        public ProtocolReader myProtocolReader;
        public Button startLogButton;
        public Button stopLogButton;
        public PlaybackScript myPlaybackScript;
        public ManifestLoader myManifest;
        public TestChooser myTests;
        public EnvironmentChooser myEnvironments;
        public CalibrationManagement calibrationManage;
        private string sep; //What is sep?
        private int totalTypes;
        private string[] orderIDs;
        private string[] nodeLabels;
        private string secondLastLabel;
        private string lastLabel;

        public bool logging;
        public bool printImgs = false;
        //public List<string> TextToWrite;

        public string recordingPath;
        //public string patientIdentifier;
        public int indexCount;

        List<byte[]> textureListR;
        List<byte[]> textureListL;
        public Font aFont;
        public Texture2D fontTexture;
        public PatientLogging patientLog = new PatientLogging();
        public Text progressTextL;
        public Text progressTextR;

        void Start() {
            stopLogButton.gameObject.SetActive(false);
            logging = false;
            textureListR = new List<byte[]>();
            textureListL = new List<byte[]>();            
        }

        public string getTestsAndEnvironmentsNameStrings() {
            string tNames = "Tests: ";
            for (int j = 0; j < myTests.testArray.Length; j++) {
                tNames += myTests.testArray[j].name + ":" + j.ToString() + " ";
            }
            tNames += " Environments: ";
            for (int i = 0; i < myEnvironments.objArray.Length; i++) {
                tNames += myEnvironments.objArray[i].name + ":" + i.ToString() + " ";
            }
            return tNames;
        }

        public void StartLog() {
            stopLogButton.gameObject.SetActive(true);
            logging = true;
            recordingPath = CreateDirectoryForPatientEyeResults();
            sw = new StreamWriter(recordingPath + formatLegalPath("EyeData" + System.DateTime.Now + ".txt"));   //The file is created or Overwritten outside the Assests Folder.

            indexCount = 0;
            textureListR = new List<byte[]>();
            textureListL = new List<byte[]>();
            string tAndENames = getTestsAndEnvironmentsNameStrings();
            tAndENames += "////";
            sw.WriteLine("////Log Items:" + tAndENames);
            sep = myManifest.getSeparator();
            totalTypes = myManifest.getTotalTypes();
            orderIDs= new string[totalTypes];
            nodeLabels = new string[totalTypes];
            for (int i = 0; i < totalTypes - 2; i++) {
                 orderIDs[i] = myManifest.getOrderIDByType(i+1);
                 nodeLabels[i] = myManifest.getNodeLabelByID(i+1);
            }
            secondLastLabel = myManifest.getNodeLabelByID(totalTypes - 1);
            lastLabel = myManifest.getNodeLabelByID(totalTypes);
        }

        public void StartLogTEST() {
            stopLogButton.gameObject.SetActive(true);
            logging = true;
            sw = new StreamWriter("Logs\\" + formatLegalPath("EyeData" + System.DateTime.Now + ".txt"));   //The file is created or Overwritten outside the Assests Folder.

            indexCount = 0;
            string tAndENames = getTestsAndEnvironmentsNameStrings();
            tAndENames += "////";
            sw.WriteLine("////Log Items:" + tAndENames);
            sep = myManifest.getSeparator();
            totalTypes = myManifest.getTotalTypes();
            orderIDs = new string[totalTypes];
            nodeLabels = new string[totalTypes];
            for (int i = 0; i < totalTypes - 2; i++) {
                orderIDs[i] = myManifest.getOrderIDByType(i + 1);
                nodeLabels[i] = myManifest.getNodeLabelByID(i + 1);
            }
            secondLastLabel = myManifest.getNodeLabelByID(totalTypes - 1);
            lastLabel = myManifest.getNodeLabelByID(totalTypes);
        }

        //string GetCalibrationDataString() {
        //    string s = "";
        //    if(calibrationManage.I)
        //}

        public void LogButPressed() {
            myProtocolReader.showProtocolChooserStuff(true);
            startLogButton.gameObject.SetActive(false);
            myProtocolRecorder.setAllowRecording(false);
            //StartLog();
        }
        public void enableLoggingButtons(bool enableMe) {
            startLogButton.gameObject.SetActive(enableMe);
        }
        public void writeAThing(bool addIndex, MainScript script) {
            if (logging) {
                string logString = "";
                //tracked objects
                for (int j = 0; j < totalTypes; j++) {
                    for (int i = 1; i < totalTypes-1; i++) {
                        if (orderIDs[i-1] == j.ToString()) {
                            logString += nodeLabels[i - 1];
                            logString += myManifest.getTrackedValueByID(i);
                            logString += sep;
                        }
                    }
                }
                //the date
                logString += secondLastLabel +"("+ System.DateTime.Now.ToString()+")";
                //the index number
                logString += sep + lastLabel + "("+indexCount.ToString()+")";
                indexCount++;
                sw.WriteLine(logString, false);
                sw.Flush();

                //Add the current eye images to their respective lists to be printed later
                textureListR.Add(script.eyeTrackInterpret.GetLiveEyeFootage(0));
                textureListL.Add(script.eyeTrackInterpret.GetLiveEyeFootage(1));
            }
        } 

        //public int numOfPic = 0;
        //public void PrintEyeImage(MainScript script) {
        //    if (printImgs) {
                
        //        //To create new files
        //        //byte[] bytes = tex.EncodeToPNG();
        //        //File.WriteAllBytes("VidLogs\\" + formatLegalPath("EyeData" + numOfPic + ".png"), bytes);
        //        numOfPic++;
        //        testingCharacterAdd(textureListR[0], aFont);
        //    }
        //}

        //Testing
        public void testingCharacterAdd(int numOfIndex, Texture2D texture, Font someFont, string eye, string path, bool addCharacters) {
            LoggedEyeImage loggedImg = new LoggedEyeImage();
            loggedImg.Image = texture;
            if (addCharacters) {
                loggedImg.FontTexture = fontTexture;
                loggedImg.FontStyle = someFont;
                //loggedImg.AddTextToImageTexture(TextToWrite);
            }
            
            byte[] bytes = loggedImg.Image.EncodeToPNG();
            
            File.WriteAllBytes(path + formatLegalPath("EyeImage" + numOfIndex + eye + ".png"), bytes);            
        }

        public void testingCharacterAdd(int numOfIndex, byte[] texture, Font someFont, string eye, string path, bool addCharacters) {
            
            File.WriteAllBytes(path + formatLegalPath("EyeImage" + numOfIndex + eye + ".png"), texture);
        }

        public string CreateDirectoryForPatientEyeResults() {
            //string beginningPath = "Assets\\Resources\\";
            string beginningPath = "";
            if (!Directory.Exists(beginningPath + "Logs\\")){
                Directory.CreateDirectory(beginningPath + "Logs\\");
            }
            if (!Directory.Exists(beginningPath + "Logs\\" + "Patient_" + patientLog.pName + "\\")) {
                Directory.CreateDirectory(beginningPath + "Logs\\" + "Patient_" + patientLog.pName + "\\");
            }
            string path = (beginningPath + "Logs\\" + "Patient_" + patientLog.pName + "\\" + formatLegalPath(System.DateTime.Now.ToString()) + "\\");
            Directory.CreateDirectory(path);
            return path;
        }

        public void StopLogging() {
            if (logging) {
                sw.Close();

                Debug.Log("Closed stream");

                startLogButton.gameObject.SetActive(true);
                stopLogButton.gameObject.SetActive(false);
                logging = false;
                myProtocolRecorder.setAllowRecording(true);
                myProtocolReader.StopRecordingLog();
                if (textureListL == null || textureListL.Count == 0 || textureListR == null || textureListR.Count == 0) {
                    Debug.LogError("No images to print!");
                }
                else {
                    if (printImgs) {
                        Debug.LogWarning("Writing...");
                        string pathToUse = recordingPath;
                        StartCoroutine(CreateVideo(pathToUse, textureListL, "L", progressTextL));
                        StartCoroutine(CreateVideo(pathToUse, textureListR, "R", progressTextR));
                        //StartCoroutine(PrintImages(textureListL, pathToUse, "L"));
                        //StartCoroutine(PrintImages(textureListR, pathToUse, "R"));
                    }
                    
                }
                recordingPath = null;
            }
        }

        
        IEnumerator CreateVideo(string path, List<byte[]> textureList, string chara, Text progressTxt) {
            progressTxt.gameObject.SetActive(true);
             VideoTrackAttributes videoAttributes = new VideoTrackAttributes {
                frameRate = new MediaRational(90),
                width = 320,
                height = 240,
                includeAlpha = false
            };

            string filePath = Path.Combine(path, "eye_vid" + chara + ".mp4");
            using (MediaEncoder encoder = new MediaEncoder(filePath, videoAttributes)) {
                for(int i = 0; i < textureList.Count; i++) {
                    Texture2D tex = new Texture2D(320, 240, TextureFormat.RGBA32, false);
                    tex.LoadImage(textureList[i]);
                    Texture2D newTex = new Texture2D(320, 240, TextureFormat.RGBA32, false);
                    newTex.SetPixels(tex.GetPixels());
                    newTex.Apply();
                    encoder.AddFrame(newTex);
                    if(i%100 == 0) {
                        Resources.UnloadUnusedAssets();
                        System.GC.Collect();
                    }
                    if (i % 10 == 0) {
                        progressTxt.text = "Writing " + chara + " : " + System.Math.Round(((float)i / (float)textureList.Count)*100, 2) + "%";
                    }
                        yield return new WaitForEndOfFrame();
                }
                encoder.Dispose();
            }
            Debug.Log("Finished!");
            textureList.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            progressTxt.gameObject.SetActive(false);
        }

        private IEnumerator PrintImages(List<byte[]> list, string path, string eye) {
            for(int i = 0; i < list.Count; i++) {
                testingCharacterAdd(i + 1000000, list[i], aFont, eye, path, false);
                list[i] = null;
                yield return null;
            }
            Debug.LogWarning("Finished: " + eye);
            list.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            yield return null;
        }

        public void EnableLogging(bool imEnabled) {
            if (logging) {
                sw.Close();

                Debug.Log("Closed stream");

                logging = false;
            }
            if (!imEnabled) {
                startLogButton.gameObject.SetActive(false);
                stopLogButton.gameObject.SetActive(false);
            }
            else {
                startLogButton.gameObject.SetActive(true);
                stopLogButton.gameObject.SetActive(false);
            }
        }
        void OnApplicationQuit() {
            StopLogging();
        }
        public string formatLegalPath(string e) {
            e = e.Replace(" ", "_");
            e = e.Replace(":", "-");
            e = e.Replace("\\", "-");
            e = e.Replace("/", "-");
            return e;
        }

        public void TogglePrintImages(bool _printImgs) {
            printImgs = _printImgs;
        }


        ////Test
        //public RawImage img;

        //public void RunCapturedImages() {
        //    StartCoroutine(FrameCheck(img));
        //}

        //IEnumerator FrameCheck(RawImage placeHolder) {
        //    int i = 0;
        //    while (i < textureListR.Count) {
        //        placeHolder.texture = textureListR[i];
        //        i++;
        //        yield return new WaitForSeconds(0.05f);
        //    }
        //}
    }
}