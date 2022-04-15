using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

//Este script sirve para ir guardando los distintos inputs del teclado, para luego paasarlo a un archivo midi
public class MidiRecorder : MonoBehaviour
{
    List<MidiFile.MidiTrack> midiTracks;

    public string filename = "test.mid";
    // Start is called before the first frame update
    void Start()
    {
        midiTracks = new List<MidiFile.MidiTrack>();
        List<MidiFile.MidiEvent> midiEvents = new List<MidiFile.MidiEvent>();
        List<MidiFile.MidiNote> midiNotes = new List<MidiFile.MidiNote>();

        midiTracks.Add(new MidiFile.MidiTrack("Track 0" + "+ ", "Piano", midiEvents, midiNotes, 0, 0));
    }

    public void addMidiNote(byte nKey, byte nVelocity, System.UInt32 nStartTime, System.UInt32 nDuration,uint nDeltaKick, MidiFile.MidiEvent.Type type  , int channel,int track = 0)
    {
        midiTracks[track].vecNotes.Add(new MidiFile.MidiNote(nKey, nVelocity, nStartTime, nDuration));
        midiTracks[track].vecEvents.Add(new MidiFile.MidiEvent(type,nKey,nVelocity,nDeltaKick));
    }

    public void openMidiFile() 
    {
        //Hay que escribir en big endian, por lo que algunos de los valores fijos los ecribo directamente con los bits invertidos
        uint n32 = 0;
        ushort n16 = 0;
        try
        {
            using (var stream = File.Open(filename, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    //Id cabecera ('MThd')
                    n32 = 1684558925;
                    writer.Write(n32);

                    //Lenght cabecera (6)
                    n32 = 100663296;
                    writer.Write(n32);

                    //Formato cabecera canales (1)
                    n16 = 256;
                    writer.Write(n16);

                    //Formato cabecera numero de tracks (en este caso solo vamos a hacer grabaciones de 1 track)
                    n16 = 256;
                    writer.Write(n16);

                    //Patrones de track ?¿. Este valor parece funcionar para lo que buscamos
                    n16 = 49152;
                    writer.Write(n16);

                    //Primero leemos la cabecera del track
                    n32 = 1802654797;
                    writer.Write(n16);

                    //Los siguientes 32bits corresponden al tamaño del encabezado
                    n32 = 3154116608;
                    writer.Write(n16);

                    for(int i = 0; i < midiTracks[0].vecEvents.Count; i++)
                    {



                    }


                    writer.Close();
                }
                stream.Close();
            }
        }
        catch(Exception e)
        {
            print("Algo falló");
        }
        
    }
    //Escei
    public void writeValue(BinaryWriter writer, uint value)
    {
        uint aux;
        byte byteToWrite;


        if(value > 127)
        {
            if(value > 16383)
            {
                if(value > 2097151)
                {

                }
            }
        }
        else
        {
            byteToWrite = Convert.ToByte(value & 0x7F);

        }
    }

    public UInt32 swap32bit(UInt32 n)
    {
        return (((n >> 24) & 0xff) | ((n << 8) & 0xff0000) | ((n >> 8) & 0xff00) | ((n << 24) & 0xff000000));
    }

    public UInt16 swap16bit(UInt16 n)
    {

        return Convert.ToUInt16(((n >> 8) | (n << 8)) & 0xFF);
    }

}
