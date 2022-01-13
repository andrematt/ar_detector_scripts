using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        initializeRuleElementType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Dictionary<string, string> ruleElementType = new Dictionary<string, string>();
    //public static Dictionary<string, Vector> triggerType = new Dictionary<string, string>();
    
    public void initializeRuleElementType()
    {
        ruleElementType.Add("estimote_beacon", "trigger");
        ruleElementType.Add("lamp", "both");
        ruleElementType.Add("window", "both");
        ruleElementType.Add("door", "both");
        ruleElementType.Add("philips_hue_sensor", "trigger");
        ruleElementType.Add("echo_dot", "both");
        ruleElementType.Add("honeywell_smoke_gas", "trigger");
    }


    
   //public static IDictionary<string, string> ruleElementConfiguration = new Dictionary<string, string>();
   //public static IDictionary<string, string> triggerConfiguration = new Dictionary<string, string>();
   //ruleElementConfiguration.Add("estimote_beacon", "");
}
