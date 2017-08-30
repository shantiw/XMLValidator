using System;
using System.IO;
using System.Text;
using System.Xml;

namespace XData.Data.Xml
{
    public class XmlValidator
    {
        public const string SUCCESS = "Success!";

        public string Validate(string xml, string xsd, string targetNamespace = null)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "xml:The XML is required.";
            if (string.IsNullOrWhiteSpace(xsd)) return "xsd:The XSD is required.";

            StringBuilder sb = new StringBuilder();

            XmlReaderSettings settings;
            byte[] buffer = Encoding.UTF8.GetBytes(xsd);
            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                XmlReader reader = XmlReader.Create(stream);
                try
                {
                    settings = new XmlReaderSettings
                    {
                        ValidationType = ValidationType.Schema
                    };
                    settings.Schemas.Add(targetNamespace, reader);
                    settings.ValidationEventHandler += (sender, args) =>
                    {
                        sb.AppendLine("val:" + args.Message);
                    };
                }
                catch (Exception ex)
                {
                    return "xml:" + ex.Message;
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return "xml:" + ex.Message;
            }
            finally
            {
                stream.Close();
            }

            //
            buffer = Encoding.UTF8.GetBytes(xml);
            stream = new MemoryStream(buffer);
            try
            {
                XmlReader reader = XmlReader.Create(stream, settings);
                try
                {
                    while (reader.Read()) ;
                }
                catch (Exception ex)
                {
                    sb.AppendLine("xsd:" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return "xsd:" + ex.Message;
            }
            finally
            {
                stream.Close();
            }

            return (sb.Length == 0) ? SUCCESS : sb.ToString();
        }


    }
}
