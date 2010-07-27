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
        [DataMember]
        private User user;

        public Result(string comment)
        {
            this.comment = comment;
        }

        public Result(User user)
        {
            this.user = user;
        }

        // Properties
        public string Comment
        {
            get { return comment; }
        }

        public User User
        {
            get { return this.user; }
        }

    }
}
