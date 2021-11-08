using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

/*
 * Manages the anchors (points in the real world uses as reference 
 * for the visualizations) and the raycast (activated by the user tap)
 */
public class AnchorCreator : MonoBehaviour
{
    public void RemoveAllAnchors()
    {
        Debug.Log($"DEBUG: Removing all anchors ({anchorDic.Count})");
        foreach (var anchor in anchorDic)
        {
            Destroy(anchor.Key.gameObject);
        }
        s_Hits.Clear();
        anchorDic.Clear();
        trackableList.Clear();
        raycastHitDic.Clear();
        labelsOfAnchors.Clear();
    }

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        GameObject cameraImage = GameObject.Find("Camera Image");
        phoneARCamera = cameraImage.GetComponent<PhoneARCamera>();
    }
    
    /*
     * here we'll place the plane anchoring code
     */
    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {

        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
           ScreenLog.Log($"DEBUG: Creating plane anchor. distance: {hit.distance}. session distance: {hit.sessionRelativeDistance} type: {hit.hitType}.");
           return m_AnchorManager.AttachAnchor(plane, hit.pose);
        }
        else
        {
            // create a regular anchor at the hit pose
            ScreenLog.Log($"DEBUG: Creating regular anchor. distance: {hit.distance}. session distance: {hit.sessionRelativeDistance} type: {hit.hitType}.");
            return m_AnchorManager.AddAnchor(hit.pose);
        }
    }
    
    /*
     * Info log
     */
    public void hitsToAnchorLogger(ARRaycastHit hit, BoundingBox outline)
    {
        Debug.Log("Managing an hit");
        Debug.Log("Hit:" + hit.hitType);
        Debug.Log("outline label:" + outline.Label);
    }
    
    /*
     * 
     */
    public bool alreadyInAnchorList(BoundingBox outline)
    {
        return labelsOfAnchors.Contains(outline.Label);
    }
    
    /*
     * trabableDic is a list of <ARTrackable, anchor> tuples
     * check if the passed ARTrackable is already on this dic 
     */
    public bool alreadyInTrackableDic(ARRaycastHit hit)
    {
        ARTrackable trackable = hit.trackable;
        ScreenLog.Log("CHECKING TRACKABLE!!");
        return raycastHitDic.ContainsKey(hit);
    }
    
    // Search the trackable list 
    public bool trackableAlreadyUsed(ARTrackable trackable)
    {
        return trackableList.Contains(trackable);
    }

    // 
    public BoundingBox returnOutlineFromAnchor(ARAnchor anchor)
    {
        return anchorDic[anchor];
    }

    public ARAnchor returnAnchorFromTrackable(ARTrackable trackable)
    {
        ScreenLog.Log("RETURN ANCHOR FROM TRACKABLE");
        return trackableDic[trackable];
    }
    

    /*
     * Creates a new anchor on the passed position
     * opens the "edit rule element" panel if the outline label is used and on that trackable there is an anchor
     * otherwise, opens the "new rule element" panel 
     */
    private bool Pos2AnchorNew(float x, float y, BoundingBox outline, ARRaycastHit hit)
    {
        // GameObject anchorObj = m_RaycastManager.raycastPrefab;
        // TextMesh anchorObj_mesh = anchorObj.GetComponent<TextMesh>();
        anchorObj_mesh.text = $"{outline.Label}: {(int)(outline.Confidence * 100)}%";
        //TextMesh anchorObj = GameObject.Find("New Text").GetComponent<TextMesh>();

        //if (alreadyInAnchorList(outline) && alreadyInRaycastHitDic(hit)) 
        if (alreadyInAnchorList(outline) && trackableAlreadyUsed(hit.trackable)) 
        {
            //Load the edit rule element from the RuleElementCanvas game object 
            Debug.Log("anchor already exists: load the edit rule element canvas");
            if (!NewElementScript.getIsOpen() && !EditElementScript.getIsOpen())
            {
                EditElementScript.editElement(outline);
                EditElementScript.setIsOpen(true);
            }
            //phoneARCamera.localization = false;
            return false;
        }
        else
        {
            // Create a new anchor
            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                labelsOfAnchors.Add(outline.Label);
                Debug.Log("anchor created: localization false");
                phoneARCamera.localization = false;
                Debug.Log($"DEBUG: creating anchor. {outline}");
                if (!NewElementScript.getIsOpen() && !EditElementScript.getIsOpen())
                {
                    NewElementScript.newElement(outline);
                    NewElementScript.setIsOpen(true);
                }
                // Remember the anchor so we can remove it later.
                ARTrackable hitted = hit.trackable;
                anchorDic.Add(anchor, outline);
                // Also remember the raycast hit with the associated anchor
                //raycastHitDic.Add(hit, anchor);
                // and the hitted trackable.
                trackableList.Add(hitted);
                trackableDic.Add(hitted, anchor);

                Debug.Log($"DEBUG: Current number of anchors {anchorDic.Count}.");
                return true;
            }
            else
            {
                Debug.Log("DEBUG: Error creating anchor");
                return false;
            }
        }
    }
    
    /*
     * Collection of checks to decide if a touch is not useful
     */ 
    public bool checkReturnConditions()
    {
        //Return if there is no touch input
        if (Input.touchCount == 0)
        { 
            return true;
        }
        
        //Return if the touch is over an UI element 
        foreach (Touch touchLoop in Input.touches)
        {
            int id = touchLoop.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                return true;
            }
        }
        
        //Return if a game object (e.g. UI element) is currenctly selected 
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return true;
        }
        return false;
    }

    /*
     * if a touch is detected && it hits a trackable: 
     *   check if there is an object anchored in the position
     *     if yes, open the edit element panel & return
     *   check if an object is currenctly detected 
     *     if yes, check if there is an object anchord on that position 
     *       if yes, open edit element panel
     *       if no, open new element panel
     */
    void Update()
    {
        if (checkReturnConditions()){
            return;
        }
        
        // Get stuff
        boxSavedOutlines = phoneARCamera.boxSavedOutlines;
        shiftX = phoneARCamera.shiftX;
        shiftY = phoneARCamera.shiftY;
        scaleFactor = phoneARCamera.scaleFactor;
        GameObject camera = GameObject.Find("Camera Image");
        PhoneARCamera camera2 = camera.GetComponent<PhoneARCamera>();

        // Only consider single-finger touches that are beginning
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return; 
        }

        // Perform the raycast, we want all the trackables
        if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes)) //last arg is instead of TrackableType.All, because we don't want to capture the PlaneEstimated: too imprecise!
        {
            // Only returns true if there is at least one hit
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            if (s_Hits.Count > 0)
            {
                var hit = s_Hits[0];
                var hitPose = s_Hits[0].pose;
                ScreenLog.Log("RAYCAST HIT SOMETHING!!!" + hit.hitType);
                
                // If something is hitted, before all test if it is an already placed anchor. 
                //if (alreadyInTrackableDic(hit))
                if (trackableAlreadyUsed(hit.trackable))
                {
                    // In this case open the edit rule element panel and return
                    if (!NewElementScript.getIsOpen() && !EditElementScript.getIsOpen())
                    {
                        ScreenLog.Log("RAYCAST HIT AN ALREADY USED TRACKABLE && EDIT PANEL NOT OPEN!!!");
                        //TODO Something does not work here: the edit element panel is not opened!!
                        //retreive the trackable anchor from the hit
                        ARAnchor myAnchor = returnAnchorFromTrackable(hit.trackable);
                        //retrieve the outline linked to that trackable
                        ScreenLog.Log("RETREIVED ARANCHOR!!!!");
                        BoundingBox myBoundingBox = returnOutlineFromAnchor(myAnchor);
                        ScreenLog.Log("RETREIVED BOUNDINGBOX!!!!");
                        ScreenLog.Log(myBoundingBox.Label);
                        EditElementScript.editElement(myBoundingBox);
                        EditElementScript.setIsOpen(true);
                    }
                    return;
                }
                
                // create anchor for new bounding boxes
                foreach (var outline in boxSavedOutlines)
                {
                    ScreenLog.Log("RAYCAST HIT SOMETHING && THERE ARE SAVED OUTLINES");
                    if (outline.Used) // Used to not place more anchors on the same outline
                    {
                        ScreenLog.Log("USED OUTLNE!!!");
                        continue;
                    }

                    // Note: rect bounding box coordinates starts from top left corner.
                    // AR camera starts from borrom left corner.
                    // Need to flip Y axis coordinate of the anchor 2D position when raycast
                    var xMin = outline.Dimensions.X * this.scaleFactor + this.shiftX;
                    var width = outline.Dimensions.Width * this.scaleFactor;
                    var yMin = outline.Dimensions.Y * this.scaleFactor + this.shiftY;
                    yMin = Screen.height - yMin;
                    var height = outline.Dimensions.Height * this.scaleFactor;

                    float center_x = xMin + width / 2f;
                    float center_y = yMin - height / 2f;
                   
                    if (Pos2AnchorNew(center_x, center_y, outline, hit))
                    {
                        outline.Used = true; //No anchor can be placed on this outline
                    }
                }
            }
        }
    }

    // Reference to logging UI element in the canvas
    public UnityEngine.UI.Text Log;
    
    // List for labels of objects with an anchor placed
    public List<string> labelsOfAnchors = new List<string>();
    
    // List for trackabels already used (anchor is placed on them)
    //Useless, because the trackableDic already contains all the trackables 
    //TODO remove and change references and methods to use trackableDic
    public List<ARTrackable> trackableList = new List<ARTrackable>();
    
    // List for raycast hits is re-used by raycast manager
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    
    // stores the anchor inserted into the environment with the associated boundingbox 
    IDictionary<ARAnchor, BoundingBox> anchorDic = new Dictionary<ARAnchor, BoundingBox>();

    // stores the anchor inserted into the environment withh the associated trackable
    // (feature point, planeWithinPolygon, ... )
    IDictionary<ARTrackable, ARAnchor> trackableDic = new Dictionary<ARTrackable, ARAnchor>();

    // stores the raycasthit with the associated anchor
    // There is no need to also store the trackable (see https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARTrackable.html))
    // because it is stored in the .hit property of the raycastHit
    IDictionary<ARRaycastHit, ARAnchor> raycastHitDic = new Dictionary<ARRaycastHit, ARAnchor>();

    // from PhoneARCamera
    private List<BoundingBox> boxSavedOutlines;
    private float shiftX;
    private float shiftY;
    private float scaleFactor;

    public PhoneARCamera phoneARCamera;
   
    // Cache ARRaycastManager GameObject from ARCoreSession
    public ARRaycastManager m_RaycastManager;
  
    public TextMesh anchorObj_mesh;
    public ARAnchorManager m_AnchorManager;

    // Raycast against planes and feature points
    // Generic planes are not included because they detect the sloppy estimatedPlanes
    const TrackableType trackableTypes = TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon | TrackableType.PlaneWithinBounds;
}
