using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Server.Services
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.DataModel;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Services;

    public class ExportLeonFormatService : IExportLeonFormatService
    {
        private XslCompiledTransform m_xsltLeonFormat;
        private XmlSerializer m_serLag;
        private string m_leonFormatXsltFile;
        private string m_exportDirLeon;
        public ExportLeonFormatService()
        {
            m_serLag = new XmlSerializer(typeof(List<Lag>));
            m_leonFormatXsltFile = ConfigurationLoader.GetAppSettingsValue("LeonFormatXsltFile");
            m_exportDirLeon = ConfigurationLoader.GetAppSettingsValue("ExportDirLeonFormat");
            InitXslt();
        }

        private void InitXslt()
        {
            if (File.Exists(m_leonFormatXsltFile))
            {
                this.m_xsltLeonFormat = new XslCompiledTransform(true);
                try
                {
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;
                    this.m_xsltLeonFormat.Load(m_leonFormatXsltFile, settings, null);
                }
                catch (XsltException e)
                {
                    Log.Error(e, "Line={0} pos={1} File={2}", e.LineNumber, e.LinePosition, m_leonFormatXsltFile);
                    throw;
                }
            }
            else
            {
                Log.Info("Could not find file {0}", m_leonFormatXsltFile);
            }
        }

        void IExportLeonFormatService.GenerateLeonFormat(List<Lag> itemsToConvert)
        {
            try
            {

            
            if (itemsToConvert != null)
            {
                    var outputXmlStream = new MemoryStream { Position = 0 };
                    var memStevne = new MemoryStream();
                    XmlTextWriter write = new XmlTextWriter(memStevne, new UTF8Encoding(false));
                    this.m_serLag.Serialize(write, itemsToConvert);
                    write.Flush();
                    memStevne.Position = 0;

                    var debugStrem = new MemoryStream(memStevne.ToArray());
                    debugStrem.Position = 0;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(debugStrem);
                    memStevne.Position = 0;
                    XPathDocument xpathDoc;
                    var enc = new UTF8Encoding(false);
                    var reader = new StreamReader(memStevne, enc, true);

                    // XmlTextReader xmlReader = new XmlTextReader(reader);
                    xpathDoc = new XPathDocument(reader);
                    this.m_xsltLeonFormat.Transform(xpathDoc, null, outputXmlStream);
                    outputXmlStream.Position = 0;
                    XmlDocument docSaver = new XmlDocument();
                    StreamReader readerOut = new StreamReader(outputXmlStream, enc, true);
                    XmlTextReader xmlReaderOut = new XmlTextReader(readerOut);
                    docSaver.Load(xmlReaderOut);
                     string fileexportName = Path.Combine(m_exportDirLeon, "Paameldinger.xml");
                    if (!string.IsNullOrEmpty(fileexportName))
                    {
                        Log.Info("Generating new Start List {0}", fileexportName);
                        var encServer = Encoding.GetEncoding("UTF-8");
                        XmlTextWriter writer = new XmlTextWriter(fileexportName, encServer);
                        writer.Formatting = Formatting.Indented;
                        docSaver.Save(writer);
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error GenerateLeonFormat");
                throw;
            }
        }
    }
}
