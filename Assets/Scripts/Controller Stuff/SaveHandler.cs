using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    string saveFilePath;
    // Start is called before the first frame update
    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/PlayerData.json";
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.S))
            SaveGame();

        if (Input.GetKeyDown(KeyCode.L))
            LoadGame();

        if (Input.GetKeyDown(KeyCode.D))
            DeleteSaveFile();*/
    }

    public void SaveGame()
    {
        //HANDLE ALL OF THE NECESSARY UPDATES TO SAVE THE GAME
        //UPGRADE UPDATES
        for (int i = 0; i < GameControl.SaveData.upgrades.Count; i++)
        {
            GameControl.SaveData.upgrades[i].SetStats(GameControl.PlayerData.upgrades[i]);
        }
        //RESEARCH UPDATES
        for (int i = 0; i < GameControl.SaveData.researchData.Count; i++)
        {
            GameControl.SaveData.researchData[i].SetData(GameControl.PlayerData.researchItems[i]);
        }
        //Print all the currently discovered crowns
        /*for (int i = 0; i < GameControl.SaveData.discoveredCrowns.Count; i++)
        {
            Debug.Log(GameControl.SaveData.discoveredCrowns[i].GetTitle() + " is discovered");
        }*/
        foreach (var shiftReport in GameControl.SaveData.shiftReports)
        {
            Debug.Log("Shift Num: " + shiftReport.GetShiftNum() + " " + shiftReport.GetScorePay());
        }
        string savePlayerData = JsonUtility.ToJson(GameControl.SaveData);
        File.WriteAllText(saveFilePath, savePlayerData);

        Debug.Log("Save file created at: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string loadPlayerData = File.ReadAllText(saveFilePath);
            GameControl.SaveData = JsonUtility.FromJson<SaveData>(loadPlayerData);
            Debug.Log("Load game complete!");
            Debug.Log(loadPlayerData);
        }
        else
            Debug.Log("There are no save files left to load!");
    }

    public bool FileFind()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);

            Debug.Log("Save file deleted");
        }
        else
            Debug.Log("There is no file to delete!");
    }
}
