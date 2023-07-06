using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.Xml;

namespace PLCServer.Interceptor
{
    public static class WcfRestMessageExtension
    {
        private static byte[] ReadRaw(Message message)
        {
            var bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary"); //bodyReader.ReadStartElement();
            return bodyReader.ReadContentAsBase64();
        }

        public static WebContentFormat BodyFormat(this Message message)
        {
            object bodyFormatProperty;
            if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty))
                throw new InvalidOperationException();
            return (bodyFormatProperty as WebBodyFormatMessageProperty).Format;
        }

        public static Message RawBody(this Message message, out byte[] body)
        {
            MemoryStream ms;
            var m = RawBody(message, out ms);
            body = ms.ToArray();
            ms.Dispose();
            return m;
        }

        public static Message RawBody(this Message message, out MemoryStream bodyStream)
        {
            MemoryStream ms = null;
            var bodyFormat = message.BodyFormat();
            XmlDictionaryWriter w = null;
            switch (bodyFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    ms = new MemoryStream();
                    w = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    ms = new MemoryStream();
                    w = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    return BinaryRawBody(message, out bodyStream);
            }
            message.WriteMessage(w);
            w.Flush();

            ms.Position = 0;
            var reader = bodyFormat == WebContentFormat.Json
                             ? JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max)
                             : XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
            var newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            ms.Position = 0;
            bodyStream = ms; return newMessage;
        }

        private static Message BinaryRawBody(Message message, out MemoryStream ms)
        {
            var bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            var body = bodyReader.ReadContentAsBase64();

            var m = new MemoryStream();
            var writer = XmlDictionaryWriter.CreateBinaryWriter(m);
            writer.WriteStartElement("Binary");
            writer.WriteBase64(body, 0, body.Length);
            writer.WriteEndElement();
            writer.Flush();
            m.Position = 0;
            ms = new MemoryStream(m.ToArray());
            writer.Close();
            m.Dispose();

            ms.Position = 0;
            var reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
            var newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            ms.Position = 0;
            return newMessage;
        }

        public static MemoryStream RawBody(this Message message)
        {
            MemoryStream ms = null;
            var bodyFormat = message.BodyFormat();
            XmlDictionaryWriter w = null;
            switch (bodyFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    ms = new MemoryStream();
                    w = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    ms = new MemoryStream();
                    w = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    ms = new MemoryStream(ReadRaw(message));
                    ms.Position = 0;
                    return ms;
            }
            message.WriteMessage(w);
            w.Flush();
            ms.Position = 0;
            ms = new MemoryStream(ms.ToArray());
            ms.Position = 0;
            w.Close();
            return ms;
        }
    }
}