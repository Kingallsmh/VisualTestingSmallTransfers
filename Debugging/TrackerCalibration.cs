using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Research;

public class TrackerCalibration : MonoBehaviour {

    public GameObject eyeCalibrationImg;
    private bool eyesAreTracking = false;    

    public IEnumerator StartTracking(IEyeTracker eyeTracker) {
        while(!eyesAreTracking) {
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(Calibrate(eyeTracker));
    }

    public IEnumerator Calibrate(IEyeTracker eyeTracker) {
        // Create a calibration object.
        HMDBasedCalibration calibration = new HMDBasedCalibration(eyeTracker);
        // Enter calibration mode.
        calibration.EnterCalibrationMode();
        // Define the points in the HMD space we should calibrate at.
        Point3D[] pointsToCalibrate = new Point3D[] {
                new Point3D(9f, 9f, 20f),
                new Point3D(-9f, 9f, 20f),
                new Point3D(-9f, -9f, 20f),
                new Point3D(9f, -9f, 20f),
                new Point3D(0f, 0f, 20f),
            };
        // Collect data.
        foreach (Point3D point in pointsToCalibrate) {
            // Show an image on screen where you want to calibrate.
            Debug.Log("Show point in HMD space at ( " + point.X + " , " + point.Y + " , " + point.Z + " )");
            VRHeadsetDebugDisplay.Instance.SetText("Show point in HMD space at ( " + point.X + " , " + point.Y + " , " + point.Z + " )");
            DisplayImgAtPoint(new Vector3(point.X, point.Y, point.Z));
            // Wait a little for user to focus.
            yield return new WaitForSeconds(5);
            // Collect data.
            CalibrationStatus status = calibration.CollectData(point);
            if (status != CalibrationStatus.Success) {
                // Try again if it didn't go well the first time.
                // Not all eye tracker models will fail at this point, but instead fail on ComputeAndApply.
                calibration.CollectData(point);
                Debug.Log("Calibration point status : " + status);
            }
            VRHeadsetDebugDisplay.Instance.SetText("Ok, next!");
        }
        // Compute and apply the calibration.
        HMDCalibrationResult calibrationResult = calibration.ComputeAndApply();
        Debug.Log("Compute and apply returned : " + calibrationResult.Status);
        // See that you're happy with the result.
        // The calibration is done. Leave calibration mode.


        calibration.LeaveCalibrationMode();
        VRHeadsetDebugDisplay.Instance.SetText("All finished!");
    }

    public void DisplayImgAtPoint(Vector3 point) {
        eyeCalibrationImg.transform.localPosition = point;
    }

    public bool EyesAreTracking
    {
        get
        {
            return eyesAreTracking;
        }

        set
        {
            eyesAreTracking = value;
        }
    }
}
