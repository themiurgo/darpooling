using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Communication
{
    [DataContract]
    public class Result
    {
        [DataMember]
        private string comment;

        public Result(string comment)
        {
            this.comment = comment;
        }

        // Properties
        public string Comment
        {
            get { return comment; }
        }

    }
}
