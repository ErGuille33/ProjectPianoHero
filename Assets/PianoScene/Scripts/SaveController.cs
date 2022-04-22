using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public static class SaveController
{

    public static string filepath = "data.fun";



    public static void SaverData(Data _data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filepath, FileMode.Create);

        Data data = new Data(_data);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Data LoadData()
    {

        if (File.Exists(filepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            Data data = formatter.Deserialize(stream) as Data;

            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Error al abrir el archivo");
            return null;
        }
    }
    

}
