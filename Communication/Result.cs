using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Communication
{
    [DataContract]
    [KnownType(typeof(NullResult))]
    [KnownType(typeof(WaitAndTryResult))]
    [KnownType(typeof(LoginOkResult))]
    [KnownType(typeof(LoginInvalidResult))]
    [KnownType(typeof(LoginErrorResult))]
    [KnownType(typeof(RegisterOkResult))]
    [KnownType(typeof(RegisterErrorResult))]
    [KnownType(typeof(ConnectionErrorResult))]
    public abstract class Result
    {
        [DataMember]
        protected string comment;
        //protected int resultID;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        /*
        public int ResultID
        {
            get { return resultID; }
            set { resultID = value; }
        }*/

    }

    /// <summary>
    /// A result that does nothing. Used for debugging/testing.
    /// </summary>
    [DataContract]
    public class NullResult : Result { }

    [DataContract]
    public class ConnectionErrorResult : Result { }


    // Possible results in response to JoinCommand
    [DataContract]
    public class LoginOkResult : Result 
    {
        [DataMember]
        string authorizedUsername;

        public string AuthorizedUsername
        {
            get { return authorizedUsername; }
            set { authorizedUsername = value; }
        }
    }

    [DataContract]
    public class LoginInvalidResult : Result { }

    [DataContract]
    public class LoginErrorResult : Result { }
    

    // Possible results in response to UnjoinCommand

    public class UnjoinConfirmed : Result { }


    // Possible results in response to RegisterUserCommand

    public class RegisterOkResult : Result { }

    public class RegisterErrorResult : Result { }

    
    // Possible results in response to InsertTrip

    public class InsertOkResult : Result { }

    public class InsertErrorResult : Result { }

    
    // Possible results in response to SearchTrip

    public class SearchTripResult : Result
    {
        public List<Trip> Trips;
    }

    public class SearchTripError : Result { }


    /// <summary>
    /// This result is used when working with callbacks is not possible
    /// (i.e. smartphone devices). It simply says to wait for a given (or
    /// arbitrary) amount of time and then try again requesting the resource.
    /// </summary>
    public class WaitAndTryResult : Result
    {
        public int Seconds { get; set; }
    }
}