using System;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;


namespace Communication
{

    /// <summary>
    /// Interface IDarPoolingOperations states the set of methods that must be implemented
    /// on the service side in order to satisfy the client's requests.
    /// All Commands sent by clients assume that a proper module of the service provides this
    /// implementation, and invoke this interface in their methods.
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
    /// ICommand represents the interface of the distributed command pattern.
    /// It specifies the methods that must be implemented by all Commands
    /// </summary>
    public interface ICommand
    {
        // The 'key' method of the Command pattern
        // TODO: Why does it returns a Result object?
        Result Execute();
        // Retrieve the Result of the execution of the Command.
        Result EndExecute(IAsyncResult asyncValue);
    }

    /// <summary>
    /// The command class allows arbitrary commands to be executed.
    /// </summary>
    [DataContract]
    [KnownType(typeof(JoinCommand))]
    [KnownType(typeof(UnjoinCommand))]
    [KnownType(typeof(InsertTripCommand))]
    public abstract class Command : ICommand
    {
        [DataMember]
        protected string commandID;
        
        // Store the reference of the service module which implements the
        // IDarPoolingOperations interface. 
        protected IDarPoolingOperations receiver;
        
        // All Commands run an asynchronous Execute() by using delegates.
        // The callback method is the method that will be invoked when the
        // Execute() ends.
        protected AsyncCallback callbackMethod;

        // The Result of the execution of the command, which will be retrieved
        // by EndExecute() method
        protected Result result;

        [DataMember]
        protected DateTime timestamp;
        
        // Address (Uri) of the first node that received the command. It is used 
        // only if the command will be forwarded to another service node.
        //[DataMember]
        //protected string rootSender;


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


    // The JoinCommand models a client request of logging into the DarPooling service.
    // Thus, it provides a reference to the client UserNode, username and password.
    [DataContract]
    public class JoinCommand : Command 
    {
        // TODO: Is it really necessary to pass a reference to the UserNode?
        private UserNode node;

        // Credentials to access the DarPooling service
        [DataMember]
        private string username;
        [DataMember]
        private string passwordHash;

        // The delegate is used to perform an asynchronous invocation of the
        // Join DarPoolingOperation
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

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }
    }

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
            // Obtain the AsyncResult object
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            // Obtain the delegate
            Logoff l = (Logoff) asyncResult.AsyncDelegate;
            // Obtain the return value of the invoked method
            result = l.EndInvoke(asyncValue);
            return result;
        }

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }
    }



    public class RegisterUserCommand : Command
    {
        public override Result Execute()
        {
            throw new NotImplementedException();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            throw new NotImplementedException();
        }
    }

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


    public class SearchTripCommand : Command
    {
        public override Result Execute()
        {
            throw new NotImplementedException();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            throw new NotImplementedException();
        }
    }
}