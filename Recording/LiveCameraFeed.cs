using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiveCameraFeed : MonoBehaviour {
    
    public int targetFPS = 20;
    public Camera camObj;

    public Texture2D[] eyeTexture = new Texture2D[2];
    byte[][] eyeImageRaw = new byte[2][];
    MeshRenderer[] eyeRenderer = new MeshRenderer[2];
    bool[] eyePublishingInitialized = new bool[2];

    public RawImage[] eyeRends;

    // Use this for initialization
    public void Startup () {
        // Main
        if (camObj == null) {
            Debug.Log("Frame Publisher could not find a Camera gameobject");
            return;
        }
        eyePublishingInitialized = new bool[] { false, false };

        PupilTools.SubscribeTo("frame.");

        PupilTools.Send(new Dictionary<string, object> { { "subject", "start_plugin" }, { "name", "Frame_Publisher" } });
        PupilTools.OnReceiveData += CustomReceiveData;
    }

    private void OnEnable() {
        //Startup();    
    }

    void CustomReceiveData(string topic, Dictionary<string, object> dictionary, byte[] thirdFrame = null) {
        //		if (topic == "pupil.0" || topic == "pupil.1")
        //			UpdateSphereProjection (eyeIndex: topic == "pupil.0" ? 0 : 1, dictionary: dictionary);

        if (thirdFrame == null) {
            return;
        }            
        
        if (topic == "frame.eye.0") {
            if (!eyePublishingInitialized[0])
                InitializeFramePublishing(0);
            eyeImageRaw[0] = thirdFrame;
        }
        else if (topic == "frame.eye.1") {
            if (!eyePublishingInitialized[1])
                InitializeFramePublishing(1);
            eyeImageRaw[1] = thirdFrame;
        }
    }

    public void InitializeFramePublishing(int eyeIndex) {
        Transform parent = camObj.transform;
        Shader shader = Shader.Find("Unlit/Texture");

        eyeTexture[eyeIndex] = new Texture2D(100, 100);
        //eyeRenderer[eyeIndex] = InitializeEyeObject(eyeIndex, parent);        
        eyeRends[eyeIndex].material = new Material(shader);
        eyeRends[eyeIndex].material.mainTexture = eyeTexture[eyeIndex];
        if (eyeIndex == 1)
            eyeRends[eyeIndex].material.mainTextureScale = new Vector2(-1, -1);

        eyePublishingInitialized[eyeIndex] = true;
    }

    MeshRenderer InitializeEyeObject(int eyeIndex, Transform parent) {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = "Eye " + eyeIndex.ToString();
        go.transform.parent = parent;
        go.transform.localEulerAngles = Vector3.left * 90;
        go.transform.localScale = Vector3.one * 0.05f;
        go.transform.localPosition = new Vector3((eyeIndex == 0 ? -0.3f : 0.3f), -0.5f, 1.9999f);

        Destroy(go.GetComponent<Collider>());

        //		if (visualizeSphereProjection)
        //		{
        //			var circleGO = GameObject.Instantiate (go, go.transform);
        //			circleGO.transform.localPosition = Vector3.zero;
        //			circleGO.transform.localEulerAngles = Vector3.zero;
        //			circleGO.transform.localScale = Vector3.one;
        //			sphereProjectionMaterial [eyeIndex] = new Material (Resources.Load<Material> ("Materials/Circle"));
        //			sphereProjectionMaterial [eyeIndex].SetColor ("_TintColor", Color.green);
        //			circleGO.GetComponent<MeshRenderer> ().material = sphereProjectionMaterial [eyeIndex];
        //		}

        return go.GetComponent<MeshRenderer>();
    }

    public void ToggleLiveVideo() {
        eyeRends[0].gameObject.SetActive(!eyeRends[0].gameObject.activeInHierarchy);
        eyeRends[1].gameObject.SetActive(!eyeRends[1].gameObject.activeInHierarchy);
    }

    float lastUpdate;
    public void UpdateLiveFeed() {
        //Limiting the MainThread calls to framePublishFramePerSecondLimit to avoid issues. 20-30 ideal.


        if ((Time.time - lastUpdate) >= (1f / targetFPS)) {
            for (int i = 0; i < 2; i++)
                if (eyePublishingInitialized[i])
                    eyeTexture[i].LoadImage(eyeImageRaw[i]);
            lastUpdate = Time.time;
        }
    }

    private void OnDisable() {
        UnityEngine.Debug.Log("Disconnected");

        PupilTools.Send(new Dictionary<string, object> { { "subject", "stop_plugin" }, { "name", "Frame_Publisher" } });

        PupilTools.UnSubscribeFrom("frame.");

        for (int i = eyeRenderer.Length - 1; i >= 0; i--)
            if (eyeRenderer[i] != null && eyeRenderer[i].gameObject != null)
                Destroy(eyeRenderer[i].gameObject);

        PupilTools.OnReceiveData -= CustomReceiveData;
    }

    void OnDestroy() {
        UnityEngine.Debug.Log("Disconnected");

        PupilTools.Send(new Dictionary<string, object> { { "subject", "stop_plugin" }, { "name", "Frame_Publisher" } });

        PupilTools.UnSubscribeFrom("frame.");

        for (int i = eyeRenderer.Length - 1; i >= 0; i--)
            if (eyeRenderer[i] != null && eyeRenderer[i].gameObject != null)
                Destroy(eyeRenderer[i].gameObject);

        PupilTools.OnReceiveData -= CustomReceiveData;
    }

    //public Texture2D GetValueOfTexture(int eye) {
    //    Texture2D img = new Texture2D(100, 100);
    //    img.LoadImage(eyeImageRaw[eye]);
    //    return img;
    //}

    public byte[] GetValueOfTexture(int eye) {
        byte[] newArray = new byte[eyeImageRaw[eye].Length];
        eyeImageRaw[eye].CopyTo(newArray, 0);
        return newArray;
    }
}
