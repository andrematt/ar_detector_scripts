using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using Google.XR.ARCoreExtensions;

using DG.Tweening;

/*
 * Manages the anchors (points in the real world uses as reference 
 * for the visualizations) and the raycast (activated by the user tap)
 * TODO: Now the reset button also retreive the Cloud Anchors. This is 
 * only for testing: these functionalities have to be separated. 
 */
public class AnchorCreator : MonoBehaviour
{
    public void RemoveAllAnchors()
    {
        //porta: 3,4250, 0,5955, 0,5456
        // finestra: -2,0503, 0,6887, 2,5581
        ScreenLog.Log("REMOVE ALL ANCHORS");
        //string x = m_WorldOrigin.position[0].ToString();
        //ScreenLog.Log("X, " + x);
        ScreenLog.Log("x" + Camera.current.transform.position[0].ToString());
        ScreenLog.Log("y" + Camera.current.transform.position[1].ToString());
        ScreenLog.Log("z" + Camera.current.transform.position[2].ToString());
        //ScreenLog.Log(aRPlaceTrackedImages.getInstantiatedApollo());
        //ScreenLog.Log($"DEBUG: Removing all anchors ({anchorDic.Count})");
        
        foreach (var anchor in anchorDic)
        {
            Destroy(anchor.Key.gameObject);
        }
        s_Hits.Clear();
        anchorDic.Clear();
        trackableList.Clear();
        trackableDic.Clear();
        raycastHitDic.Clear();
        labelsOfAnchors.Clear();
        retreived = false;
    }

    public void placeSavedAnchors()
    {
        m_MainMenuCanvas = GameObject.Find("MainMenuCanvas").GetComponent<MainMenuScript>();
        if (m_MainMenuCanvas.getViewAR() == false)
        {
            return;
        }
        foreach (var myObj in augmentedObjectList)
        {
            ScreenLog.Log("INSTANTIATING AN OBJ");
            //CubePanelManager newPanel = new CubePanelManager();
            instantiatedObject = Instantiate(_anchorPrefab, myObj.distanceFromTrackable, Quaternion.identity);
            instantiatedObject.AddComponent<ARAnchor>(); // just adding the component already creates the anchor!
            instantiatedGameObjects.Add(myObj.name, instantiatedObject); 
            isAnchoredDict[myObj.name] = true;
            instantiatedObject.GetComponent<CubePanelManager>().setInfo(myObj.name, myObj.realName, myObj.referenceRoom);
            instantiatedObject.GetComponent<CubePanelManager>().setPosition(myObj.distanceFromTrackable);
            ScreenLog.Log("APOLLO INSTANTIATED & ANCHORED");
        }
        /*
        ScreenLog.Log(phoneARCamera.shiftX.ToString());
        ScreenLog.Log(phoneARCamera.transform.position[0].ToString());
        Vector3 translatedPosition = new Vector3((float)0.5, (float)0.01, (float)5.99);
            GameObject test = Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
      
        Vector3 translatedPosition2 = new Vector3((float)2.15, (float)0.77, (float)0.11);
            GameObject test2 = Instantiate(_anchorPrefab, translatedPosition2, Quaternion.identity);
        */
        /* OK questo carica e riposiziona
        saveSerial.LoadGame();
        if (saveSerial.retreivedPoseCheck)
        {
            ScreenLog.Log("PLACING A RETREIVED ANCHOR!");

            SerializablePose retreived = saveSerial.getPose();
            ScreenLog.Log("X of retreived pose (In AnchorCreator): " + retreived.position.x.ToString());
            Vector3 translatedPosition = new Vector3(retreived.position.x, retreived.position.y, retreived.position.z);
            GameObject test = Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
            //MakeContentAppearAt(test, translatedPosition);

        }
        else { 
            ScreenLog.Log("NO RETREIVED ANCHORS!");
        }
        */
    }

