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
using System.Linq;
using System.Text;

namespace Unity_Studio
{
    class Material
    {

        public string m_Name;
        public PPtr m_Shader;
        public string[] m_ShaderKeywords;
        public int m_CustomRenderQueue;
        public TexEnv[] m_TexEnvs;
        public strFloatPair[] m_Floats;
        public strColorPair[] m_Colors;
        
        public Material(AssetPreloadData preloadData)
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
            m_Shader = sourceFile.ReadPPtr();

            if (sourceFile.version[0] == 4 && (sourceFile.version[1] >= 2 || (sourceFile.version[1] == 1 && sourceFile.buildType[0] != "a")))
            {
                m_ShaderKeywords = new string[a_Stream.ReadInt32()];
                for (int i = 0; i < m_ShaderKeywords.Length; i++)
                {
                    m_ShaderKeywords[i] = a_Stream.ReadAlignedString(a_Stream.ReadInt32());
                }
            }
            else if (sourceFile.version[0] == 5)
            {
                m_ShaderKeywords = new string[1] { a_Stream.ReadAlignedString(a_Stream.ReadInt32()) };
                uint m_LightmapFlags = a_Stream.ReadUInt32();
            }

            if (sourceFile.version[0] > 4 || (sourceFile.version[0] == 4 && sourceFile.version[1] >= 3)) { m_CustomRenderQueue = a_Stream.ReadInt32(); }

            if (sourceFile.version[0] == 5 && sourceFile.version[1] >= 1)
            {
                string[][] stringTagMap = new string[a_Stream.ReadInt32()][];
                for (int i = 0; i < stringTagMap.Length; i++)
                {
                    stringTagMap[i] = new string[2] { a_Stream.ReadAlignedString(a_Stream.ReadInt32()), a_Stream.ReadAlignedString(a_Stream.ReadInt32()) };
                }
            }

                //m_SavedProperties
                m_TexEnvs = new TexEnv[a_Stream.ReadInt32()];
            for (int i = 0; i < m_TexEnvs.Length; i++)
            {
                TexEnv m_TexEnv = new TexEnv()
                {
                    name = a_Stream.ReadAlignedString(a_Stream.ReadInt32()),
                    m_Texture = sourceFile.ReadPPtr(),
                    m_Scale = new float[2] { a_Stream.ReadSingle(), a_Stream.ReadSingle() },
                    m_Offset = new float[2] { a_Stream.ReadSingle(), a_Stream.ReadSingle() }
                };
                m_TexEnvs[i] = m_TexEnv;
            }

            m_Floats = new strFloatPair[a_Stream.ReadInt32()];
            for (int i = 0; i < m_Floats.Length; i++)
            {
                strFloatPair m_Float = new strFloatPair()
                {
                    first = a_Stream.ReadAlignedString(a_Stream.ReadInt32()),
                    second = a_Stream.ReadSingle()
                };
                m_Floats[i] = m_Float;
            }

            m_Colors = new strColorPair[a_Stream.ReadInt32()];
            for (int i = 0; i < m_Colors.Length; i++)
            {
                strColorPair m_Color = new strColorPair()
                {
                    first = a_Stream.ReadAlignedString(a_Stream.ReadInt32()),
                    second = new float[4] { a_Stream.ReadSingle(), a_Stream.ReadSingle(), a_Stream.ReadSingle(), a_Stream.ReadSingle() }
                };
                m_Colors[i] = m_Color;
            }
        }
    }
}
