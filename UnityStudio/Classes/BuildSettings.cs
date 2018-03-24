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
    public class BuildSettings
    {
        public string m_Version;

        public BuildSettings(AssetPreloadData preloadData)
        {
            var sourceFile = preloadData.sourceFile;
            var a_Stream = preloadData.sourceFile.a_Stream;
            a_Stream.Position = preloadData.Offset;

            int levels = a_Stream.ReadInt32();
            for (int l = 0; l < levels; l++) { string level = a_Stream.ReadAlignedString(a_Stream.ReadInt32()); }

            if (sourceFile.version[0] == 5)
            {
                int preloadedPlugins = a_Stream.ReadInt32();
                for (int l = 0; l < preloadedPlugins; l++) { string preloadedPlugin = a_Stream.ReadAlignedString(a_Stream.ReadInt32()); }
            }

            a_Stream.Position += 4; //bool flags
            if (sourceFile.fileGen >= 8) { a_Stream.Position += 4; } //bool flags
            if (sourceFile.fileGen >= 9) { a_Stream.Position += 4; } //bool flags
            if (sourceFile.version[0] == 5 || 
                (sourceFile.version[0] == 4 && (sourceFile.version[1] >= 3 || 
                                                (sourceFile.version[1] == 2 && sourceFile.buildType[0] != "a"))))
                                                { a_Stream.Position += 4; } //bool flags

            m_Version = a_Stream.ReadAlignedString(a_Stream.ReadInt32());
        }
    }
}