    public void testLineSuggestions(string objectName)
    {
       

        /*
        var go = new GameObject();
        var lr = go.AddComponent<LineRenderer>();

        //var gun = GameObject.Find("Gun");
        //var projectile = GameObject.Find("Projectile");
        var gun = instantiatedGameObjects[objectName];
        var projectile = instantiatedGameObjects["DOOR"]; //random
        

        lr.SetPosition(0, gun.transform.position);
        lr.SetPosition(1, projectile.transform.position);

        transform.DOMoveX(2, 1).From(true);
        gun.transform.DOMoveX(20, 1).From(true);
        */

        if (objectName == "Window")
        {
            GameObject startPositionObject = instantiatedGameObjects[objectName];
            CubePanelManager startPositionScript = startPositionObject.GetComponent<CubePanelManager>();
            Vector3 startPosition = startPositionScript.getPosition();

            GameObject middlePositionObject = instantiatedGameObjects["Fridge"];
            CubePanelManager middlePositionScript = middlePositionObject.GetComponent<CubePanelManager>();
            Vector3 middlePosition = middlePositionScript.getPosition();

            GameObject endPositionObject = instantiatedGameObjects["Door"];
            CubePanelManager endPositionScript = endPositionObject.GetComponent<CubePanelManager>();
            Vector3 endPosition = endPositionScript.getPosition();
            //noGravityProjectileScript.moveBetween2(startPosition, endPosition);
            noGravityProjectileScript.moveBetween3(startPosition, middlePosition, endPosition);
        }
        else if(objectName == "Fridge")
        {
            GameObject startPositionObject = instantiatedGameObjects[objectName];
            CubePanelManager startPositionScript = startPositionObject.GetComponent<CubePanelManager>();
            Vector3 startPosition = startPositionScript.getPosition();

            GameObject endPositionObject = instantiatedGameObjects["Microwave"];
            CubePanelManager endPositionScript = endPositionObject.GetComponent<CubePanelManager>();
            Vector3 endPosition = endPositionScript.getPosition();
            
            noGravityProjectileScript.moveBetween2(startPosition, endPosition);

        }

    }

    /*
     * initialize the AR objects. 
     * For the moment, the placement uses static coordinates 
     * defined with Vector3(x, y, z). 
     * In the future, the APP will read the saved coordinates and 
     * place objects accordingly. 
     */
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        noGravityProjectileScript = FindObjectOfType<NoGravityProjectileScript>();
        currentCamera = Camera.current;
        
        saveSerial = new SaveSerial();
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        GameObject cameraImage = GameObject.Find("Camera Image");
        phoneARCamera = cameraImage.GetComponent<PhoneARCamera>();
        
        _anchorObject = null;
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>(); 
        aRPlaceTrackedImages = FindObjectOfType<ARPlaceTrackedImages>();

        AugmentedObjectSpatialInfo firstObject = new AugmentedObjectSpatialInfo();
        firstObject.name = "Door";
        firstObject.realName = "door";
        firstObject.trackableName = "APOLLO";
        firstObject.referenceRoom = "kitchen";
        firstObject.distanceFromTrackable = new Vector3(0, 0, (float)3.0);

        augmentedObjectList.Add(firstObject);
        isAnchoredDict.Add(firstObject.name, false);
        
        AugmentedObjectSpatialInfo secondObject = new AugmentedObjectSpatialInfo();
        secondObject.name = "Window";
        secondObject.realName = "window";
        secondObject.trackableName = "APOLLO";
        secondObject.referenceRoom = "kitchen";
        secondObject.distanceFromTrackable = new Vector3((float)-3.0, 0, 0);

        augmentedObjectList.Add(secondObject);
        isAnchoredDict.Add(secondObject.name, false);


        AugmentedObjectSpatialInfo thirdObject = new AugmentedObjectSpatialInfo();
        thirdObject.name = "Microwave";
        thirdObject.realName = "microwave";
        thirdObject.trackableName = "APOLLO";
        thirdObject.referenceRoom = "kitchen";
        thirdObject.distanceFromTrackable = new Vector3((float)-3.0, 0, (float)3.0);

        augmentedObjectList.Add(thirdObject);
        isAnchoredDict.Add(thirdObject.name, false);
        
        AugmentedObjectSpatialInfo fourthObject = new AugmentedObjectSpatialInfo();
        fourthObject.name = "Fridge";
        fourthObject.realName = "fridge";
        fourthObject.trackableName = "APOLLO";
        fourthObject.referenceRoom = "kitchen";
        fourthObject.distanceFromTrackable = new Vector3(0, 0, 0);
        
        augmentedObjectList.Add(fourthObject);
        isAnchoredDict.Add(fourthObject.name, false);

