using System.Collections;
/*
 * Parcialmente inspirado en el siguiente código
 * https://github.com/OneLoneCoder/olcPixelGameEngine/blob/master/Videos/OneLoneCoder_PGE_MIDI.cpp

*/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Linq;


public class MidiFile : MonoBehaviour
{
    public struct MidiEvent
    {
        public enum Type
        {
        NoteOff,
		NoteOn,
		Other
        }

        public Type type;
        public byte nKey;
        public byte nVelocity;
        public UInt32 nDeltaTick;

        public MidiEvent(MidiEvent.Type type , byte nKey, byte nVelocity, UInt32 nDeltaTick)
        {
            this.type = type;
            this.nKey = nKey;
            this.nVelocity = nVelocity;
            this.nDeltaTick = nDeltaTick;
       
        }
    };
    
    //Nota
    public struct MidiNote
    {
        public byte nKey;
        public byte nVelocity;
        public UInt32 nStartTime;
        public UInt32 nDuration;
        public MidiNote(byte nKey, byte nVelocity, UInt32 nStartTime, UInt32 nDuration)
        {
                this.nKey = nKey;
                this.nVelocity = nVelocity;
                this.nStartTime = nStartTime;
                this.nDuration = nDuration;
        }

        public static bool operator ==(MidiNote c1, MidiNote c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(MidiNote c1, MidiNote c2)
        {
            return !c1.Equals(c2);
        }


    };
  
    //Pista
    public struct MidiTrack
    {
        public string sName;
        public string sInstrument;
        public List<MidiEvent> vecEvents;
        public List<MidiNote> vecNotes;
        public byte nMaxNote;
        public byte nMinNote;

        public MidiTrack(string sName, string sInstrument, List<MidiEvent> vecEvents, List<MidiNote> vecNotes, byte nMaxNote, byte nMinNote)
        {
            this.sName = sName;
            this.sInstrument = sInstrument;
            this.vecEvents = vecEvents;
            this.vecNotes = vecNotes;
            this.nMaxNote = nMaxNote;
            this.nMinNote = nMinNote;

        }

    };
    

    //Enum con todos los posibles bytes de estado de un mensaje
    public enum EventName: byte
    {
        VoiceNoteOff = 0x80,
        VoiceNoteOn = 0x90,
        VoiceAftertouch = 0xA0,
        VoiceControlChange = 0xB0,
        VoiceProgramChange = 0xC0,
        VoiceChannelPressure = 0xD0,
        VoicePitchBend = 0xE0,
        SystemExclusive = 0xF0,
    };
    //Enum con los posibles bytes de meta información de un mensaje
    public enum MetaEventName : byte
    {
        MetaSequence = 0x00,
        MetaText = 0x01,
        MetaCopyright = 0x02,
        MetaTrackName = 0x03,
        MetaInstrumentName = 0x04,
        MetaLyrics = 0x05,
        MetaMarker = 0x06,
        MetaCuePoint = 0x07,
        MetaChannelPrefix = 0x20,
        MetaEndOfTrack = 0x2F,
        MetaSetTempo = 0x51,
        MetaSMPTEOffset = 0x54,
        MetaTimeSignature = 0x58,
        MetaKeySignature = 0x59,
        MetaSequencerSpecific = 0x7F,
    };

    public List<MidiTrack> midiTracks = new List<MidiTrack>();
    UInt32 m_nTempo = 0;
    UInt32 m_nBPM = 0;

    public List<MidiTrack> getMidiFileTracks()
    {
        return midiTracks;
    }

