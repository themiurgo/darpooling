using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Communication
{

    public interface IDarPoolingOperations
    {
        Result LoginUser(string username, string password);
    }

    public interface ICommand
    {
        void Execute();
        Result EndExecute(IAsyncResult asyncValue);
    }

    /// <summary>
    /// The command class allows arbitrary commands to be executed.
    /// </summary>
    [DataContract]
    [KnownType(typeof(LoginUserCommand))]
    public abstract class Command : ICommand
    {
        protected int commandID;
        protected IDarPoolingOperations receiver;
        protected AsyncCallback callbackMethod;
        protected Result result;

        public virtual void Execute()
        {
        }

        public virtual Result EndExecute(IAsyncResult asyncValue)
        {
            return result;
        }

        public int CommandID
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
    }

    [DataContract]
    public class LoginUserCommand : Command 
    {
        private string username;
        private string password;
        public delegate Result Login(string x, string y);
        Login login;
        

        public LoginUserCommand(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public override void Execute()
        {
            login = new Login(receiver.LoginUser);
            login.BeginInvoke(username, password, callbackMethod, this);
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            // Obtaining the AsyncResult object
            AsyncResult asyncResult = (AsyncResult)asyncValue;
            // Obtaining the delegate
            Login l = (Login) asyncResult.AsyncDelegate;
            // Obtaining the return value of the invoked method
            result = l.EndInvoke(asyncValue);
            return result;
        }



    }



    public class JoinCommand : Command
    {
        private UserNode node;

        public JoinCommand(UserNode node)
        {
            this.node = node;
        }

        public override void Execute()
        {

        }
    }
    public class UnjoinCommand : Command { }
    public class RegisterUserCommand : Command { }
    
    public class LogoutUserCommand : Command { }
    public class InsertTripCommand : Command { }
    public class SearchTripCommand : Command { }
}