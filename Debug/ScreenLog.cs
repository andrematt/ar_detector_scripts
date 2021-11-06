using UnityEngine;
using UnityEngine.UI;

// Class used to debug on screen instead of in console 
public class ScreenLog : MonoBehaviour
{
    public Text logText;
    public static ScreenLog Instance { get; private set; }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        logText.text = "start";
    }

    private void _log(string msg)
    {
        if (logText)
        {
            logText.text += msg + "\n";
        }
    }

    public static void Log(string msg)
    {
        if (Instance)
        {
            Instance._log(msg);
        }
        Debug.Log(msg);
    }
}