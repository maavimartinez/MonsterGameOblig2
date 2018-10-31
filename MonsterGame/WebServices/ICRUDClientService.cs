using System.Collections.Generic;
using System.ServiceModel;
using Business;

namespace WebServices
{
    [ServiceContract]
    public interface ICRUDClientService
    {
        [OperationContract]
        bool CreateClient(ClientCredentials client);

        [OperationContract]
        bool UpdateClient(ClientCredentials existingClientDto, ClientCredentials newClientDto);

        [OperationContract]
        bool DeleteClient(ClientCredentials client);

        [OperationContract]
        List<ClientCredentials> GetClients();
    }
}
