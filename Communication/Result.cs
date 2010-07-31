using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

// Daniele's Version

namespace Communication
{
    public enum ResultState
    { 
        SUCCESS,
        FAILURE
    }


    [DataContract]
    [KnownType(typeof(LoginResult))]
    public abstract class Result
    {
        protected ResultState _state;
        [DataMember]
        protected string _comment;

        public ResultState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }
    }

    [DataContract]
    public class LoginResult : Result
    {

        public LoginResult(string comment)
        {
            Console.WriteLine("Creating the result...");
            _comment = comment;

        }
        
    }

}
