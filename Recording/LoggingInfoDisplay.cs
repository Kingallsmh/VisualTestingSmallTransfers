using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggingInfoDisplay : MonoBehaviour {

    public Text timeText, indexText, RERotText, REWidthText, LERotText, LEWidthText;
    public RawImage LEyeImg, REyeImg;

    public void ChangeTimeText(string time) {
        timeText.text = "Time: " + time;
    }

    public void ChangeIndexText(int indexNum) {
        indexText.text = "Index: " + indexNum;
    }

    public void ChangeREyeText(Vector3 rot) {
        RERotText.text = "Right Eye Rotation: " + rot;
    }

    public void ChangeLEyeText(Vector3 rot) {
        LERotText.text = "Left Eye Rotation: " + rot;
    }

    public void ChangeREyeWidthText(float width) {
        REWidthText.text = "Right Eye Width: " + width;
    }

    public void ChangeLEyeWidthText(float width) {
        LEWidthText.text = "Left Eye Width: " + width;
    }

    public void ChangeLEyeImage(Texture2D img) {
        //Texture2D image = new Texture2D(320, 240);
        //image.LoadImage(img);
        LEyeImg.texture = img;
    }

    public void ChangeREyeImage(Texture2D img) {
        //Texture2D image = new Texture2D(320, 240);
        //image.LoadImage(img);
        REyeImg.texture = img;
    }

    public void ResetAll() {
        ChangeTimeText("");
        ChangeIndexText(0);
        ChangeREyeText(Vector3.zero);
        ChangeLEyeText(Vector3.zero);
        ChangeREyeWidthText(0);
        ChangeLEyeWidthText(0);
    }
}
