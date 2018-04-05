using Prem.PTC;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Titan.Xml
{
    public class XmlFile
    {
        public static void SaveToFile(object content, string folderPath, string name = null)
        {
            try
            {
                var xmlContent = ObjectToXml(content);

                var filePath = string.Format("~/{0}/", folderPath);
                var fileName = name ?? string.Format("XML {0}.xml", AppSettings.ServerTime.ToString("yyyy_MM_dd_HH_mm"));

                var outputXML = new CustomFile(xmlContent);
                outputXML.SaveOnClient(fileName, filePath);
                outputXML.Delete();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
        }

        public static string ObjectToXml<T>(T objectToSerialise)
        {
            StringWriter Output = new StringWriter(new StringBuilder());
            XmlSerializer xs = new XmlSerializer(objectToSerialise.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            xs.Serialize(Output, objectToSerialise, ns);
            return Output.ToString();
        }
    }
}