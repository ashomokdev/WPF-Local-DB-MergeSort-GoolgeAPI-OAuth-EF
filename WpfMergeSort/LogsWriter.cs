using System.IO;
using System.Xml;

namespace WpfMergeSort
{
    class LogsWriter
    {
        private static readonly object Locker = new object();
        private static readonly XmlDocument _doc = new XmlDocument();

        internal static void Log(string threadname, string state)
        {          
            lock (Locker)
            {
                if (File.Exists("logs.txt"))
                    _doc.Load("logs.txt");
                else
                {
                    var root = _doc.CreateElement("result");
                    _doc.AppendChild(root);
                }

                if (_doc.DocumentElement != null)
                {
                    var el = (XmlElement)_doc.DocumentElement.AppendChild(_doc.CreateElement("thread"));
                    el.SetAttribute("ThreadID", threadname);
                    el.AppendChild(_doc.CreateElement("State")).InnerText = state;
                }
                _doc.Save("logs.txt");
            }
        }
    }
}
