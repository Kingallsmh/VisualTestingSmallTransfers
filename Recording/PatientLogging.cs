using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientLogging {

    public string pName = "Default";
    public string age = "NaN";
    public string height = "NaN";

    public PatientLogging() {

    }

    public PatientLogging(string _name, string _age, string _height) {
        pName = _name;
        age = _age;
        height = _height;
    }
}