        /*
        if (saveSerial.checkSavedListLenght())
        {
            poseList = saveSerial.getPoseList();
        }
        */
    }


    // Gets or sets the world origin which will be used as the transform parent for network
    // spawned objects.
    public Transform WorldOrigin
    {
        get
        {
            ScreenLog.Log("GET WORLD ORIGIN");
            return m_WorldOrigin;
        }

        set
        {
            ScreenLog.Log("SET WORLD ORIGIN");
            IsOriginPlaced = true;
            m_WorldOrigin = value; //Value is a Transoform, which have position, rotation and scale values
            //ScreenLog.Log("SETTED WORLDORIGIN");
            //ScreenLog.Log(m_WorldOrigin.position[0].ToString());
            //ScreenLog.Log(SessionOrigin.transform.rotation);
            //ScreenLog.Log(SessionOrigin.transform.position);
            //ScreenLog.Log("SETTED SESSIONORIGIN"); //Ecco il problema, SessionOrigin non è inizializzata...
            // Remember to link the Session Origin object to the script in unity!
            ScreenLog.Log(SessionOrigin.transform.position[0].ToString());
            Pose sessionPose = _ToWorldOriginPose(new Pose(SessionOrigin.transform.position,
                SessionOrigin.transform.rotation));
            //ScreenLog.Log("SESSION POSE SETTED");
            SessionOrigin.transform.SetPositionAndRotation(
                sessionPose.position, sessionPose.rotation);
        }
    }

    private Pose _ToWorldOriginPose(Pose pose)
    {
        //quando recupera l'ancora e prova a riposizionarla rimane bloccato 
        // tra toworld... 0 e 1
        //ScreenLog.Log("TOWORLDORIGINPOSE 0");
        if (!IsOriginPlaced)
        {
            ScreenLog.Log("NO ORIGIN PLACED, RETURN!!");
            return pose;
        }

        //ScreenLog.Log("TOWORLDORIGINPOSE 1");
        Matrix4x4 anchorTWorld = Matrix4x4.TRS(
            m_WorldOrigin.position, m_WorldOrigin.rotation, Vector3.one).inverse;
        //ScreenLog.Log("TOWORLDORIGINPOSE 2");
        Quaternion rotation = Quaternion.LookRotation(
            anchorTWorld.GetColumn(2),
            anchorTWorld.GetColumn(1));
        //ScreenLog.Log("TOWORLDORIGINPOSE 3");
        return new Pose(
            anchorTWorld.MultiplyPoint(pose.position),
            pose.rotation * rotation);
    }

    /*
     * here we'll place the plane anchoring code
     * Try first a plane (easier to retreive with a tap)
     */
    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        
        // If we hit a plane, try to "attach" the anchor to the plane
        if (hit.trackable is ARPlane plane)
        {
           lastPlane = plane;
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
     * for now, it works with only 1 object per type
     */
    private bool Pos2AnchorNew(float x, float y, BoundingBox outline, ARRaycastHit hit)
    {
        // GameObject anchorObj = m_RaycastManager.raycastPrefab;
        // TextMesh anchorObj_mesh = anchorObj.GetComponent<TextMesh>();
        anchorObj_mesh.text = $"{outline.Label}: {(int)(outline.Confidence * 100)}%";

        //TextMesh anchorObj = GameObject.Find("New Text").GetComponent<TextMesh>();
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
            //if (!alreadyInAnchorList(outline))
            //{ //There can be duplicate objects: just not on the same trackable!!!
            // Create a new anchor
            var anchor = CreateAnchor(hit);
            if (anchor)
            {
                // save the World Origin
                //m_WorldOrigin = anchor.transform;
                //WorldOrigin = anchor.transform;
                poseList.Add(hit.pose);
                //ScreenLog.Log("X of saved hit pose: " + hit.pose.position.x.ToString());
                ScreenLog.Log("LOCAL POSITION OF THE ANCHOR:");
                ScreenLog.Log(anchor.transform.localPosition[0].ToString() +", " +anchor.transform.localPosition[1].ToString() +", " + anchor.transform.localPosition[2].ToString());
                ScreenLog.Log("POSITION OF THE ANCHOR:");
                ScreenLog.Log(anchor.transform.position[0].ToString() +", " + anchor.transform.position[1].ToString() +", " + anchor.transform.position[2].ToString());
                ScreenLog.Log("LOCAL POSITION OF THE PLANE:");
                ScreenLog.Log(lastPlane.transform.localPosition[0].ToString() +", " + lastPlane.transform.localPosition[1].ToString() +", " + lastPlane.transform.localPosition[2].ToString());
                ScreenLog.Log("POSITION OF THE PLANE:");
                ScreenLog.Log(lastPlane.transform.position[0].ToString() +", " + lastPlane.transform.position[1].ToString() +", " + lastPlane.transform.position[2].ToString());
                saveSerial.addToSaveList(hit.pose);
                // Devo prendere il game object inserito nell'ambiente, da questo il transform, e da questo localPosition: è questo che devo salvare, non la pose
                //saveSerial.addToSaveList(1);
                // save the Cloud Anchor.
                //_cloudAnchor = m_AnchorManager.HostCloudAnchor(anchor, 365);

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
        }
        ScreenLog.Log("Anchor NOT created");
        return false;
        //}
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



        // Request the Cloud Anchor.
    void ResolveCloudAnchor(string cloudAnchorId)
    {
        _cloudAnchor = m_AnchorManager.ResolveCloudAnchorId(cloudAnchorId);
    }       

    // chiamata da ARPlaceTrackedImages quando apollo viene tracciato
    public void anchorObjectsFromImageTracking()
    {
        foreach (var myObj in augmentedObjectList)
        {
            if (aRPlaceTrackedImages.getInstantiatedApollo() && 
                myObj.trackableName == "APOLLO" &&
                !isAnchoredDict[myObj.name])
            {
                ScreenLog.Log("INSTANTIATING APOLLO");
                Transform myTransform = aRPlaceTrackedImages.getApolloTransform();
                Vector3 translatedPosition = new Vector3(
                    myTransform.position[0] + myObj.distanceFromTrackable[0], 
                    myTransform.position[1] + myObj.distanceFromTrackable[1], 
                    myTransform.position[2] + myObj.distanceFromTrackable[2]);
                instantiatedObject = Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
                instantiatedObject.AddComponent<ARAnchor>(); // just adding the component already creates the anchor!
                ScreenLog.Log("APOLLO INSTANTIATED & ANCHORED");
                isAnchoredDict[myObj.name] = true;
                //how to move the child objects?
                //instantiatedObject.transform.GetChild(0).transform.localPosition = instantiatedObject.transform.localPosition;
            }
        }
        return;
        //tutto questo non serve più!!
         // volendo La prima instanziazione potrebbe essere fatta al click del tasto delete, 
         //Così evitiamo che parta prima di aver fatto lo scanning della stanza
        if (aRPlaceTrackedImages.getInstantiatedApollo())
        {
            // IF is the first time that we see that object, instantiated it
            if (!isApolloObjectInstantiated)
            {

                ScreenLog.Log("INSTANTIATING APOLLO");
                Transform myTransform = aRPlaceTrackedImages.getApolloTransform();
                Vector3 translatedPosition = new Vector3(myTransform.position[0], myTransform.position[1], myTransform.position[2] + (float)1);
                instantiatedObject = Instantiate(_anchorPrefab, translatedPosition, Quaternion.identity);
                isApolloObjectInstantiated = true;
                instantiatedObject.AddComponent<ARAnchor>(); // just adding the component already creates the anchor!
                ScreenLog.Log("APOLLO INSTANTIATED & ANCHORED");
            }
            // NOn ne vale la pena, non mi sembra che il posizioanmento sia miglire
            return;
            // Shoot a raycast toward the object position. 
            // If something is identified, anchor the object to it
            if (!isApolloObjectAnchored)
            {
                Transform myTransform = aRPlaceTrackedImages.getApolloTransform();
                Vector3 translatedPosition = new Vector3(myTransform.position[0], myTransform.position[1], myTransform.position[2] + (float)1.5);
                if (m_RaycastManager.Raycast(translatedPosition, s_Hits, trackableTypes)) //last arg is instead of TrackableType.All, because we don't want to capture the features points: difficult to retreive
                {
                    var hit = s_Hits[0];
                    var hitPose = s_Hits[0].pose;
                    // Only returns true if there is at least one hit
                    // Raycast hits are sorted by distance, so the first one will be the closest hit.
                    if (s_Hits.Count > 0)
                    {
                        ScreenLog.Log("APOLLO RAYCAST HIT SOMETHING!!!" + hit.hitType);
                        //ARAnchor anchor;
                        //anchor = instantiatedObject.GetComponent<ARAnchor>();
                        //ScreenLog.Log("GETTING THE ANCHOR COMPONENT");
                        //if (anchor == null)
                        //{
                            var planeManager = GetComponent<ARPlaneManager>();
                            if (planeManager && hit.trackable is ARPlane plane)
                            {
                                ScreenLog.Log("PLACING A PLANE ANCHOR!!!");
                                m_AnchorManager.AttachAnchor(plane, hit.pose);
                            }
                            else
                            {
                                m_AnchorManager.AddAnchor(hit.pose);
                                ScreenLog.Log("PLACING A STANDARD ANCHOR!!!");
                            }
                            isApolloObjectAnchored = true;
                        //}
                    }
                }
            }

        }

    }

    /*
     * if a touch is detected && it hits a trackable: 
     *   check if the trackable is already used
     *     if yes, open the edit element panel & return
     *   check if an object is currenctly detected 
     *     if yes, open the new element panel 
     */
    void Update()
    {
        if (checkReturnConditions()){
            return;
        }
        
        if (Input.touchCount < 1 || (Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return; 
        }
        
        //Ray ray = currentCamera.ScreenPointToRay(Input.GetTouch(0).position);
        Ray ray = Camera.current.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit myHit;
        ScreenLog.Log("Raycasting");
        if (Physics.Raycast(ray, out myHit))
        {
            ScreenLog.Log("Hit something");
            ScreenLog.Log(myHit.collider.gameObject.tag);
            if(myHit.collider.gameObject.tag=="cubeButton1")
            {
                GameObject hittedButton = myHit.collider.gameObject;
                GameObject parentCubeGameObj = hittedButton.transform.parent.gameObject;
                CubePanelManager myManager = (CubePanelManager)parentCubeGameObj.GetComponent("CubePanelManager");
                myManager.setTextMesh(1);
            }
            if(myHit.collider.gameObject.tag=="cubeButton2")
            {
                GameObject hittedButton = myHit.collider.gameObject;
                GameObject parentCubeGameObj = hittedButton.transform.parent.gameObject;
                CubePanelManager myManager = (CubePanelManager)parentCubeGameObj.GetComponent("CubePanelManager");
                myManager.setTextMesh(2);
            }
            if(myHit.collider.gameObject.tag=="cubeButton3")
            {
                GameObject hittedButton = myHit.collider.gameObject;
                GameObject parentCubeGameObj = hittedButton.transform.parent.gameObject;
                CubePanelManager myManager = (CubePanelManager)parentCubeGameObj.GetComponent("CubePanelManager");
                myManager.setTextMesh(3);
            }

        }
        return;
        
        // Get stuff
        boxSavedOutlines = phoneARCamera.boxSavedOutlines;
        shiftX = phoneARCamera.shiftX;
        shiftY = phoneARCamera.shiftY;
        scaleFactor = phoneARCamera.scaleFactor;
        //GameObject camera = GameObject.Find("Camera Image"); ???????
        //PhoneARCamera camera2 = camera.GetComponent<PhoneARCamera>(); ????????

        // Only consider single-finger touches that are beginning
        Touch touch;
        // Perform the raycast, we want all the trackables
        if (m_RaycastManager.Raycast(touch.position, s_Hits)) //last arg is instead of TrackableType.All, because we don't want to capture the features points: difficult to retreive
        //if (m_RaycastManager.Raycast(touch.position, s_Hits, trackableTypes)) //last arg is instead of TrackableType.All, because we don't want to capture the features points: difficult to retreive
        {
            // Only returns true if there is at least one hit
            // Raycast hits are sorted by distance, so the first one will be the closest hit.
            if (s_Hits.Count > 0)
            {
                var hit = s_Hits[0];
                var hitPose = s_Hits[0].pose;
                ScreenLog.Log("RAYCAST HIT SOMETHING!!!" + hit.hitType);
                
                // If something is hitted, before all test if an anchor has already been placed here. 
                //if (alreadyInTrackableDic(hit))
                if (trackableAlreadyUsed(hit.trackable))
                {
                    // In this case open the edit rule element panel and return
                    if (!NewElementScript.getIsOpen() && !EditElementScript.getIsOpen())
                    {
                        //ScreenLog.Log("RAYCAST HIT AN ALREADY USED TRACKABLE && EDIT PANEL NOT OPEN!");
                        //retreive the trackable anchor from the hit
                        ARAnchor myAnchor = returnAnchorFromTrackable(hit.trackable);
                        //retrieve the outline linked to that trackable
                        BoundingBox myBoundingBox = returnOutlineFromAnchor(myAnchor);
                        //ScreenLog.Log("RETREIVED BOUNDINGBOX! ", myBoundingBox.Label);
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
    public MainMenuScript m_MainMenuCanvas;
    
    public NoGravityProjectileScript noGravityProjectileScript;


    public Dictionary<string, GameObject> instantiatedGameObjects = new Dictionary<string, GameObject>();
    public List<AugmentedObjectSpatialInfo> augmentedObjectList = new List<AugmentedObjectSpatialInfo>();

    public string lastImageTargetName = "";
    public Transform lastImageTargetTransform;

    public Dictionary<string, bool> isAnchoredDict = new Dictionary<string, bool>();
    public void setLastImageTargetName(string name)
    {
        lastImageTargetName=name;
    }
    public string getLastImageTargetName()
    {
        return lastImageTargetName;
    }
    public void setLastImageTargetTransform(Transform transform)
    {
        lastImageTargetTransform=transform;
    }
    public Transform getLastImageTargetTransform()
    {
        return lastImageTargetTransform;
    }

    public Camera currentCamera;
    GameObject instantiatedObject;

    ARTrackedImageManager m_TrackedImageManager;
    public ARPlaceTrackedImages aRPlaceTrackedImages;
    // Gets a value indicating whether the Origin of the new World Coordinate System,
    // i.e. the Cloud Anchor was placed.
    public bool IsOriginPlaced
    {
        get;
        private set;
    }
    public SaveSerial saveSerial;

    // The world origin transform for this session.
    // https://docs.unity3d.com/Manual/class-Transform.html
    private Transform m_WorldOrigin = null;

    public bool isApolloObjectInstantiated = false; //TODO REMOVE!!
    public bool isApolloObjectAnchored = false; //TODO REMOVE!!!

        // The active AR Session Origin used in the example.
        public ARSessionOrigin SessionOrigin; //Ma viene settata quando si crea??


    // the transparent skull prefab: needs to be PUBLIC to be seen from unity
    public GameObject _anchorPrefab;
    
    private GameObject _anchorObject;

    private bool retreived = false;
    public ARPlane lastPlane;
    public GameObject attachAnchor;
    public TextMesh new_anchorObj_mesh;
    // hosts ...
    private List<ARCloudAnchor> cloudAnchorList = new List<ARCloudAnchor>();
    private List<Pose> poseList = new List<Pose>();
    
    // hosts the current cloudAnchor that have to be saved
    private ARCloudAnchor _cloudAnchor;

    private ARCloudAnchor __cloudAnchor;

    // Reference to logging UI element in the canvas
    public UnityEngine.UI.Text Log;
    
    // List for labels of objects with an anchor placed
    public List<string> labelsOfAnchors = new List<string>();
    
    // List for trackabels already used (anchor is placed on them)
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
    // Generic planes also includes estimatedPlanes that are not optimal, but is better then rely on feature points that are difficult to tap
    // Using only real planes gives too few anchoring points
    //const TrackableType trackableTypes = TrackableType.FeaturePoint | TrackableType.PlaneWithinPolygon | TrackableType.PlaneWithinBounds;
    const TrackableType trackableTypes = TrackableType.Planes;
    //const TrackableType trackableTypes = TrackableType.FeaturePoint | TrackableType.Planes;

}



public struct AugmentedObjectSpatialInfo
{
    public string name;
    public string realName;
    public string trackableName;
    public string referenceRoom;
    public Vector3 distanceFromTrackable;

    public AugmentedObjectSpatialInfo(string myName, string myRealName, string myTrackableName, string myReferenceRoom, Vector3 myDistanceFromTrackable)
    {
        name = myName;
        realName = myRealName;
        trackableName = myTrackableName;
        referenceRoom = myReferenceRoom;
        distanceFromTrackable = myDistanceFromTrackable;
    }
}
