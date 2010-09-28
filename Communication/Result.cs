using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Communication
{

    /// <summary>
    /// Abstract class representing a result to a command.
    /// </summary>
    [DataContract]
    [KnownType(typeof(NullResult))]
    [KnownType(typeof(ConnectionErrorResult))]
    [KnownType(typeof(RegisterOkResult))]
    [KnownType(typeof(RegisterErrorResult))]
    [KnownType(typeof(LoginOkResult))]
    [KnownType(typeof(LoginErrorResult))]
    [KnownType(typeof(UnjoinConfirmedResult))]
    [KnownType(typeof(InsertErrorResult))]
    [KnownType(typeof(InsertOkResult))]
    [KnownType(typeof(SearchTripResult))]
    [KnownType(typeof(SearchTripError))]
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

    // General purpose results

    /// <summary>
    /// Represents a null result. Used when service cannot return a result
    /// immediately.
    /// </summary>
    [DataContract]
    public class NullResult : Result { }

    [DataContract]
    public class ConnectionErrorResult : Result { }

    /// <summary>
    /// This result is used when working with callbacks is not possible
    /// (i.e. smartphone devices). It simply says to wait for a given (or
    /// arbitrary) amount of time and then try again requesting the resource.
    /// </summary>
    public class WaitAndTryResult : Result
    {
        public int Seconds { get; set; }
    }


    // Possible results in response to RegisterUserCommand

    [DataContract]
    public class RegisterOkResult : Result 
    {
        [DataMember]
        private string registeredUsername;

        public string FinalUsername
        {
            get { return registeredUsername; }
            set { registeredUsername = value; }
        }
    }
    [DataContract]
    public class RegisterErrorResult : Result { }


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
    public class LoginErrorResult : Result { }
    

    // Possible results in response to UnjoinCommand
    
    [DataContract]
    public class UnjoinConfirmedResult : Result { }

    
    // Possible results in response to InsertTrip
    
    [DataContract]
    public class InsertOkResult : Result { }
    [DataContract]
    public class InsertErrorResult : Result { }

    
    // Possible results in response to SearchTrip
    
    [DataContract]
    public class SearchTripResult : Result
    {
        [DataMember]
        private List<Trip> trips;
        [DataMember]
        private string originalQueryID;

        public List<Trip> Trips
        {
            get { return trips; }
        }

        public SearchTripResult(List<Trip> trips)
        {
            this.trips = trips;
        }

        public string OriginalQueryID
        {
            get { return originalQueryID; }
            set { originalQueryID = value; }
        }

    }
    
    [DataContract]
    public class SearchTripError : Result { }
}