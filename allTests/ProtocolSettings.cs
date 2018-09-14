using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainProject {
    public class ProtocolSettings : MonoBehaviour {
        public BagoliniSettings testSettings0;
        public BoxStringSettings testSettings1;
        public DepthTestSettings testSettings2;
        public VernierSettings testSettings3;
        public DoubleMaddoxSettings testSettings4;
        public DepthPracticeSettings testSettings5;
        public VisualField1Settings testSettings6;
        public Worth4DotSettings testSettings7;
        public StandardEyeChartSettings testSettings8;

        public int currentTest;
        public int currentEnvironment;
        public int timeCount;
        public void initSettings() {
            testSettings0 = new BagoliniSettings();
            testSettings1 = new BoxStringSettings();
            testSettings2 = new DepthTestSettings();
            testSettings3 = new VernierSettings();
            testSettings4 = new DoubleMaddoxSettings();
            testSettings5 = new DepthPracticeSettings();
            testSettings6 = new VisualField1Settings();
            testSettings7 = new Worth4DotSettings();
            testSettings8 = new StandardEyeChartSettings();
        }
    }
}
