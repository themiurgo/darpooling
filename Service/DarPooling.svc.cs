using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Communication;

namespace Service
{
    // NOTE: If you change the class name "DarPooling" here, you must also update the reference to "DarPooling" in Web.config.
    public class DarPooling : IDarPooling
    {
        public SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers)
        {
       
            List<SimpleUser> result = new List<SimpleUser>();
            foreach (SimpleUser su in inputUsers)
            {
                if (su.userId < 10)
                {
                    result.Add(su);
                }
            
            }

            return result.ToArray();
        }
    }
}
