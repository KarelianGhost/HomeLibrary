using System.Xml.Linq;

namespace HomeLibrary.Web.Helpers;

public static class XmlHelper
{
    public static string ExtractHtml(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return "";

        try
        {
            var doc = XElement.Parse(xml);
            return doc.Element("Section")?.Value ?? "";
        }
        catch
        {
            return xml;
        }
    }
}