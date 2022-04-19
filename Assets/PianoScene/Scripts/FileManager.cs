using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FileManager : MonoBehaviour
{

    string path;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string SaveFileExplorer()
    {
        path = EditorUtility.SaveFilePanel("Save Level File as MIDI ", "", "test" + ".mid", "mid");
        return path;
    }

    public string OpenFileExplorer()
    {
        path = EditorUtility.OpenFilePanel("Open Level File as MIDI ", "", "mid");
        return path;
    }
}