    public bool parseFile(string file_path)
    {
        UInt32 n32 = 0;
        UInt16 n16 = 0;
        //Abrimos stream
        if (File.Exists(file_path))
        {
            using (var stream = File.Open(file_path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {

                    //Primero leemos la cabecera del archivo
                    n32 = reader.ReadUInt32();
                    //Como los archivos midi son big endian, lo swapeamos para que sea más legible
                    UInt32 fileID = swap32bit(n32);

                    //Los siguientes 32bits no nos interesan, corresponden al tamaño del encabezado
                    n32 = reader.ReadUInt32();
                    UInt32 encabezado = swap32bit(n32);

                    //Los siguientes 16bits no nos interesan, corresponden al formato del archivo
                    n16 = reader.ReadUInt16();
                    UInt16 formato = swap16bit(n16);

                    //Los siguientes 16 bits nos indican el número de pistas midi que contiene el archivo (en nuestro caso, aunque nos interese solamente la del piano, en algunos archivos viene separado)
                    n16 = reader.ReadUInt16();
                    UInt16 nChunks = swap16bit(n16);
                    print("nchunks" + nChunks);

                    //Los siguientes 16 bits no nos interesan
                    n16 = reader.ReadUInt16();
                    UInt16 meh = swap16bit(n16);

                    //Ahora leemos cada piista midi
                    for(UInt16 nChunk = 0; nChunk < nChunks; nChunk++)
                    {
                        if (reader.BaseStream.Position != reader.BaseStream.Length) { 
                        print("PISTA" + nChunk);

                        //Primero leemos la cabecera del archivo
                        n32 = reader.ReadUInt32();
                        UInt32 nfileID = swap32bit(n32);
                        //Los siguientes 32bits no nos interesan, corresponden al tamaño del encabezado
                        n32 = reader.ReadUInt32();
                        UInt32 nEncabezado = swap32bit(n32);
                        }
                        //
                        bool endPista = false;

                        List<MidiEvent> midiEvents = new List<MidiEvent>();
                        List<MidiNote> midiNotes = new List<MidiNote>();

                        midiTracks.Add(new MidiTrack("","",midiEvents, midiNotes, 0,0));

                        byte nPreviousStatus = 0;
                         

                        while (reader.BaseStream.Position != reader.BaseStream.Length &&!endPista)
                        {
                            //Cada evento midi posee un valor de duración relativa y un byte de estado
                            UInt32 deltaTimeStatus = 0;
                            byte nStatus = 0;

                            deltaTimeStatus = readValue(reader);
                            nStatus = reader.ReadByte();
         

                            //Si el primer bit del byte es 0, significa que se está usando el valor del anterior, por lo que debemos de volver atras en el stream
                            if(nStatus < 0x80)
                            {
                                nStatus = nPreviousStatus;
                                //Volvemos un byte atrás en el stream
                                reader.BaseStream.Seek(-1, SeekOrigin.Current);
   
                            }

                            //Ahora deberemos comprobar cuál de los siguientes bytes de estado es y cuál no. Solo importan los primeros 4 bits.
                            if((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceNoteOn))
                            {
                                nPreviousStatus = nStatus;
                                //El canal
                                byte nChannel = (byte)(nStatus & 0x0F);
                                //LA id de la nota
                                byte nNoteID = reader.ReadByte();
                                //La velocidad de la nota
                                byte nNoteVelocity = reader.ReadByte();
                                if (nNoteVelocity == 0)
                                {
                                    midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.NoteOff, nNoteID, nNoteVelocity, deltaTimeStatus));
                                }
                                else
                                {
                                    midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.NoteOn, nNoteID, nNoteVelocity, deltaTimeStatus));
                                    print("noteOn" + nNoteID);
                                }
                         

                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceNoteOff))
                            {
                                nPreviousStatus = nStatus;

                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nNoteID = reader.ReadByte();
                                byte nNoteVelocity = reader.ReadByte();


                                print("noteOFf" + nNoteID);
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.NoteOff,nNoteID,nNoteVelocity,deltaTimeStatus));
                  
                            }

