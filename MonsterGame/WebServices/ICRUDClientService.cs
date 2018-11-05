using System.Collections.Generic;
using System.ServiceModel;
using Business;
using Entities;
using System.Runtime.Serialization;

namespace WebServices
{
    [ServiceContract]
    public interface ICRUDClientService
    {
        [OperationContract]
        bool CreateClient(ClientCredentials client);

        [OperationContract]
        bool UpdateClient(ClientCredentials old, ClientCredentials newC);

        [OperationContract]
        bool DeleteClient(ClientCredentials client);

        [OperationContract]
        List<ClientCredentials> GetClients();

        [OperationContract]
        LogEntry GetLastLog();

        [OperationContract]
        List<RankingCredentials> GetRanking();

    }
}
