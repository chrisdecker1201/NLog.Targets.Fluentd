using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MsgPack;
using MsgPack.Serialization;

namespace NLog.Targets.Fluentd
{
    internal class FluentdEmitter
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly Packer packer;
        private readonly SerializationContext serializationContext;
        private readonly Stream destination;

        public void Emit(DateTime timestamp, string tag, IDictionary<string, object> data)
        {
            long unixTimestamp = timestamp.ToUniversalTime().Subtract(unixEpoch).Ticks / 10000000;
            this.packer.PackArrayHeader(3);
            this.packer.PackString(tag, Encoding.UTF8);
            this.packer.Pack((ulong)unixTimestamp);
            this.packer.Pack(data, serializationContext);
            this.destination.Flush();    // Change to packer.Flush() when packer is upgraded
        }

        public FluentdEmitter(Stream stream)
        {
            this.destination = stream;
            this.packer = Packer.Create(destination);
            var embeddedContext = new SerializationContext(this.packer.CompatibilityOptions);
            embeddedContext.Serializers.Register(new OrdinaryDictionarySerializer(embeddedContext, null));
            this.serializationContext = new SerializationContext(PackerCompatibilityOptions.PackBinaryAsRaw);
            this.serializationContext.Serializers.Register(new OrdinaryDictionarySerializer(this.serializationContext, embeddedContext));
        }
    }
}