                            //Estos no nos interesan
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceAftertouch))
                            {
                                nPreviousStatus = nStatus;

                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nNoteID = reader.ReadByte();
                                byte nNoteVelocity = reader.ReadByte();
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.Other, nNoteID, nNoteVelocity, deltaTimeStatus));
        
                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceControlChange))
                            {
                                nPreviousStatus = nStatus;

                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nNoteID = reader.ReadByte();
                                byte nNoteVelocity = reader.ReadByte();
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.Other, nNoteID, nNoteVelocity, deltaTimeStatus));
                     

                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceProgramChange))
                            {
                                nPreviousStatus = nStatus;
 
                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nProgramID = reader.ReadByte();
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.Other, 0, 0, deltaTimeStatus));
                 

                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoiceChannelPressure))
                            {
                                nPreviousStatus = nStatus;

                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nChannelPressure = reader.ReadByte();
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.Other, 0, 0, deltaTimeStatus));
                         
                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.VoicePitchBend))
                            {
                                nPreviousStatus = nStatus;

                                byte nChannel = Convert.ToByte(nStatus & 0x0F);
                                byte nLS7B = reader.ReadByte();
                                byte nMS7B = reader.ReadByte();
                                midiTracks[nChunk].vecEvents.Add(new MidiEvent(MidiEvent.Type.Other, 0, 0, deltaTimeStatus));
                           
                            }
                            else if ((nStatus & 0xF0) == Convert.ToByte(MidiFile.EventName.SystemExclusive))
                            {

                                nPreviousStatus = 0;

                                if (nStatus == 0xFF)
                                {
                                    // Meta Message
                                    byte nType = reader.ReadByte();
                                    byte nLength = Convert.ToByte(readValue(reader));

                                    if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaSequence)) { print("Sequence Number: " + reader.ReadByte() + reader.ReadByte()); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaText)) { print("Text: " + reader.ReadChars(nLength)); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaCopyright)) { print("Copyright: " + ReadString(nLength, reader));  }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaTrackName) && midiTracks[nChunk].sName != "") {
                                        midiTracks[nChunk].sName.Replace(midiTracks[nChunk].sName, ReadString(nLength, reader)).ToList();
                                        print("Track Name: " + midiTracks[nChunk].sName);
                                    }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaInstrumentName))
                                    {
                                        midiTracks[nChunk].sInstrument.Replace(midiTracks[nChunk].sInstrument, ReadString(nLength, reader)).ToList();
                                        print("Instrument Name: " + midiTracks[nChunk].sInstrument);
                                    }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaLyrics)) { print("Lyrics: " + ReadString(nLength, reader)); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaMarker)) { print("Marker: " + ReadString(nLength, reader)); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaCuePoint)) { print("Cue: " + ReadString(nLength, reader)); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaChannelPrefix)) { print("Prefix: " + reader.ReadByte()); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaEndOfTrack)) { endPista = true; print("endpista"); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaSetTempo)) 
                                    {
                                        // Tempo is in microseconds per quarter note	
                                        if (m_nTempo == 0)
                                        {
                                            m_nTempo |= Convert.ToUInt32(reader.ReadByte() << 16);
                                            m_nTempo |= Convert.ToUInt32(reader.ReadByte() << 8); ;
                                            m_nTempo |= Convert.ToUInt32(reader.ReadByte() << 0); ;
                                            m_nBPM = (60000000 / m_nTempo);
                                            print( "Tempo: " + m_nTempo + " (" + m_nBPM + "bpm)");
                                        }
                                    }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaSMPTEOffset)) { print( "SMPTE: H:" + reader.ReadByte() + " M:" + reader.ReadByte() + " S:" + reader.ReadByte() + " FR:" + reader.ReadByte() + " FF:" + reader.ReadByte()); }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaTimeSignature)) 
                                    { 
                                        print("Time Signature: " + reader.ReadByte() + "/" + (2 + reader.ReadByte()));
                                        print("ClocksPerTick: " + reader.ReadByte());
                                        print("32per24Clocks: " + reader.ReadByte());

                                    }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaKeySignature))
                                    {
                                        print("Key Signature: " + reader.ReadByte());
                                        print("Minor Key: " + reader.ReadByte());

                                    }
                                    else if (nType == Convert.ToByte(MidiFile.MetaEventName.MetaSequencerSpecific)) { print("Sequencer Specific: " + ReadString(nLength,reader)); }

                                    else
                                    {
                                        print("Urecogniced MetaEvent: " + nType);
                                    }   
                                }

                                if (nStatus == 0xF0)
                                {
                                    // System Exclusive Message Begin
                                    print( "System Exclusive Begin: " + ReadString(readValue(reader),reader) );
                                }

                                if (nStatus == 0xF7)
                                {
                                    // System Exclusive Message Begin
                                    print("System Exclusive end: " + ReadString(readValue(reader), reader));
                                }
                            }

                            else
                            {
                                print("No se reconoce este estado de byte");
                            }

                        }
                    }
                    // Convert Time Events to Notes
                    for (int i  = 0; i < midiTracks.Count; i++)
                    {
                        UInt32 nWallTime = 0;
                        List<MidiNote> notasSiendoProcesadas = new List<MidiNote>();

                        for(int j = 0; j < midiTracks[i].vecEvents.Count; j++)
                        {
                            nWallTime += midiTracks[i].vecEvents[j].nDeltaTick;

                            if(midiTracks[i].vecEvents[j].type == MidiEvent.Type.NoteOn)
                            {
                                //Nueva nota

                                notasSiendoProcesadas.Add(new MidiNote(midiTracks[i].vecEvents[j].nKey, midiTracks[i].vecEvents[j].nVelocity, nWallTime, 0));
                            }

                            if (midiTracks[i].vecEvents[j].type == MidiEvent.Type.NoteOff)
                            {
                                MidiNote note = notasSiendoProcesadas.Find(x => x.nKey == midiTracks[i].vecEvents[j].nKey);
                                if (notasSiendoProcesadas.Count != 0)
                                {
                                    if (note == notasSiendoProcesadas.Last())
                                    {
                                        note.nDuration = nWallTime - note.nStartTime;
                                        midiTracks[i].vecNotes.Add(note);

                                        MidiTrack min = midiTracks[i];
                                        min.nMinNote =
                                            Math.Min(midiTracks[i].nMinNote, note.nKey);

                                        MidiTrack max = midiTracks[i];
                                        max.nMinNote =
                                            Math.Max(midiTracks[i].nMinNote, note.nKey);

                                        midiTracks[i] = min;
                                        midiTracks[i] = max;
                                        notasSiendoProcesadas.Remove(note);
                                    }
                                }
                            }
                        }
                    }
                    reader.Close();
                    print("Closing reader");
                }
               
            }
            
            return true;
        }
        
        else
        {
            print("no se abrio");
            return false;
        }
    }
    //Lee el valor de un mensaje
    public UInt32 readValue(BinaryReader reader)
    {

        //Byte. El primer bit indica la extension en bits del mensaje
        UInt32 nValue ;
        //Usaremos esta variable para leer los siguientes 7 bits del mensaje, que nos indican el valor
        byte nByte = 0;

        //Leemos byte
        nValue = reader.ReadByte();
        //Si es distinto de 0
        if (Convert.ToBoolean(nValue & 0x80))
        {
            //Extraemos los últimos 7 bits
            nValue = nValue & 0x7F;

            do
            {
                nByte = reader.ReadByte();

                //Corremos los 7 bits hacia la izquierda y le añadimos los 7 ´´ultimos bits del próximo byte
                nValue = Convert.ToUInt32( (nValue << 7) | (nByte & 0x7F) );

            } while (Convert.ToBoolean(nByte & 0x80)); //Mietras el byte de control sea 1

        }
        return nValue;
    }

    public UInt32 swap32bit(UInt32 n) {
        return (((n >> 24) & 0xff) | ((n << 8) & 0xff0000) | ((n >> 8) & 0xff00) | ((n << 24) & 0xff000000)); 
    }

    public UInt16 swap16bit(UInt16 n)
    {
        
        return  Convert.ToUInt16(((n >> 8) | (n << 8))&0xFF);
    }
    public string ReadString(UInt32 nLength,BinaryReader reader)
	{
        string s ="";
	    for (UInt32 i = 0; i<nLength; i++) s += reader.ReadByte();
		return s;
    }

    
    public bool writeInFile(string file_path)
    {
        try
        {
            int i = 0;
            StreamWriter file = new StreamWriter(file_path);
            foreach (MidiTrack track in midiTracks)
            {
                file.WriteLine("\n\n---------TRACK "+ i +" -----------\n");
                
                    foreach(MidiEvent eve in track.vecEvents)
                    {
                        file.Write(" - Type: " + eve.type + " Note: " + eve.nKey + " Vel: " + eve.nVelocity + " Tick : " + eve.nDeltaTick);
                        
                    }
                i++;
            }
            file.Close();
            return true;
        }
        catch (FileNotFoundException e)
        {
            print("Nombre de archivo incorrecto. " + e.Source);
            return false;
        }

        
  
    }

private void Awake()
    {
        parseFile("Assets/Resources/MIDI/do4(2).mid");
        writeInFile("Assets/Resources/PruebaMidi.txt");
        
    }



}
