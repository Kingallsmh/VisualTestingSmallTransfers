using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; //Needed for XML functionality


namespace MainProject {
    public class ManifestLoader : MonoBehaviour {

        public MainScript mainScript;
        public XmlNodeList headersList;
        public GameObject type01;
        public GameObject type03;
        public GameObject type04;
        public GameObject type05;
        public GameObject type06;
        public GameObject type07;
        public GameObject type08;
        public GameObject type09;
        public GameObject type10;
        public string separator;

        public string decimals;


        //public Camera type02;
        public TestChooser myTests;
        public EnvironmentChooser myEnvironments;

        private XmlDocument xmlDoc;

        void Start() {
            Debug.Log("Loading XMLDOC");
            xmlDoc = new XmlDocument();
            xmlDoc.Load("Manifest.xml"); 
            headersList = xmlDoc.GetElementsByTagName("Header");
            decimals = getDecimals();
        }
        public string getNodeLabelByID(int e) {
            string nodeLabel = "";
            string compA = e.ToString();
            foreach (XmlNode headerInfo in headersList) {
                if (compA == headerInfo.Attributes["typeID"].Value) {
                    nodeLabel = headerInfo.Attributes["logLabel"].Value;
                }
            }
            return nodeLabel;
        }
        public int getTotalTypes() {
            return headersList.Count;
        }
        public string getSeparator() {
            var retString = "";
            XmlNodeList myHeaderList = xmlDoc.GetElementsByTagName("LogHeaders");
            foreach(XmlNode headerInfo in myHeaderList) {
                retString=headerInfo.Attributes["separator"].Value;
            }
            return retString;
        }
        public string getDecimals() {
            //Default decimal places is 4 unless otherwise specified in the manifest.
            string retString = "4";
            XmlNodeList myHeaderList = xmlDoc.GetElementsByTagName("LogHeaders");
            foreach (XmlNode headerInfo in myHeaderList) {
                retString = headerInfo.Attributes["decimals"].Value;
            }
            return retString;
        }
        public string getOrderIDByType(int e) {
            string nodeLabel = "";
            string compA = e.ToString();
            foreach (XmlNode headerInfo in headersList) {
                if (compA == headerInfo.Attributes["typeID"].Value) {
                    nodeLabel = headerInfo.Attributes["orderID"].Value;
                }
            }
            return nodeLabel;
        }
        public int getTypeByLabel(string e) {
            int myRet = -1;
            int totalTypes = getTotalTypes();
            for (int j = 0; j < totalTypes; j++) {
                int idCount = (j + 1);
                if (e == getNodeLabelByID(idCount)) {
                    myRet = idCount;
                }
            }
            return myRet;
        }
        public string stripIllegals(string e) {
            e = e.Replace(getSeparator(), "ERROR_0");
            e = e.Replace("////", "ERROR_1");
            e = e.Replace("(", "ERROR_2");
            return e;
        }
        public string getTrackedValueByID(int e) {
            //these have to match the ids in the Manifest.XML file.
            //date and index are always last
            string nodeValue = "";
            if (e == 1) {
                //Head Rotation
                //nodeValue = "(" + type01.transform.rotation.x.ToString() + "," + type01.transform.rotation.y.ToString() + "," + type01.transform.rotation.z.ToString() + "," + type01.transform.rotation.w.ToString()+")";
                nodeValue = type01.transform.rotation.ToString("F"+decimals);
            }
            if (e == 2) {
                //Head Position
                nodeValue = type01.transform.position.ToString("F"+decimals);
            }
            if (e == 3) {
                //Left Eye Rotation
                //nodeValue = type03.transform.rotation.ToString("F" + decimals);
                //nodeValue = mainScript.LeftEyeGazeData.ToString("F" + decimals);

                //Kyle's Attempt
                nodeValue = mainScript.eyeTrackInterpret.LeftEyeInfo.EyeNorm.ToString("F" + decimals);
            }
            if (e == 4) {
                //Right Eye Rotation
                //nodeValue = type04.transform.rotation.ToString("F" + decimals);
                //nodeValue = mainScript.RightEyeGazeData.ToString("F" + decimals);

                //Kyle's Attempt
                nodeValue = mainScript.eyeTrackInterpret.RightEyeInfo.EyeNorm.ToString("F" + decimals);
            }
            if (e == 5) {
                //Left Controller Position
                nodeValue = type05.transform.position.ToString("F" + decimals);
            }
            if (e == 6) {
                //Right Controller Position
                nodeValue = type06.transform.position.ToString("F" + decimals);
            }
            if (e == 7) {
                //Left Controller Rotation
                nodeValue = type05.transform.rotation.ToString("F" + decimals);
            }
            if (e == 8) {
                //Right Controller Rotation
                nodeValue = type06.transform.rotation.ToString("F" + decimals);
            }
            if (e == 9) {
                //Left Trigger Axis
                nodeValue = "(" + mainScript.ControllerLeft.TriggerValue.ToString("F" + decimals) +")";
            }
            if (e == 10) {
                //Right Trigger Axis
                nodeValue = "(" + mainScript.ControllerRight.TriggerValue.ToString("F" + decimals) +")";
            }
            if (e == 11) {
                nodeValue = mainScript.ControllerLeft.TopAxis.ToString("F" + decimals);
            }
            if (e == 12) {
                nodeValue = mainScript.ControllerRight.TopAxis.ToString("F" + decimals);
            }
            if (e == 13) {
                nodeValue = "(" + mainScript.ControllerLeft.TopPressed + ")";
            }
            if (e == 14) {
                nodeValue = "(" + mainScript.ControllerRight.TopPressed + ")";
            }
            if (e == 15) {
                nodeValue = "(" + mainScript.ControllerLeft.GripPressed + ")";
            }
            if (e == 16) {
                nodeValue = "(" + mainScript.ControllerRight.GripPressed + ")";
            }
            if (e == 17) {
                //Left Pupil
                //nodeValue = "(" + type09.transform.localScale.x.ToString("F" + decimals) +")";
                //nodeValue = "(" + mainScript.LeftEyePupilData.ToString("F" + decimals) + ")";
                nodeValue = "(" + mainScript.eyeTrackInterpret.LeftEyeInfo.PupilRadius.ToString("F" + decimals) + ")";
            }
            if (e == 18) {
                //Right Pupil
                //nodeValue = "(" + type10.transform.localScale.x.ToString("F" + decimals) +")";
                //nodeValue = "(" + mainScript.RightEyePupilData.ToString("F" + decimals) + ")";
                nodeValue = "(" + mainScript.eyeTrackInterpret.LeftEyeInfo.PupilRadius.ToString("F" + decimals) + ")";
            }
            if (e == 19) {
                if (myTests.currentTest != -1) { 
                    string testGuts = myTests.currentTest.ToString();
                    if (myTests.baseTests[myTests.currentTest].dataString.Length > 0) {
                        //Strip illegals makes sure whoever wrote the script doesnt try to put the manifest separators into their custom data string.
                        testGuts += ":"+stripIllegals(myTests.baseTests[myTests.currentTest].dataString);
                    }
                    nodeValue = "(" + testGuts+")";
                }
                else {
                    //Add brackets to the data because that is what's expected in the reading script
                    nodeValue = "(-1)";
                }
            }
            if (e == 20) {
                nodeValue = "(" + myEnvironments.currentEnvironment.ToString()+")";
            }
            if (e == 21) {
               nodeValue = System.DateTime.Now.ToString(); 
            }
            if (e == 22) {
                //this "true" value tells the file writer its the last item.
                nodeValue = "true";
            }
            return nodeValue;
        }
    }
}
