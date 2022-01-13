using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using Google.XR.ARCoreExtensions;


public class SaveSerial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        poseToSave = new Pose();
        //poseList = new List<Pose>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addToSaveList(Pose pose)
    {
        // Questo metodo è diventato inutile
        poseToSave = pose;
        //poseList.Add(pose);
        Debug.Log("SAVED!!!!");
        SaveGame(); //This will be moved in a button
    }
    public SerializablePose getPose()
    {
        return retreivedPose;
    }

    public bool checkRetreivedPose() 
    { 
        if (retreivedPoseCheck) 
        {
            return true;
        }
        return false;
    }
    
    public void LoadGame()
    {
        Debug.Log("debug");
        if (File.Exists(Application.persistentDataPath + "/MySaveData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/MySaveData.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            Vector3 parsedPosition = new Vector3();
            Quaternion parsedRotation = new Quaternion();
            SerializablePose parsedPose = new SerializablePose();
            retreivedPose = data.poseListSaved;
            ScreenLog.Log("X of retreived pose: (In saveSerial)" + retreivedPose.position.x.ToString());
            retreivedPoseCheck = true;
            ScreenLog.Log("Game data loaded!");
        }
        else
        {
            ScreenLog.Log("There is no save data!");
        }
    }

    
    void SaveGame()
    {
        ScreenLog.Log("save game");
        BinaryFormatter bf = new BinaryFormatter();
        ScreenLog.Log(Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/MySaveData.dat");
        SaveData data = new SaveData();
        ScreenLog.Log("Here ok?");
        SerializableVector3 serializablePosition = new SerializableVector3();
        serializablePosition = poseToSave.position;
        SerializableQuaternion serializableRotation = new SerializableQuaternion();
        serializableRotation = poseToSave.rotation;

        SerializablePose serializablePose = new SerializablePose
        {
            position = serializablePosition,
            rotation = serializableRotation
        };
        //data.poseListSaved.Add(serializablePose);
        ScreenLog.Log("Here OK!");
        ScreenLog.Log("X of saving pose: " + serializablePose.position.x.ToString());
        //List<SerializablePose> myData = new List<SerializablePose>();
        //myData.Add(serializablePose);
        //bf.Serialize(file, data);
        data.poseListSaved = serializablePose;
        bf.Serialize(file, data);
        file.Close();
        ScreenLog.Log("Game data saved!");
    }

    public bool retreivedPoseCheck = false;

   public SerializablePose retreivedPose;
  
    public Pose poseToSave;
    //public Pose poseList;

}

[Serializable]
public struct SerializablePose
{
    public SerializableVector3 position;
    public SerializableQuaternion rotation;
}    

[Serializable]
class SaveData
{
   //public List<SerializablePose> poseListSaved = new List<SerializablePose>();
   public SerializablePose poseListSaved = new SerializablePose();
}

