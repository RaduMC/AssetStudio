/*
Copyright (c) 2016 Radu

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

/*
DISCLAIMER
The reposiotory, code and tools provided herein are for educational purposes only.
The information not meant to change or impact the original code, product or service.
Use of this repository, code or tools does not exempt the user from any EULA, ToS or any other legal agreements that have been agreed with other parties.
The user of this repository, code and tools is responsible for his own actions.

Any forks, clones or copies of this repository are the responsability of their respective authors and users.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unity_Studio
{
    class AudioClip
    {
        public string m_Name;
        public int m_Format;
        public int m_Type = -1;
        public bool m_3D;
        public bool m_UseHardware;

        //version 5
        public int m_LoadType;
        public int m_Channels;
        public int m_Frequency;
        public int m_BitsPerSample;
        public float m_Length;
        public bool m_IsTrackerFormat;
        public int m_SubsoundIndex;
        public bool m_PreloadAudioData;
        public bool m_LoadInBackground;
        public bool m_Legacy3D;
        public int m_CompressionFormat = -1;

        public string m_Source;
        public long m_Offset;
        public long m_Size;
        public byte[] m_AudioData;

        public AudioClip(AssetPreloadData preloadData, bool readSwitch)
        {
            var sourceFile = preloadData.sourceFile;
            var a_Stream = preloadData.sourceFile.a_Stream;
            a_Stream.Position = preloadData.Offset;

            if (sourceFile.platform == -2)
            {
                uint m_ObjectHideFlags = a_Stream.ReadUInt32();
                PPtr m_PrefabParentObject = sourceFile.ReadPPtr();
                PPtr m_PrefabInternal = sourceFile.ReadPPtr();
            }

            m_Name = a_Stream.ReadAlignedString(a_Stream.ReadInt32());

            if (sourceFile.version[0] < 5)
            {

                m_Format = a_Stream.ReadInt32(); //channels?
                m_Type = a_Stream.ReadInt32();
                m_3D = a_Stream.ReadBoolean();
                m_UseHardware = a_Stream.ReadBoolean();
                a_Stream.Position += 2; //4 byte alignment

                if (sourceFile.version[0] >= 4 || (sourceFile.version[0] == 3 && sourceFile.version[1] >= 2)) //3.2.0 to 5
                {
                    int m_Stream = a_Stream.ReadInt32();
                    m_Size = a_Stream.ReadInt32();

                    if (m_Stream > 1)
                    {
                        m_Offset = a_Stream.ReadInt32();
                        m_Source = sourceFile.filePath + ".resS";
                    }
                }
                else { m_Size = a_Stream.ReadInt32(); }
            }
            else
            {
                m_LoadType = a_Stream.ReadInt32();//Decompress on load, Compressed in memory, Streaming
                m_Channels = a_Stream.ReadInt32();
                m_Frequency = a_Stream.ReadInt32();
                m_BitsPerSample = a_Stream.ReadInt32();
                m_Length = a_Stream.ReadSingle();
                m_IsTrackerFormat = a_Stream.ReadBoolean();
                a_Stream.Position += 3;
                m_SubsoundIndex = a_Stream.ReadInt32();
                m_PreloadAudioData = a_Stream.ReadBoolean();
                m_LoadInBackground = a_Stream.ReadBoolean();
                m_Legacy3D = a_Stream.ReadBoolean();
                a_Stream.Position += 1;
                m_3D = m_Legacy3D;

                m_Source = a_Stream.ReadAlignedString(a_Stream.ReadInt32());
                //m_Source = Path.GetFileName(m_Source);
                m_Source = Path.Combine(Path.GetDirectoryName(sourceFile.filePath), m_Source.Replace("archive:/",""));
                m_Offset = a_Stream.ReadInt64();
                m_Size = a_Stream.ReadInt64();
                m_CompressionFormat = a_Stream.ReadInt32();
            }
            
            if (readSwitch)
            {
                m_AudioData = new byte[m_Size];

                if (m_Source == null)
                {
                    a_Stream.Read(m_AudioData, 0, (int)m_Size);
                }
                else if (File.Exists(m_Source))
                {
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(m_Source)))
                    {
                        reader.BaseStream.Position = m_Offset;
                        reader.Read(m_AudioData, 0, (int)m_Size);
                        reader.Close();
                    }
                }
            }
            else
            {
                preloadData.InfoText = "Compression format: ";

                switch (m_Type)
                {
                    case 2:
                        preloadData.extension = ".aif";
                        preloadData.InfoText += "AIFF";
                        break;
                    case 13:
                        preloadData.extension = ".mp3";
                        preloadData.InfoText += "MP3";
                        break;
                    case 14:
                        preloadData.extension = ".ogg";
                        preloadData.InfoText += "Ogg Vorbis";
                        break;
                    case 20:
                        preloadData.extension = ".wav";
                        preloadData.InfoText += "WAV";
                        break;
                    case 22: //xbox encoding
                        preloadData.extension = ".wav";
                        preloadData.InfoText += "Xbox360 WAV";
                        break;
                }

                switch (m_CompressionFormat)
                {
                    case 0:
                        preloadData.extension = ".fsb";
                        preloadData.InfoText += "PCM";
                        break;
                    case 1:
                        preloadData.extension = ".fsb";
                        preloadData.InfoText += "Vorbis";
                        break;
                    case 2:
                        preloadData.extension = ".fsb";
                        preloadData.InfoText += "ADPCM";
                        break;
                    case 3:
                        preloadData.extension = ".fsb";
                        preloadData.InfoText += "MP3";//not sure
                        break;
                }

                if (preloadData.extension == "")
                {
                    preloadData.extension = ".AudioClip";
                    preloadData.InfoText += "Unknown";
                }
                preloadData.InfoText += "\n3D: " + m_3D.ToString();

                if (m_Name != "") { preloadData.Text = m_Name; }
                else { preloadData.Text = preloadData.TypeString + " #" + preloadData.uniqueID; }
                preloadData.exportSize = (int)m_Size;
                preloadData.SubItems.AddRange(new string[] { preloadData.TypeString, m_Size.ToString() });
            }
        }
    }
}
