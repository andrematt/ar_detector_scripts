using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceTrackedImages : MonoBehaviour
{
    // Cache AR tracked images manager from ARCoreSession
    private ARTrackedImageManager _trackedImagesManager;

    // List of prefabs - these have to have the same names as the 2D images in the reference image library
    public GameObject[] ArPrefabs;

    // Internal storage of created prefabs for easier updating
    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    // Reference to logging UI element in the canvas
    public UnityEngine.UI.Text Log;

    void Awake()
    {
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
        anchorCreator = FindObjectOfType<AnchorCreator>(); 
    }

    void OnEnable()
    {
        _trackedImagesManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _trackedImagesManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {

    }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Come salvare la posizione di un oggetto quando lo stato diventa "success"?
        // Provo a copiare da qua
        // https://github.com/Unity-Technologies/arfoundation-demos/blob/master/Assets/ImageTracking/Scripts/ImageTrackingObjectManager.cs
        foreach (ARTrackedImage image in eventArgs.updated)
        {
            // image is tracking or tracking with limited state, show visuals and update it's position and rotation
            if (image.trackingState == TrackingState.Tracking)
            {
                trackedImageTransform = image.transform;
                //ScreenLog.Log(trackedImageTransform.position[0].ToString()+trackedImageTransform.position[1].ToString()+ trackedImageTransform.position[2].ToString());
                var imageName = image.referenceImage.name;
                if (imageName == "apollo")
                {
                    setApolloTransform(trackedImageTransform);
                    setInstantiatedApollo(true);
                    anchorCreator.anchorObjectsFromImageTracking();
                    //ScreenLog.Log("POSITION OF THE FIRST VIZ");
                    //Vector3 translatedPosition = new Vector3(trackedImageTransform.localPosition[0], trackedImageTransform.localPosition[1] + (float)1.5, trackedImageTransform.localPosition[2] + (float)2);
                    //ScreenLog.Log(translatedPosition[0].ToString()+ " " + translatedPosition[1].ToString()+ " " + translatedPosition[2].ToString());
                    //Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
                    
                    //ScreenLog.Log("POSITION OF THE SECOND VIZ");
                    //Vector3 translatedPosition2 = new Vector3(trackedImageTransform.localPosition[0], trackedImageTransform.localPosition[1] + (float)1.5, trackedImageTransform.localPosition[2] + (float)5);
                    //ScreenLog.Log(translatedPosition2[0].ToString()+ " " + translatedPosition2[1].ToString()+ " " + translatedPosition2[2].ToString());
                    //Instantiate(_anchorPrefab, translatedPosition2, Quaternion.identity);
                    /*
        Vector3 translatedPosition = new Vector3((float)0.5, (float)0.01, (float)5.99);
            GameObject test = Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
      
        Vector3 translatedPosition2 = new Vector3((float)2.15, (float)0.77, (float)0.11);
            GameObject test2 = Instantiate(_anchorPrefab, translatedPosition2, Quaternion.identity);
                    */

                }
            }
        }
            /*

                // Good reference: https://forum.unity.com/threads/arfoundation-2-image-tracking-with-many-ref-images-and-many-objects.680518/#post-4668326
                // https://github.com/Unity-Technologies/arfoundation-samples/issues/261#issuecomment-555618182

                // Go through all tracked images that have been added
                // (-> new markers detected)
                foreach (var trackedImage in eventArgs.added)
        {
            // Get the name of the reference image to search for the corresponding prefab
            var imageName = trackedImage.referenceImage.name;
            ScreenLog.Log("FOUND AN IMAGE! " + imageName);
            foreach (var curPrefab in ArPrefabs)
            {
                if (string.Compare(curPrefab.name, imageName, StringComparison.Ordinal) == 0
                    && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    // Found a corresponding prefab for the reference image, and it has not been instantiated yet
                    // -> new instance, with the ARTrackedImage as parent (so it will automatically get updated
                    // when the marker changes in real-life)
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    // Store a reference to the created prefab
                    _instantiatedPrefabs[imageName] = newPrefab;
                    Log.text = $"{Time.time} -> Instantiated prefab for tracked image (name: {imageName}).\n" +
                               $"newPrefab.transform.parent.name: {newPrefab.transform.parent.name}.\n" +
                               $"guid: {trackedImage.referenceImage.guid}";
                    ShowAndroidToastMessage("Instantiated!");
                }
            }
        }

        // Disable instantiated prefabs that are no longer being actively tracked
        foreach (var trackedImage in eventArgs.updated)
        {
            _instantiatedPrefabs[trackedImage.referenceImage.name]
                .SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        // Remove is called if the subsystem has given up looking for the trackable again.
        // (If it's invisible, its tracking state would just go to limited initially).
        // Note: ARCore doesn't seem to remove these at all; if it does, it would delete our child GameObject
        // as well.
        foreach (var trackedImage in eventArgs.removed)
        {
            // Destroy the instance in the scene.
            // Note: this code does not delete the ARTrackedImage parent, which was created
            // by AR Foundation, is managed by it and should therefore also be deleted
            // by AR Foundation.
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            // Also remove the instance from our array
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);

            // Alternative: do not destroy the instance, just set it inactive
            //_instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(false);

            Log.text = $"REMOVED (guid: {trackedImage.referenceImage.guid}).";
        }
            */
    }

    public void setInstantiatedApollo(bool myBool)
    {
        instantiatedApollo = myBool;
    }

    public Boolean getInstantiatedApollo()
    {
        //ScreenLog.Log("Getting instantiated Apollo!");
        return instantiatedApollo;
    }
    
    public Transform getApolloTransform()
    {
        return apolloTransform;
    }

    public void setApolloTransform(Transform transform)
    {
        apolloTransform = transform;
    }

    public AnchorCreator anchorCreator;
    public Transform apolloTransform;
    public GameObject _anchorPrefab;
    public Transform trackedImageTransform;
    public bool instantiatedApollo = false;
}
