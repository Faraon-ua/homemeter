using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HomeMeter.Domain.Models
{
    [Serializable, XmlRoot("RequestLogs")]
    public class RequestLogs
    {
        public List<RequestLog> Logs { get; set; }
    }
    [Serializable]
    public class RequestLog
    {
        public DateTime TimeStamp { get; set; }
        public List<HeaderLog> Headers { get; set; }
        public string Body { get; set; }
    }
    [Serializable]
    public class HeaderLog
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
