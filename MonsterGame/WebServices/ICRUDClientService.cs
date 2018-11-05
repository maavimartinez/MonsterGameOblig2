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
        bool CreateClient(ClientDTO client);

        [OperationContract]
        bool UpdateClient(ClientDTO old, ClientDTO newC);

        [OperationContract]
        bool DeleteClient(ClientDTO client);

        [OperationContract]
        List<ClientDTO> GetClients();

        [OperationContract]
        LogEntry GetLastLog();

        [OperationContract]
        List<RankingCredentials> GetRanking();

        [OperationContract]
        List<StatisticCredentials> GetStatistics();

    }
}
