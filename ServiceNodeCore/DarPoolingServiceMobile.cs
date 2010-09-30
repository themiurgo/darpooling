using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;
using System.ServiceModel;

namespace ServiceNodeCore
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class DarPoolingServiceMobile : IDarPoolingMobile
    {
        Command c;

        #region IDarPoolingMobile Members

        string IDarPoolingMobile.HandleDarPoolingMobileRequest(Command c)
        {
            string timestamp = DateTime.Now.ToString();
            c.CommandID = Tools.HashString(timestamp);
            return timestamp;
        }

        Result IDarPoolingMobile.GetMobileResult(string requestID)
        {
            Result r = new NullResult();
            r.Comment = "Your request is:" + requestID;
            return r;
        }

        #endregion
    }
}
