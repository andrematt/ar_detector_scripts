using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Draw programmatically elements in this panel
public class NewElementScript : MonoBehaviour
{
    public Button buttonNewOkObject;
    private static Canvas newCanvasObject;
    public AnchorCreator newAnchorCreator;
    // Start is called before the first frame update
    void Start()
    {
        //Button b = gameObject.GetComponent<Button>();
        //b.onClick.AddListener(delegate () { onClickOk(); });
        newCanvasObject = GameObject.Find("NewElementCanvas").GetComponent<Canvas>();
        newCanvasObject.enabled = false;
        buttonNewOkObject = GameObject.Find("NewElementOkButton").GetComponent<Button>();
        buttonNewOkObject.onClick.AddListener(delegate () { onClickOkNew(); });
        newAnchorCreator = FindObjectOfType<AnchorCreator>(); //useful to destroy anchor
        setIsOpen(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
     * Create and place the anchor on the hit using the 
     * PlaceAnchor method in AnchorCreator class
     */
    public void onClickOkNew()
    {
        //newAnchorCreator.PlaceAnchor(); No, because the anchor can be destroyed when a rule element is ready
        newCanvasObject.enabled = false;
        setIsOpen(false);
    }


    public static void newElement(BoundingBox outline)
    {

        newCanvasObject.enabled = true;
        string myText = "New rule element:"+ outline.Label;
        Text textElement = GameObject.Find("NewRuleElementText").GetComponent<Text>();
        textElement.text = "";
        textElement.text = myText;
        //textElement = "New rule element: " + outline.Label;
    }


    public static bool getIsOpen()
    {
        return isOpen;
    }

    public static void setIsOpen(bool value)
    {
        isOpen = value;
    }

    public static bool isOpen;
}