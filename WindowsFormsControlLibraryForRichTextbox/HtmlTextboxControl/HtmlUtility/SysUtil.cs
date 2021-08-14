using System;

namespace WindowsFormsControlLibraryForRichTextbox.HtmlTextboxControl.HtmlUtility
{
    /// <summary>
    /// Summary description for SysUtil
    /// </summary>
    public class SysUtil
    {
        /// <summary>
        /// Convert a string from one charset to another charset
        /// </summary>
        /// <param name="strText">source string</param>
        /// <param name="strSrcEncoding">original encoding name</param>
        /// <param name="strDestEncoding">dest encoding name</param>
        /// <returns></returns>
        public static String StringEncodingConvert(String strText, String strSrcEncoding, String strDestEncoding)
        {
            System.Text.Encoding srcEnc = System.Text.Encoding.GetEncoding(strSrcEncoding);
            System.Text.Encoding destEnc = System.Text.Encoding.GetEncoding(strDestEncoding);
            byte[] bData=srcEnc.GetBytes(strText);
            byte[] bResult = System.Text.Encoding.Convert(srcEnc, destEnc, bData);
            return destEnc.GetString(bResult);
        }

        /// <summary>
        /// convert a byte array to string using default encoding
        /// </summary>
        /// <param name="bData">the content of the array</param>
        /// <returns>converted string</returns>
        public static String BytesToString(byte[] bData)
        {
            return System.Text.Encoding.GetEncoding(0).GetString(bData);
        }

        /// <summary>
        /// get the byte array from a string using default encoding
        /// </summary>
        /// <param name="strData">source string</param>
        /// <returns>converted array</returns>
        public static byte[] StringToBytes(String strData)
        {
            return System.Text.Encoding.GetEncoding(0).GetBytes(strData);
        }

        /// <summary>
        /// swap two elements in a array
        /// </summary>
        /// <param name="objArray">dest array</param>
        /// <param name="i">first element</param>
        /// <param name="j">the other element</param>
        public static void SwapArrayElement(Object[] objArray, int i, int j)
        {
            Object t = objArray[i];
            objArray[i] = objArray[j];
            objArray[j] = t;
        }

        /// <summary>
        /// write a byte array to a file
        /// </summary>
        /// <param name="strFilePath">dest file path</param>
        /// <param name="bContent">content to write</param>
        /// <returns>return 0 on succeed,otherwise throw a exception</returns>
        public static int WriteFile(String strFilePath, byte[] bContent)
        {
            System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
            try
            {
                bw.Write(bContent);
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
            return 0;
        }

        /// <summary>
        /// copy a object by content,not by reference
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="srcObject">source objected to be cloned</param>
        /// <returns>the object that cloned from the source object</returns>
        public static T SerializeClone<T>(T srcObject)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bfFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.MemoryStream msStream = new System.IO.MemoryStream();
            T result = default(T);
            try
            {
                bfFormatter.Serialize(msStream, srcObject);
                msStream.Seek(0, System.IO.SeekOrigin.Begin);
                result=(T)bfFormatter.Deserialize(msStream);
            }
            finally
            {
                if (msStream != null) msStream.Close();
            }
            return result;
        }

        /// <summary>
        /// write a string to a file using default encoding
        /// </summary>
        /// <param name="strFilePath">path of the dest file</param>
        /// <param name="strContent">content</param>
        /// <returns>return 0 on succeed,otherwise throw a exception</returns>
        public static int WriteFile(String strFilePath, String strContent)
        {
            System.Text.Encoding encDefault=System.Text.Encoding.GetEncoding(0);
            System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
            try
            {
                bw.Write(encDefault.GetBytes(strContent));
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
            return 0;
        }

        /// <summary>
        /// read all the content from a file as byte array
        /// </summary>
        /// <param name="strFilePath">source file path</param>
        /// <returns>dest byte array on succced</returns>
        public static byte[] ReadFileAsBytes(String strFilePath)
        {
            System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            byte[] baResult = null;
            try
            {
                baResult = new byte[fs.Length];
                br.Read(baResult, 0, baResult.Length);
            }
            finally
            {
                br.Close();
                fs.Close();
            }
            return baResult;
        }
        /// <summary>
        /// read all the content from a file as string in default encoding
        /// </summary>
        /// <param name="strFilePath">source file path</param>
        /// <returns>dest string in default encoding</returns>
        public static String ReadFile(String strFilePath)
        {
            System.Text.Encoding encDefault = System.Text.Encoding.GetEncoding(0);
            System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            String strResult = null;
            try
            {
                byte[] bData = new byte[fs.Length];
                br.Read(bData, 0, bData.Length);
                strResult = encDefault.GetString(bData);
            }
            finally
            {
                br.Close();
                fs.Close();
            }
            return strResult;
        }

        /// <summary>
        /// 将值为ArrayList的Hashtable转换为值为数组类型的Hashtable
        /// </summary>
        /// <param name="htList"></param>
        /// <param name="htArray"></param>
        public static void HashList2HashArray(System.Collections.Hashtable htList, System.Collections.Hashtable htArray)
        {
            System.Collections.IEnumerator it = htList.Keys.GetEnumerator();
            while (it.MoveNext())
            {
                Object obj = it.Current;
                System.Collections.ArrayList alList = (System.Collections.ArrayList)htList[obj];
                if (alList.Count > 0)
                {
                    Object[] objSize = new Object[1];
                    objSize[0] = alList.Count;
                    Type[] types = new Type[1];
                    types[0] = typeof(int);
                    Object[] objArray = (Object[])alList[0].GetType().MakeArrayType().GetConstructor(types).Invoke(objSize);
                    for (int i = 0; i < alList.Count; i++)
                    {
                        objArray[i] = alList[i];
                    }
                    htArray[obj] = objArray;
                }
                else htArray[obj] = null;
            }

        }

        /// <summary>
        /// Convert a ArrayList object to a array
        /// </summary>
        /// <param name="alList">dest ArrayList to convert</param>
        /// <returns>dest array</returns>
        public static Object[] List2Array(System.Collections.ArrayList alList)
        {
            if (alList.Count == 0) return null;
            Object[] objSize = new Object[1];
            objSize[0] = alList.Count;
            Type[] types = new Type[1];
            types[0] = typeof(int);
            Object[] objArray = (Object[])alList[0].GetType().MakeArrayType().GetConstructor(types).Invoke(objSize);
            for (int i = 0; i < alList.Count; i++)
            {
                objArray[i] = alList[i];
            }
            return objArray;
        }

        /// <summary>
        /// Load elements into a generic list from an array
        /// </summary>
        /// <typeparam name="T">type of the element</typeparam>
        /// <param name="list">dest list</param>
        /// <param name="array">source array</param>
        public static void LoadListFromArray<T>(System.Collections.Generic.List<T> list, T[] array)
        {
            list.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }
        }

    };
}