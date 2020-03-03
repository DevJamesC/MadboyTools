using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleShooterGameManager : MonoBehaviour
{
    public static PuzzleShooterGameManager instance;
    public int levelsCompleted { get; protected set; }
    public int totalLevels = 50;
    [HideInInspector]
    public bool completedLevelValueChanged{get; protected set;}

    string saveDataPath = "/saveData.save";

    public bool exampleWin;
    public bool examplePurge;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            saveDataPath = Application.persistentDataPath + saveDataPath;
            LoadProgress();

        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Update()
    {
       if(completedLevelValueChanged)
        {
            StartCoroutine("ResetChangedValue");
        }

       
       
    }

    public void CompletedLevel(int levelCompleted)
    {
        if (levelCompleted > levelsCompleted)
        {
            ++levelsCompleted;
            completedLevelValueChanged = true;
            
        }
        SaveProgress();
    }
    public void ReturnToLevelSelect()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveProgress()
    {
        WriteSaveToFile(CreateSaveFile());
    }

    public void LoadProgress()
    {
       
        if(File.Exists(saveDataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveDataPath,FileMode.Open);
            PuzzleShooterSaveFile save = (PuzzleShooterSaveFile)bf.Deserialize(file);
            file.Close();

            levelsCompleted = save.levelsCompleted;
        }
        else
        {
            levelsCompleted = 0;
            SaveProgress();
        }
    }

    public void PurgeSaveFile()
    {
        if (File.Exists(saveDataPath))
        {
            File.Delete(saveDataPath);

            levelsCompleted = 0;
            completedLevelValueChanged = true;
            
            SaveProgress();
        }
    }

    private PuzzleShooterSaveFile CreateSaveFile()
    {
        PuzzleShooterSaveFile saveFile = new PuzzleShooterSaveFile(levelsCompleted);

        return saveFile;
    }

    private void WriteSaveToFile(PuzzleShooterSaveFile saveData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveDataPath);
        bf.Serialize(file, saveData);

        file.Close();
    }

    IEnumerator ResetChangedValue()
    {
        yield return new WaitForEndOfFrame();

        completedLevelValueChanged = false;
    }

    
    
}

[System.Serializable]
public class PuzzleShooterSaveFile
{
   public int levelsCompleted;

    public PuzzleShooterSaveFile(int levelsCompleted_)
    {
        levelsCompleted = levelsCompleted_;
    }
}
