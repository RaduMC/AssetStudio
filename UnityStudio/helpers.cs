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
using System.Text.RegularExpressions;

namespace Unity_Studio
{
    public class PPtr
    {
        //m_FileID 0 means current file
        public int m_FileID = -1;
        //m_PathID acts more like a hash in some games
        public long m_PathID = 0;
    }

    public static class PPtrHelpers
    {
        public static PPtr ReadPPtr(this AssetsFile sourceFile)
        {
            PPtr result = new PPtr();
            var a_Stream = sourceFile.a_Stream;

            int FileID = a_Stream.ReadInt32();
            if (FileID >= 0 && FileID < sourceFile.sharedAssetsList.Count)
            { result.m_FileID = sourceFile.sharedAssetsList[FileID].Index; }
            
            if (sourceFile.fileGen < 14) { result.m_PathID = a_Stream.ReadInt32(); }
            else { result.m_PathID = a_Stream.ReadInt64(); }

            return result;
        }
           
        public static bool TryGetPD(this List<AssetsFile> assetsfileList, PPtr m_elm, out AssetPreloadData result)
        {
            result = null;

            if (m_elm != null && m_elm.m_FileID >= 0 && m_elm.m_FileID < assetsfileList.Count)
            {
                AssetsFile sourceFile = assetsfileList[m_elm.m_FileID];

                //TryGetValue should be safe because m_PathID is 0 when initialized and PathID values range from 1
                if (sourceFile.preloadTable.TryGetValue(m_elm.m_PathID, out result)) { return true; }
            }

            return false;
        }

        public static bool TryGetTransform(this List<AssetsFile> assetsfileList, PPtr m_elm, out Transform m_Transform)
        {
            m_Transform = null;

            if (m_elm != null && m_elm.m_FileID >= 0 && m_elm.m_FileID < assetsfileList.Count)
            {
                AssetsFile sourceFile = assetsfileList[m_elm.m_FileID];

                if (sourceFile.TransformList.TryGetValue(m_elm.m_PathID, out m_Transform)) { return true; }
            }

            return false;
        }

        public static bool TryGetGameObject(this List<AssetsFile> assetsfileList, PPtr m_elm, out GameObject m_GameObject)
        {
            m_GameObject = null;

            if (m_elm != null && m_elm.m_FileID >= 0 && m_elm.m_FileID < assetsfileList.Count)
            {
                AssetsFile sourceFile = assetsfileList[m_elm.m_FileID];

                if (sourceFile.GameObjectList.TryGetValue(m_elm.m_PathID, out m_GameObject)) { return true; }
            }

            return false;
        }
    }

    class TexEnv
    {
        public string name;
        public PPtr m_Texture;
        public float[] m_Scale;
        public float[] m_Offset;
    }

    class strFloatPair
    {
        public string first;
        public float second;
    }

    class strColorPair
    {
        public string first;
        public float[] second;
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Compares the string against a given pattern.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="pattern">The pattern to match, where "*" means any sequence of characters, and "?" means any single character.</param>
        /// <returns><c>true</c> if the string matches the given pattern; otherwise <c>false</c>.</returns>
        public static bool Like(this string str, string pattern)
        {
            return new Regex(
                "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(str);
        }
    }
}
