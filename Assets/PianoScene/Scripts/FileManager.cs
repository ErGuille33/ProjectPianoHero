using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Clase que sirve para abrir el explorador de archivos para guardar o cargar un archivo midi
public class FileManager : MonoBehaviour
{

    string path;

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
