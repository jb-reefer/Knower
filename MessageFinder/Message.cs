using System;
using System.Collections.Generic;

namespace MessageFinder
{
    /// <summary>
    /// Message DTO
    /// </summary>
    /// <example>
    /// {
    ///    "SizeInBytes": 411,
    ///    "DeviceId": "jakesdevice",
    ///    "UserKey": "9232c8bf-d256-432f-b7ac-c22831943750",
    ///    "AssetKey": "02073a43-10fd-41b5-ad73-a75860ce2808",
    ///    "ConnectorName": "DeviceSimulator",
    ///    "Points": [
    ///        {
    ///            "Name": "WindSpeed",
    ///            "Value": -12.490232130740877
    ///        },
    ///        {
    ///            "Name": "AvgWindSpeed",
    ///            "Value": -25.382205679202023
    ///        },
    ///        {
    ///            "Name": "C4Write",
    ///            "Value": -8.2191826961745
    ///        },
    ///        {
    ///            "Name": "PIWebAPIWrite",
    ///            "Value": -12.352794564027709
    ///        }
    ///    ],
    ///    "TimeStamp": "2017-05-15T20:35:55.6625455Z"
    /// }
    /// </example>
    public class Message
    {
        public int SizeInBytes { get; set; }
        public string DeviceId { get; set; }
        public string UserKey { get; set; }
        public string AssetKey { get; set; }
        public string ConnectorName { get; set; }
        public List<Point> Points { get; set; }
        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return $"{DeviceId} Connector: {ConnectorName} Size: {SizeInBytes}  U: {UserKey} A: {AssetKey} {TimeStamp}";
        }
    }

    public class Point
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}
