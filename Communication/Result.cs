using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Communication
{
    [DataContract]
    [KnownType(typeof(LoginOkResult))]
    public abstract class Result
    {
        [DataMember]
        protected string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
    }

    [DataContract]
    public class LoginOkResult : Result { }

    /// <summary>
    /// A result that does nothing. Used for debugging/testing.
    /// </summary>
    [DataContract]
    public class NullResult : Result
    {
        public NullResult() { }
    }
}