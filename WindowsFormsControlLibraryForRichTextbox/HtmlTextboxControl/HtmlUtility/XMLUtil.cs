using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WindowsFormsControlLibraryForRichTextbox.HtmlTextboxControl.HtmlUtility
{
    public class XmlUtil
    {
        /// <summary>
        /// Serialize a instance to xml file using default coding
        /// </summary>
        /// <param name="xmlFileName">the file name of the output xml</param>
        /// <param name="oData">dest instance</param>
        /// <param name="useNullNameSpace">whether use null namespace or not</param>
        /// <returns>when succeed then 0 otherwise throw a exception</returns>
        public static int WriteXml(String xmlFileName, Object oData, bool useNullNameSpace)
        {

            XmlSerializer xs = new XmlSerializer(oData.GetType());
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(xmlFileName, System.Text.Encoding.GetEncoding(0));
                writer.Formatting = System.Xml.Formatting.Indented;
                if (useNullNameSpace)
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xs.Serialize(writer, oData, ns);
                }
                else
                {
                    xs.Serialize(writer, oData);
                }
                writer.Close();
            }
            finally
            {
                if (writer != null) { writer.Close(); }
            }
            return 0;
        }

        /// <summary>
        /// get the xml content of the instance without any xsl information and use empty namespace
        /// </summary>
        /// <param name="oData">dest object</param>
        /// <returns></returns>
        public static String GetXML(Object oData)
        {
            return GetXML(oData, null, true);
        }

        public static String GetShortXML(Object oData, String strType)
        {
            XmlRootAttribute xrAttr = new XmlRootAttribute(strType);
            XmlSerializer xs = new XmlSerializer(oData.GetType(), xrAttr);
            XmlTextWriter writer = null;
            System.IO.MemoryStream msStream = null;
            String strResult = null;
            try
            {
                msStream = new System.IO.MemoryStream();
                writer = new XmlTextWriter(msStream, System.Text.Encoding.GetEncoding(0));
                writer.Formatting = System.Xml.Formatting.None;
                writer.WriteRaw(null);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(writer, oData, ns);
                writer.Close();
                strResult = System.Text.Encoding.Default.GetString(msStream.ToArray());
            }
            finally
            {
                if (msStream != null) { msStream.Close(); }
                if (writer != null) { writer.Close(); }
            }
            return strResult;
        }

        /// <summary>
        /// get the corresponding xml content of the object
        /// </summary>
        /// <param name="oData">dest object</param>
        /// <param name="xslURL">if specified,this represent the url of the xsl,otherwise this parameter is ignored</param>
        /// <param name="useNullNameSpace">specify whether use null namespace for the xml or not</param>
        /// <returns>dest xml</returns>
        public static String GetXML(Object oData, String xslURL, bool useNullNameSpace)
        {

            XmlSerializer xs = new XmlSerializer(oData.GetType());
            XmlTextWriter writer = null;
            System.IO.MemoryStream msStream = null;
            String strResult = null;
            try
            {
                msStream = new System.IO.MemoryStream();
                System.Text.Encoding defaultEnc = System.Text.Encoding.GetEncoding(0);
                writer = new XmlTextWriter(msStream, defaultEnc);
                writer.Formatting = System.Xml.Formatting.Indented;
                if (xslURL != null)
                {
                    writer.WriteRaw("<?xml version=\"1.0\" encoding=\"" + defaultEnc.BodyName + "\" ?>\r\n");
                    writer.WriteRaw(String.Format("<?xml:stylesheet type=\"text/xsl\" href=\"{0}\"?>\r\n", xslURL));
                }
                if (useNullNameSpace)
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xs.Serialize(writer, oData, ns);
                }
                else
                {
                    xs.Serialize(writer, oData);
                }
                writer.Close();
                strResult = System.Text.Encoding.Default.GetString(msStream.ToArray());
            }
            finally
            {
                if (msStream != null) { msStream.Close(); }
                if (writer != null) { writer.Close(); }
            }
            return strResult;
        }

        /// <summary>
        /// deserialize a xml file into a object
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="xmlFilename">source xml path</param>
        /// <returns>instance of the xml</returns>
        public static T ReadXML<T>(String xmlFilename)
        {
            return (T)ReadXML(xmlFilename, typeof(T));
        }


        /// <summary>
        /// deserialize a xml file to a instance
        /// </summary>
        /// <param name="xmlFilename">source xml</param>
        /// <param name="dataType">type of the instance</param>
        /// <returns>return dest instance on succeed ,otherwise throws a exception</returns>
        public static Object ReadXML(String xmlFilename, Type dataType)
        {
            Object objXML = null;
            XmlSerializer xs = new XmlSerializer(dataType);
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(xmlFilename);
                objXML = xs.Deserialize(reader);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
            }

            return objXML;
        }

        /// <summary>
        /// deserialize a XML to an object from a string
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="s">XML text</param>
        /// <returns>dest instance</returns>
        public static Object LoadXML<T>(String s)
        {
            return LoadXML(s, typeof(T));
        }

        /// <summary>
        /// deserialize a string to a instance
        /// </summary>
        /// <param name="s">the content of the xml</param>
        /// <param name="dataType">type of the instance</param>
        /// <returns>return dest instance on succeed ,otherwise throws a exception</returns>
        public static Object LoadXML(String s, Type dataType)
        {
            byte[] bData = System.Text.Encoding.GetEncoding(0).GetBytes(s); ;
            return LoadXML(bData, dataType);
        }

        /// <summary>
        /// deserialize a byte array to a instance
        /// </summary>
        /// <param name="bRawData">the content bytes of the xml</param>
        /// <param name="dataType">type of the instance</param>
        /// <returns>return dest instance on succeed ,otherwise throws a exception</returns>
        public static Object LoadXML(byte[] bRawData, Type dataType)
        {
            Object objXML = null;
            XmlSerializer xs = new XmlSerializer(dataType);
            XmlTextReader reader = null;
            System.IO.MemoryStream msStream = null;
            try
            {
                msStream = new System.IO.MemoryStream();
                msStream.Write(bRawData, 0, bRawData.Length);
                msStream.Seek(0, System.IO.SeekOrigin.Begin);
                reader = new XmlTextReader(msStream);
                objXML = xs.Deserialize(reader);
            }
            finally
            {
                if (msStream != null) { msStream.Close(); }
                if (reader != null) { reader.Close(); }
            }

            return objXML;

        }

        /// <summary>
        /// This methond convert a html file to an xhtml file
        /// </summary>
        /// <param name="strOriginalContent">input html file</param>
        /// <returns>converted xhtml file content from input file</returns>
        public static String HTML2XHTML(String strOriginalContent)
        {
            return HTML2XHTML(strOriginalContent, null);
        }


        /// <summary>
        /// This methond convert a html file to an xhtml file
        /// </summary>
        /// <param name="strOriginalContent">input html file</param>
        /// <param name="strTempPath">Temppath,if this parameter is null,then it refers to the temp path of the system</param>
        /// <returns>converted xhtml file content from input file</returns>
        public static String HTML2XHTML(String strOriginalContent, String strOutputPath)
        {
            if (!string.IsNullOrWhiteSpace(strOriginalContent))
            {
                String strTempPath = strOutputPath != null ? strOutputPath : System.IO.Path.GetTempPath();
                String strFileName = String.Format("{0}tidy.exe", strTempPath);
                //check wether tidy execuble exists
                if (!System.IO.File.Exists(strFileName))
                {
                    SysUtil.WriteFile(strFileName, "");
                }

                //Create process
                System.Diagnostics.ProcessStartInfo psiInfo = new System.Diagnostics.ProcessStartInfo();
                psiInfo.FileName = strFileName;
                psiInfo.CreateNoWindow = true;
                psiInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psiInfo.WorkingDirectory = strTempPath;

                String strMainFileName = System.Guid.NewGuid().ToString("N");
                //Specify the in/out/error file name,which is located in the temporary path
                String strInFileName = String.Format("{0}{1}.in", strTempPath, strMainFileName);
                String strOutFileName = String.Format("{0}{1}.out", strTempPath, strMainFileName);
                String strErrorFileName = String.Format("{0}{1}.log", strTempPath, strMainFileName);
                System.IO.File.Delete(strInFileName);
                //UTF8 Version,and we suppose the original content is encoded though the default encoding of the system
                byte[] baUTF8Data = Encoding.Convert(Encoding.GetEncoding(0), Encoding.UTF8,
                    Encoding.GetEncoding(0).GetBytes(strOriginalContent));
                SysUtil.WriteFile(strInFileName, baUTF8Data);

                //UTF8 Version
                psiInfo.Arguments = String.Format(" -raw -utf8 -asxhtml -i -f {0}.log -o {0}.out {0}.in",
                    strMainFileName);
                System.IO.File.Delete(strOutFileName);
                System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psiInfo);
                proc.WaitForExit();
                System.IO.File.Delete(strInFileName);
                System.IO.File.Delete(strErrorFileName);

                byte[] baResult = SysUtil.ReadFileAsBytes(strOutFileName);
                //We need a head for xhtml processing
                string strContent =
                    Encoding.GetEncoding(0)
                        .GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(0), baResult));
                //strContent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" + strContent;
                System.IO.File.Delete(strOutFileName);
                return strContent;
            }

            return null;
        }
    }
}