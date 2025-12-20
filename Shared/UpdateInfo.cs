using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared
{
    [DataContract]
    public class AppInfo
    {
        [DataMember(Name = "apps")]
        public List<AppVersionInfo> Apps { get; set; }

        [DataMember(Name = "messages")]
        public List<MessageInfo> Messages { get; set; }
    }

    [DataContract]
    public class AppVersionInfo
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class MessageInfo
    {
        // Add fields as needed, e.g., id, content, type
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
