using System;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Serialization;

namespace ComprobantesElectronicos.Utils
{
    public class XmlGenerator
    {
        public static string ConvertirClaseAXml<T>(T objeto)
        {
            if (objeto == null) return string.Empty;

            // Configuración para omitir la declaración XML <?xml...?> si se desea un string limpio
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true, // Formatea el XML con sangrías
                OmitXmlDeclaration = false, // Cambiar a true si no quieres la cabecera XML
                Encoding = System.Text.Encoding.UTF8
            };

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            var sw = new Utf8StringWriter();
            using (var writer = XmlWriter.Create(sw, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                serializer.Serialize(writer, objeto, namespaces);
            }

            string xmlString = sw.ToString();//.Replace("factura id=\"comprobante\"", "factura");
            return xmlString;
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
