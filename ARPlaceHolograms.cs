using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Barracuda;

using System.IO;
using TFClassify;
using System.Linq;
using System.Collections;


public class ARPlaceHolograms : MonoBehaviour
{
    // The prefab to instantiate on touch.
    [SerializeField]

    private GameObject _prefabToPlace;
    // Cache ARRaycastManager GameObject from ARCoreSession
    private ARRaycastManager _raycastManager;
    // Cache ARAnchorManager GameObject from ARCoreSession
    private ARAnchorManager _anchorManager;
    // List for raycast hits is re-used by raycast manager
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    public void checkTouchInput()
    {
        // Only consider single-finger touches that are beginning
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        // we only have 1 detection at time
        GameObject camera = GameObject.Find("Camera Image");
        PhoneARCamera camera2 = camera.GetComponent<PhoneARCamera>();
        int detectedObjects = camera2.boxSavedOutlines.Count();
        ScreenLog.Log("DETECTED OBJS:");
        ScreenLog.Log(detectedObjects.ToString());
        if (detectedObjects == 0) { return; }

        BoundingBox nearer = camera2.boxSavedOutlines[0];
        //how to get the object nearer to the touch?
        //camera2.permanentlyStoredDetections.Add(nearer);
        //ScreenLog.Log(touch.position.toString());
        // Perform AR raycast to any kind of trackable
        if (_raycastManager.Raycast(touch.position, Hits, TrackableType.All))
        {
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            var hitPose = Hits[0].pose;
            int dimensionXInt = (int)Math.Round(nearer.Dimensions.X);
            camera2.localization = true;
            ScreenLog.Log("INSIDE ARPLACEHOLOGRAMS");
            ScreenLog.Log("POSE");
            // Instantiate the prefab at the given position
            // Note: the object is not anchored yet!
            //Instantiate(_prefabToPlace, hitPose.position, hitPose.rotation);
            //CreateAnchor(Hits[0]);

            /*
            ScreenLog.Log("Dimension X of an object bounding box");
            ScreenLog.Log(dimensionXInt.ToString());

            ScreenLog.Log("Dimension X of the touch input");
            ScreenLog.Log(touch.position.ToString());

            // Debug output what we actually hit
            ScreenLog.Log("Dimension X of the nearer hit");
            ScreenLog.Log(Hits[0].pose.ToString());

            // Instantiate the prefab at the given position
            // Note: the object is not anchored yet!
            //Instantiate(_prefabToPlace, hitPose.position, hitPose.rotation);
            CreateAnchor(Hits[0]);
            // Debug output what we actually hit
            ScreenLog.Log($"Instantiated on: {Hits[0].hitType}");
            */
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //_raycastManager = GetComponent<ARRaycastManager>();
        //_anchorManager = GetComponent<ARAnchorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //checkTouchInput();
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        // ... here, we'll place the plane anchoring code!

        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
            var planeManager = GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                var oldPrefab = _anchorManager.anchorPrefab;
                _anchorManager.anchorPrefab = _prefabToPlace;
                anchor = _anchorManager.AttachAnchor(plane, hit.pose);
                _anchorManager.anchorPrefab = oldPrefab;
                Debug.Log($"Created anchor attachment for plane (id: {anchor.nativePtr}).");
                //Log.text = $"Created anchor attachment for plane (id: {anchor.nativePtr}).";
                return anchor;
            }
        }

        // Otherwise, just create a regular anchor at the hit pose

        // Note: the anchor can be anywhere in the scene hierarchy
        var instantiatedObject = Instantiate(_prefabToPlace, hit.pose.position, hit.pose.rotation);

        // Make sure the new GameObject has an ARAnchor component
        anchor = instantiatedObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = instantiatedObject.AddComponent<ARAnchor>();
        }
        Debug.Log($"Created regular anchor (id: {anchor.nativePtr}).");
        //Log.text = $"Created regular anchor (id: {anchor.nativePtr}).";

        return anchor;
    }


}
