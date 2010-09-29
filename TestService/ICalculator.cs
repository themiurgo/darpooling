using System;
using System.ServiceModel;

namespace OpenNETCF.WCF.Sample
{
    [ServiceContract(Namespace = "http://opennetcf.wcf.sample")]
    public interface ICalculator
    {
        [OperationContract]
        int Add(int a, int b);
    }
}
