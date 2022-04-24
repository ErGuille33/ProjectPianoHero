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

    public MidiFile midiFile;

    public string filename = "Assets/Resources/MIDI/test.mid";

    public FileManager fileManager;

    void Start()
    {
        midiTracks = new List<MidiFile.MidiTrack>();
        List<MidiFile.MidiEvent> midiEvents = new List<MidiFile.MidiEvent>();
        List<MidiFile.MidiNote> midiNotes = new List<MidiFile.MidiNote>();

        midiTracks.Add(new MidiFile.MidiTrack("Track 0" + "+ ", "Piano", midiEvents, midiNotes, 0, 0,0));

    }

    public void addMidiNote(MidiFile.MidiEvent midiEvent,int track = 0)
    {
        midiTracks[track].vecEvents.Add(midiEvent);
    }
    

    public void openMidiFile() 
    {
        filename = fileManager.SaveFileExplorer();
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

                    //Patrones de track. Este valor parece funcionar para lo que buscamos
                    n16 = 49152;
                    writer.Write(n16);

                    //Primero leemos la cabecera del track
                    n32 = 1802654797;
                    writer.Write(n32);

                    //Los siguientes 32bits corresponden al tamaño del track.
                    n32 = calculateLenghtChunk();
                    n32 = swap32bit(n32);
                    writer.Write(n32);

                    //El tick
                    writeValue(writer, 0,-1);
                    //El tipo de mensaje
                    writer.Write(Convert.ToByte(0xff));
                    //ntype
                    writer.Write(Convert.ToByte(MidiFile.MetaEventName.MetaTimeSignature));
                    //length
                    writer.Write(Convert.ToByte(4));
                    //Timesignature
                    writer.Write(Convert.ToByte(4));
                    //Entre
                    writer.Write(Convert.ToByte(2));
                    //Clockspertick
                    writer.Write(Convert.ToByte(24));
                    //32per24Clocks
                    writer.Write(Convert.ToByte(8));

                    //El tick
                    writeValue(writer, 0,-1);
                    //El tipo de mensaje
                    writer.Write(Convert.ToByte(0xff));
                    //ntype
                    writer.Write(Convert.ToByte(MidiFile.MetaEventName.MetaSetTempo));
                    //length
                    writer.Write(Convert.ToByte(3));
                    //1
                    writer.Write(Convert.ToByte(7));
                    //2
                    writer.Write(Convert.ToByte(161));
                    //3
                    writer.Write(Convert.ToByte(32));


                    //Name of the track : tempo track
                    writer.Write(Convert.ToByte(0x00));
                    writer.Write(Convert.ToByte(0xff));
                    writer.Write(Convert.ToByte(0x03));
                    writer.Write(Convert.ToByte(0x0b));
                    writer.Write(Convert.ToByte(0x54));
                    writer.Write(Convert.ToByte(0x65));
                    writer.Write(Convert.ToByte(0x6d));
                    writer.Write(Convert.ToByte(0x70));
                    writer.Write(Convert.ToByte(0x6f));
                    writer.Write(Convert.ToByte(0x20));
                    writer.Write(Convert.ToByte(0x54));
                    writer.Write(Convert.ToByte(0x72));
                    writer.Write(Convert.ToByte(0x61));
                    writer.Write(Convert.ToByte(0x63));
                    writer.Write(Convert.ToByte(0x6B));

                    for (int i = 0; i < midiTracks[0].vecEvents.Count; i++)
                    {
                            //El tick
                            writeValue(writer, midiTracks[0].vecEvents[i].nDeltaTick,i);
                        //El tipo de mensaje
                        if (midiTracks[0].vecEvents[i].type == MidiFile.MidiEvent.Type.NoteOff)
                        {
                            writer.Write(Convert.ToByte(128));
                        }
                        else 
                        {
                            writer.Write(Convert.ToByte(144));
                        }
                            
                            //Note
                            writer.Write(Convert.ToByte(midiTracks[0].vecEvents[i].nKey));
                            //Velocity
                            writer.Write(Convert.ToByte(midiTracks[0].vecEvents[i].nVelocity));

                    }
                    //Valores de final de archivo
                    writeValue(writer, midiTracks[0].vecEvents[midiTracks[0].vecEvents.Count - 1].nDeltaTick, 0);
                    writer.Write(Convert.ToByte(0xff));
                    writer.Write(Convert.ToByte(0x2f));
                    writer.Write(Convert.ToByte(0x00));
                    writer.Close();
                }

                stream.Close();
                print("Escrito");
            }
        }
        catch(Exception e)
        {
            print("Algo falló");
        }
        
    }
    //Escribir un valor en el archivo midi utilizando el estandard de compresión de valores en bytes.
    public void writeValue(BinaryWriter writer, uint value, int i)
    {
        byte aux;
        byte aux2;
        byte aux3;
        byte aux4;
        byte aux5;

        //No  deberían haber valores mayores a 34 359 738 367 por lo tanto con estos casos cubrimos cualquier midi (si alguien hace un midi con notas que duran un par de años no podrá jugar al juego)
        if (value > 268435455)
        {
            //Ultimos 8 bits
            aux = Convert.ToByte(value & 0x7F);
            //Cuartos 8 bits
            aux2 = Convert.ToByte((value & 0x3F80) >> 7 | 0x80);
            //Terceros 8 bits
            aux3 = Convert.ToByte((value & 0x1FC000) >> 14 | 0x80);
            //Segundos 8 bits (Feoooooo)
            aux4 = Convert.ToByte((value & 0xFE00000) >> 21 | 0x80);
            //Primeros 8 bits
            aux5 = Convert.ToByte((value & 0x7F0000000) >> 28 | 0x80);

            writer.Write(Convert.ToByte(aux5));
            writer.Write(Convert.ToByte(aux4));
            writer.Write(Convert.ToByte(aux3));
            writer.Write(Convert.ToByte(aux2));
            writer.Write(Convert.ToByte(aux));
        }
         if (value > 2097151)
         {
            //Ultimos 8 bits
            aux = Convert.ToByte(value & 0x7F);
            //Terceros 8 bits
            aux2 = Convert.ToByte((value & 0x3F80) >> 7 | 0x80);
            //Segundos 8 bits
            aux3 = Convert.ToByte((value & 0x1FC000) >> 14 | 0x80);
            //Primeros 8 bits (Feoooooo)
            aux4 = Convert.ToByte((value & 0xFE00000) >> 21 | 0x80);
            writer.Write(Convert.ToByte(aux4));
            writer.Write(Convert.ToByte(aux3));
            writer.Write(Convert.ToByte(aux2));
            writer.Write(Convert.ToByte(aux));
        }
        else if(value > 16383)
        {

            //Ultimos 8 bits
            aux = Convert.ToByte(value & 0x7F);
            //Segundos 8 bits
            aux2 = Convert.ToByte((value & 0x3F80) >> 7 | 0x80);
            //Primeros 8 bits
            aux3 = Convert.ToByte((value & 0x1FC000) >> 14 | 0x80);
            writer.Write(Convert.ToByte(aux3));
            writer.Write(Convert.ToByte(aux2)); 
            writer.Write(Convert.ToByte(aux));
        }
        else if (value > 127)
        {
            //Ultimos 8 bits
            aux = Convert.ToByte(value & 0x7F);
            //Primeros 8 bits
            aux2 = Convert.ToByte(((value & 0x3F80) >> 7) | 0x80);
            writer.Write(Convert.ToByte(aux2));
            writer.Write(Convert.ToByte(aux));
            
        }
        
        else
        {
            aux = Convert.ToByte(value & 0x7F);
            writer.Write(Convert.ToByte(aux));

        }
    }

    //Necesitamos calcular cuál va a ser la extensión en bytes de la pista antes de escribirla, por la que uso este método para calcularla
    public uint calculateLenghtChunk()
    {
        uint firstMessages = 33;
        uint auxCount = 0;

        for (int i = 0; i < midiTracks[0].vecEvents.Count; i++)
        {
            //El tick

            auxCount++;

            if (midiTracks[0].vecEvents[i].nDeltaTick > 127)
            {
                auxCount++;
            }
            if (midiTracks[0].vecEvents[i].nDeltaTick > 16383)
            {
                auxCount++;
            }
            if (midiTracks[0].vecEvents[i].nDeltaTick > 2097151)
            {
                auxCount++;
            }
            if (midiTracks[0].vecEvents[i].nDeltaTick > 268435455)
            {
                auxCount++;
            }

            //El tipo de mensaje
            if (midiTracks[0].vecEvents[i].type == MidiFile.MidiEvent.Type.NoteOff)
            {
                auxCount++;
            }
            else
            {
                auxCount++;
            }

            //Note
            auxCount++;
            //Velocity
            auxCount++;

        }
        //Ultimos valores
        auxCount++;
        if (midiTracks[0].vecEvents[midiTracks[0].vecEvents.Count - 1].nDeltaTick > 127)
        {
            auxCount++;
        }
        if (midiTracks[0].vecEvents[midiTracks[0].vecEvents.Count - 1].nDeltaTick > 16383)
        {
            auxCount++;
        }
        if (midiTracks[0].vecEvents[midiTracks[0].vecEvents.Count - 1].nDeltaTick > 2097151)
        {
            auxCount++;
        }
        if (midiTracks[0].vecEvents[midiTracks[0].vecEvents.Count - 1].nDeltaTick > 268435455)
        {
            auxCount++;
        }

        return auxCount + firstMessages;
    }

    public uint swap32bit(uint n)
    {
        return (((n >> 24) & 0xff) | ((n << 8) & 0xff0000) | ((n >> 8) & 0xff00) | ((n << 24) & 0xff000000));
    }

    public uint swap16bit(uint n)
    {

        return Convert.ToUInt16(((n >> 8) | (n << 8)) & 0xFF);
    }

}
