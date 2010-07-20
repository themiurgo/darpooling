﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{

    [ServiceContract]
    public interface IDarPooling
    {
        [OperationContract]
        void SendCommand(Command command);

        [OperationContract]
        Result GetResult();

        [OperationContract]
        SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers);

        [OperationContract]
        string SayHello();

    }

}
