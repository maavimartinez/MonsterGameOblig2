using System.Collections.Generic;
using System.ServiceModel;
using Business;
using Entities;
using System.Net.Sockets;

using System.Runtime.Serialization;

namespace WebServices
{
    [ServiceContract]
    public interface ICRUDClientService
    {
        [OperationContract]
        WcfCode CreateClient(ClientDTO client);

        [OperationContract]
        WcfCode UpdateClient(ClientDTO old, ClientDTO newC);

        [OperationContract]
        WcfCode DeleteClient(ClientDTO client);

        [OperationContract]
        List<ClientDTO> GetClients();

        [OperationContract]
        LogEntry GetLastLog();

        [OperationContract]
        List<RankingDTO> GetRanking();

        [OperationContract]
        List<StatisticDTO> GetStatistics();

    }
}
