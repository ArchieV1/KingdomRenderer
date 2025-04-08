using System;
using System.Runtime.Serialization;

namespace KingdomRenderer.Shared.ArchieV1.KCModdingFrameworkAPI.Exceptions
{
    public class PlacementFailedException : Exception
    {
        public PlacementFailedException()
        {
        }

        public PlacementFailedException(string message) : base(message)
        {
        }

        public PlacementFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PlacementFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
