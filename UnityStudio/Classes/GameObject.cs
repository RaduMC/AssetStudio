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
using System.Windows.Forms;

namespace Unity_Studio
{
    public class GameObject : TreeNode
    {
        public PPtr m_Transform;
        public PPtr m_Renderer;
        public PPtr m_MeshFilter;
        public PPtr m_SkinnedMeshRenderer;
        public int m_Layer;
        public string m_Name;
        public ushort m_Tag;
        public bool m_IsActive;
        
        public string uniqueID = "0";//this way file and folder TreeNodes will be treated as FBX scene

        public GameObject(AssetPreloadData preloadData)
        {
            if (preloadData != null)
            {
                var sourceFile = preloadData.sourceFile;
                var a_Stream = preloadData.sourceFile.a_Stream;
                a_Stream.Position = preloadData.Offset;

                uniqueID = preloadData.uniqueID;

                if (sourceFile.platform == -2)
                {
                    uint m_ObjectHideFlags = a_Stream.ReadUInt32();
                    PPtr m_PrefabParentObject = sourceFile.ReadPPtr();
                    PPtr m_PrefabInternal = sourceFile.ReadPPtr();
                }

                int m_Component_size = a_Stream.ReadInt32();
                for (int j = 0; j < m_Component_size; j++)
                {
                    int m_Component_type = a_Stream.ReadInt32();

                    switch (m_Component_type)
                    {
                        case 4:
                            m_Transform = sourceFile.ReadPPtr();
                            break;
                        case 23:
                            m_Renderer = sourceFile.ReadPPtr();
                            break;
                        case 33:
                            m_MeshFilter = sourceFile.ReadPPtr();
                            break;
                        case 137:
                            m_SkinnedMeshRenderer = sourceFile.ReadPPtr();
                            break;
                        default:
                            PPtr m_Component = sourceFile.ReadPPtr();
                            break;
                    }
                }

                m_Layer = a_Stream.ReadInt32();
                int namesize = a_Stream.ReadInt32();
                m_Name = a_Stream.ReadAlignedString(namesize);
                if (m_Name == "") { m_Name = "GameObject #" + uniqueID; }
                m_Tag = a_Stream.ReadUInt16();
                m_IsActive = a_Stream.ReadBoolean();
                
                base.Text = m_Name;
                //name should be unique
                base.Name = uniqueID;
            }
        }
    }
}
