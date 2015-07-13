using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CrowdDesign.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an entity already exists in the repository and shouldn't.
    /// </summary>
    [Serializable]
    public class EntityAlreadyExistsException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public EntityAlreadyExistsException()
        {
        }

        public EntityAlreadyExistsException(string message)
            : base(message)
        {
        }

        public EntityAlreadyExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected EntityAlreadyExistsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
