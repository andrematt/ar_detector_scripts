using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvas = GameObject.Find("MainMenuCanvas").GetComponent<Canvas>();
        buttonSaveObjectsLocation = GameObject.Find("SaveObjectsLocation").GetComponent<Button>();
        buttonSaveObjectsLocation.onClick.AddListener(delegate () { onClickSaveObjectsLocation(); });


        buttonStartRuleEditor = GameObject.Find("StartRuleEditor").GetComponent<Button>();
        buttonStartRuleEditor.onClick.AddListener(delegate () { onClickViewAR(); });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDetectObjects(bool setValue)
    {
        detectObjects = setValue;
    }
    public void setViewAR(bool setValue)
    {
        viewAR = setValue;
        AnchorCreator anchorCreator = FindObjectOfType<AnchorCreator>(); 
        anchorCreator.placeSavedAnchors();
    }
    public void onClickSaveObjectsLocation()
    {
        mainMenuCanvas.enabled = false;
        setViewAR(false);
        setDetectObjects(true);
    }
    public void onClickViewAR()
    {
        mainMenuCanvas.enabled = false;
        setViewAR(true);
        setDetectObjects(false);
    }

    public bool getDetectObjects()
    {
        return detectObjects;
    }
    public bool getViewAR()
    {
        return viewAR;
    }

    bool detectObjects = false;
    bool viewAR = false;
    public Button buttonSaveObjectsLocation;
    public Button buttonStartRuleEditor;
    private Canvas mainMenuCanvas;
}
