using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Communication
{

    public interface IDarPoolingOperations
    {
        void LoginUser(string username, string password);
    }

    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// The command class allows arbitrary commands to be executed.
    /// </summary>
    [DataContract]
    [KnownType(typeof(LoginUserCommand))]
    public abstract class Command : ICommand
    {
        protected int commandID;
        protected IDarPoolingOperations _receiver;



        public virtual void Execute()
        {
        }

        public int CommandID
        {
            get { return commandID; }
            set { commandID = value; }
        
        }

        public IDarPoolingOperations Receiver 
        {
            get { return _receiver; }
            set { _receiver = value; }
        }
    }

    [DataContract]
    public class LoginUserCommand : Command 
    {
        private string _username;
        private string _password;
        public delegate void Login(string x, string y);
        Login login;

        public LoginUserCommand(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public override void Execute()
        {
            login = new Login(_receiver.LoginUser);
            login.BeginInvoke(_username, _password, null, null);
            //_receiver.LoginUser(_username, _password);
        }
    }



    public class JoinCommand : Command
    {
        private UserNode node;

        public JoinCommand(UserNode node)
        {
            this.node = node;
        }

        public void Execute()
        {

        }
    }
    public class UnjoinCommand : Command { }
    public class RegisterUserCommand : Command { }
    
    public class LogoutUserCommand : Command { }
    public class InsertTripCommand : Command { }
    public class SearchTripCommand : Command { }
}