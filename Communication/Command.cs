using System;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;


namespace Communication
{

    /// <summary>
    /// Contains methods used by clients to submit their requests.
    /// Client assumes that a proper module of the service provides
    /// an implementation for every method present here.
    /// </summary>
    public interface IDarPoolingOperations
    {
        Result RegisterUser(User newUser);
        Result Join(string username, string password);
        Result Unjoin(string username);
        Result InsertTrip(Trip trip);
        Result SearchTrip(QueryBuilder tripQuery);
    }

    /// <summary>
    /// Interface of Commands, as seen in Distributed Command Pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes a Command.
        /// </summary>
        /// <returns></returns>
        // TODO: Why does it returns a Result object?
        Result Execute();

        /// <summary>
        /// Retrieve the Result of the execution of the Command.
        /// </summary>
        /// <param name="asyncValue"></param>
        /// <returns></returns>
        Result EndExecute(IAsyncResult asyncValue);
    }

    /// <summary>
    /// Abstract class representing arbitrary commands. 
    /// </summary>
    [DataContract]
    [KnownType(typeof(RegisterUserCommand))]
    [KnownType(typeof(JoinCommand))]
    [KnownType(typeof(UnjoinCommand))]
    [KnownType(typeof(InsertTripCommand))]
    [KnownType(typeof(SearchTripCommand))]
    public abstract class Command : ICommand
    {
        [DataMember]
        protected string commandID;
        
        // The concrete executer of the Command.
        protected IDarPoolingOperations receiver;
        
        // All Commands run an asynchronous Execute() by using delegates.
        // The callback method is the method that will be invoked when the
        // Execute() ends.
        protected AsyncCallback callbackMethod;

        // Result of the Command, will be returned by EndExecute().
        protected Result result;

        [DataMember]
        protected DateTime timestamp;

        /// <summary>
        /// Execute lets the Command be executed. After execution, EndExecute
        /// is called, which returns the Result of the Command execution.
        /// </summary>
        /// <returns></returns>
        public abstract Result Execute();
        public abstract Result EndExecute(IAsyncResult asyncValue);

        #region Basic Properties
        public string CommandID
        {
            get { return commandID; }
            set { commandID = value; }
        }

        public IDarPoolingOperations Receiver 
        {
            get { return receiver; }
            set { receiver = value; }
        }

        public AsyncCallback Callback
        {
            set { callbackMethod = value; }
        }
        
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }
        #endregion
    }


    /// <summary>
    /// Registration request of a new user. Includes login request: if
    /// registration is successfull, user is also automatically logged in.
    /// </summary>
    [DataContract]
    public class RegisterUserCommand : Command
    {
        [DataMember]
        private User newUser; // User to be registered

        // This delegate is used to perform an asynchronous invocation
        public delegate Result Register(User u);
        Register register;

        public RegisterUserCommand(User newUser)
        {
            this.newUser = newUser;
        }

        public override Result Execute()
        {
            register = new Register(receiver.RegisterUser);
            register.BeginInvoke(newUser, callbackMethod, this);
            return new NullResult();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            Register r = (Register)asyncResult.AsyncDelegate;
            result = r.EndInvoke(asyncValue);

            return result;
        }

        public User NewUser
        {
            get { return newUser; }
            set { newUser = value; }
        }
    }

    /// <summary>
    /// Log-in request of a user who is already registered.
    /// </summary>
    [DataContract]
    public class JoinCommand : Command 
    {
        // TODO: Is it really necessary to pass a reference to the UserNode?
        private UserNode node;

        // Credentials
        private string username;
        private string passwordHash;

        // The delegate is used to perform an asynchronous invocation
        public delegate Result Login(string x, string y);
        Login login;
        
        public JoinCommand(UserNode node, string username, string passwordHash)
        {
            this.node = node;
            this.username = username;
            this.passwordHash = passwordHash;
        }
        
        public override Result Execute()
        {
            login = new Login(receiver.Join);
            login.BeginInvoke(username, passwordHash, callbackMethod, this);
            // FIXME: why this method always returns a LoginOkResult?
            return new LoginOkResult();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            // Obtain the AsyncResult object
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            // Obtain the delegate
            Login l = (Login) asyncResult.AsyncDelegate;
            // Obtain the return value of the invoked method
            result = l.EndInvoke(asyncValue);
            return result;
        }

        [DataMember]
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

    }

    // Unjoin the client from darpooling network
    [DataContract]
    public class UnjoinCommand : Command
    {
        // Username of the client that sends the unjoin request.
        [DataMember]
        private string username;

        // The delegate is used to perform an asynchronous invocation of the
        // Join DarPoolingOperation
        public delegate Result Logoff(string x);
        Logoff logoff;
       
        public UnjoinCommand(string username)
        {
            this.username = username;
        }

        public override Result Execute()
        {
            logoff = new Logoff(receiver.Unjoin);
            logoff.BeginInvoke(username, callbackMethod, this);
            return new NullResult();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            Logoff l = (Logoff) asyncResult.AsyncDelegate;
            result = l.EndInvoke(asyncValue);

            return result;
        }

        [DataMember]
        public string UserName
        {
            get { return username; }
            set { username = value; }
        }
    }

    // Insert a new trip in the database
    [DataContract]
    public class InsertTripCommand : Command
    {
        [DataMember]
        Trip newTrip;

        public delegate Result InsertNewTrip(Trip newTrip);
        private InsertNewTrip insertNewTrip;

        public InsertTripCommand(Trip trip)
        {
            this.newTrip = trip;
        }

        public override Result Execute()
        {
            insertNewTrip = new InsertNewTrip(receiver.InsertTrip);
            insertNewTrip.BeginInvoke(newTrip, callbackMethod, this);
            return new NullResult();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            InsertNewTrip i = (InsertNewTrip) asyncResult.AsyncDelegate;
            result = i.EndInvoke(asyncValue);
            return result;
        }

        public Trip NewTrip
        {
            get { return newTrip; }
        }

    }

    /// <summary>
    /// SearchTrip request, performed by a logged user, to find trips
    /// with specific criteria.
    /// </summary>
    [DataContract]
    public class SearchTripCommand : Command
    {
        [DataMember]
        private QueryBuilder queryBuilder;

        public delegate Result SearchTrip(QueryBuilder q);
        private SearchTrip search;

        public SearchTripCommand(QueryBuilder query)
        {
            this.queryBuilder = query;
        }

        public override Result Execute()
        {
            search = new SearchTrip(receiver.SearchTrip);
            search.BeginInvoke(queryBuilder, callbackMethod, this);
            return new NullResult();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            SearchTrip s = (SearchTrip)asyncResult.AsyncDelegate;
            result = s.EndInvoke(asyncValue);
            return result;
        }
    }
}