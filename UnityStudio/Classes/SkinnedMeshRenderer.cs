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
    public class SkinnedMeshRenderer
    {
        public PPtr m_GameObject;
        public bool m_Enabled;
        public byte m_CastShadows;
        public bool m_ReceiveShadows;
        public ushort m_LightmapIndex;
        public ushort m_LightmapIndexDynamic;
        public PPtr[] m_Materials;
        public PPtr m_Mesh;
        public PPtr[] m_Bones;

        public SkinnedMeshRenderer(AssetPreloadData preloadData)
        {
            var sourceFile = preloadData.sourceFile;
            var version = preloadData.sourceFile.version;
            var a_Stream = preloadData.sourceFile.a_Stream;
            a_Stream.Position = preloadData.Offset;

            if (sourceFile.platform == -2)
            {
                uint m_ObjectHideFlags = a_Stream.ReadUInt32();
                PPtr m_PrefabParentObject = sourceFile.ReadPPtr();
                PPtr m_PrefabInternal = sourceFile.ReadPPtr();
            }

            m_GameObject = sourceFile.ReadPPtr();
            if (sourceFile.version[0] < 5)
            {
                m_Enabled = a_Stream.ReadBoolean();
                m_CastShadows = a_Stream.ReadByte();//bool
                m_ReceiveShadows = a_Stream.ReadBoolean();
                m_LightmapIndex = a_Stream.ReadByte();
            }
            else
            {
                m_Enabled = a_Stream.ReadBoolean();
                a_Stream.AlignStream(4);
                m_CastShadows = a_Stream.ReadByte();
                m_ReceiveShadows = a_Stream.ReadBoolean();
                a_Stream.AlignStream(4);

                m_LightmapIndex = a_Stream.ReadUInt16();
                m_LightmapIndexDynamic = a_Stream.ReadUInt16();
            }

            if (version[0] >= 3) { a_Stream.Position += 16; } //m_LightmapTilingOffset vector4d
            if (sourceFile.version[0] >= 5) { a_Stream.Position += 16; } //Vector4f m_LightmapTilingOffsetDynamic

            m_Materials = new PPtr[a_Stream.ReadInt32()];
            for (int m = 0; m < m_Materials.Length; m++)
            {
                m_Materials[m] = sourceFile.ReadPPtr();
            }

            if (version[0] < 3) { a_Stream.Position += 16; } //m_LightmapTilingOffset vector4d
            else
            {
                int m_SubsetIndices_size = a_Stream.ReadInt32();
                a_Stream.Position += m_SubsetIndices_size * 4;
                PPtr m_StaticBatchRoot = sourceFile.ReadPPtr();

                if (version[0] >= 4 || (version[0] == 3 && version[1] >= 5))
                {
                    bool m_UseLightProbes = a_Stream.ReadBoolean();
                    a_Stream.Position += 3; //alignment
                    if (version[0] == 5) { int m_ReflectionProbeUsage = a_Stream.ReadInt32(); }
                    //did I ever check if the anchor is conditioned by the bool?
                    PPtr m_LightProbeAnchor = sourceFile.ReadPPtr();
                }

                if (version[0] >= 5 || (version[0] == 4 && version[1] >= 3))
                {
                    if (version[0] == 4 && version[1] <= 3) { int m_SortingLayer = a_Stream.ReadInt16(); }
                    else { int m_SortingLayer = a_Stream.ReadInt32(); }
                    
                    int m_SortingOrder = a_Stream.ReadInt16();
                    a_Stream.AlignStream(4);
                }
            }

            int m_Quality = a_Stream.ReadInt32();
            bool m_UpdateWhenOffscreen = a_Stream.ReadBoolean();
            bool m_SkinNormals = a_Stream.ReadBoolean(); //3.1.0 and below
            a_Stream.Position += 2;

            if (version[0] == 2 && version[1] < 6)
            {
                //this would be the only error if mainVersion is not read in time for a version 2.x game
                PPtr m_DisableAnimationWhenOffscreen = sourceFile.ReadPPtr();
            }

            m_Mesh = sourceFile.ReadPPtr();

            m_Bones = new PPtr[a_Stream.ReadInt32()];
            for (int b = 0; b < m_Bones.Length; b++)
            {
                m_Bones[b] = sourceFile.ReadPPtr();
            }

            if (version[0] < 3)
            {
                int m_BindPose = a_Stream.ReadInt32();
                a_Stream.Position += m_BindPose * 16 * 4;//Matrix4x4f
            }
            else
            {
                if (version[0] >= 4 || (version[0] == 4 && version[1] >= 3))
                {
                    int m_BlendShapeWeights = a_Stream.ReadInt32();
                    a_Stream.Position += m_BlendShapeWeights * 4; //floats
                }

                if (version[0] >= 4 || (version[0] >= 3 && version[1] >= 5))
                {
                    PPtr m_RootBone = sourceFile.ReadPPtr();
                }

                if (version[0] >= 4 || (version[0] == 3 && version[1] >= 4))
                {
                    //AABB
                    float[] m_Center = new float[] { a_Stream.ReadSingle(), a_Stream.ReadSingle(), a_Stream.ReadSingle() };
                    float[] m_Extent = new float[] { a_Stream.ReadSingle(), a_Stream.ReadSingle(), a_Stream.ReadSingle() };
                    bool m_DirtyAABB = a_Stream.ReadBoolean();
                }
            }
        }
    }
}
