
using UnityEngine;
using UnityEditor;
using SimpleFileBrowser;
using System.Collections;
using System.IO;


//Clase que sirve para abrir el explorador de archivos para guardar o cargar un archivo midi
public class FileManager : MonoBehaviour
{

    string path = "";

    public IEnumerator SaveFileExplorer()
    {

		path = "";

		FileBrowser.SetDefaultFilter(".mid");
		FileBrowser.SetFilters(true, new FileBrowser.Filter("MIDI", ".mid"));

		StartCoroutine(ShowSaveDialogCoroutine());
		while (path == "")
		{
			yield return null;
		}
	}

	public string getPath()
	{
		return path;
	}

    public IEnumerator OpenFileExplorer()
    {
		path = "";

		FileBrowser.SetDefaultFilter(".mid");
		FileBrowser.SetFilters(true, new FileBrowser.Filter("MIDI", ".mid"));
		StartCoroutine(ShowLoadDialogCoroutine());
		while(path == "")
		{
			yield return null;
		}

    }

	IEnumerator ShowSaveDialogCoroutine()
	{
		char[] separator = { '/', '.' };
		string[] auxName = path.Split(separator);

		path = "";

		while (auxName[auxName.Length-1] != "mid" && path != "cancel") {
		

			yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.FilesAndFolders, false, Path.GetFullPath("."), "level.mid", "Save as .mid file", "Save");

			if (FileBrowser.Success)
			{
				for (int i = 0; i < FileBrowser.Result.Length; i++)
					path = FileBrowser.Result[i];

				auxName = path.Split(separator);

				print(path);
			}
			else if (!FileBrowser.Success)
			{
				path = "cancel";
			}
		}
	}

	IEnumerator ShowLoadDialogCoroutine()
	{

		path = "";

		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, Path.GetFullPath(".mid"), ".mid", "Load MIDI file", "Load");

		if (FileBrowser.Success)
		{
			for (int i = 0; i < FileBrowser.Result.Length; i++)
				path = FileBrowser.Result[i];

			print(path);
		}
		else if (!FileBrowser.Success)
		{
			path = "cancel";
		}
	}

}
