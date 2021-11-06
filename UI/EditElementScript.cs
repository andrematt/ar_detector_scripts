using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditElementScript : MonoBehaviour
{
    public Button buttonEditOkObject;
    private static Canvas editCanvasObject;
    // Start is called before the first frame update
    void Start()
    {
        //Button b = gameObject.GetComponent<Button>();
        //b.onClick.AddListener(delegate () { onClickOk(); });

        editCanvasObject = GameObject.Find("EditElementCanvas").GetComponent<Canvas>();
        editCanvasObject.enabled = false;
        buttonEditOkObject = GameObject.Find("EditElementOkButton").GetComponent<Button>();
        buttonEditOkObject.onClick.AddListener(delegate () { onClickOkEdit(); });
        setIsOpen(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void onClickOkEdit()
    {
        editCanvasObject.enabled = false;
        setIsOpen(false);
    }


    public static void editElement(BoundingBox outline)
    {
        editCanvasObject.enabled = true;
        string myText = "Edit rule element:" + outline.Label;
        Text textElement = GameObject.Find("EditRuleElementText").GetComponent<Text>();
        textElement.text = "";
        textElement.text = myText;
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