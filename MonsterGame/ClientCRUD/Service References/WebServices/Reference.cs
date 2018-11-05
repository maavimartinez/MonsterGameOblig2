﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Business;
using Entities;
using System.Collections.Generic;

namespace CRUDClient.WebServices {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WebServices.ICRUDClientService")]
    public interface ICRUDClientService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/CreateClient", ReplyAction="http://tempuri.org/ICRUDClientService/CreateClientResponse")]
        bool CreateClient(Business.ClientDTO client);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/CreateClient", ReplyAction="http://tempuri.org/ICRUDClientService/CreateClientResponse")]
        System.Threading.Tasks.Task<bool> CreateClientAsync(Business.ClientDTO client);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/UpdateClient", ReplyAction="http://tempuri.org/ICRUDClientService/UpdateClientResponse")]
        bool UpdateClient(Business.ClientDTO old, Business.ClientDTO newC);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/UpdateClient", ReplyAction="http://tempuri.org/ICRUDClientService/UpdateClientResponse")]
        System.Threading.Tasks.Task<bool> UpdateClientAsync(Business.ClientDTO old, Business.ClientDTO newC);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/DeleteClient", ReplyAction="http://tempuri.org/ICRUDClientService/DeleteClientResponse")]
        bool DeleteClient(Business.ClientDTO client);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/DeleteClient", ReplyAction="http://tempuri.org/ICRUDClientService/DeleteClientResponse")]
        System.Threading.Tasks.Task<bool> DeleteClientAsync(Business.ClientDTO client);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetLastLog", ReplyAction = "http://tempuri.org/ICRUDClientService/GetLastLogResponse")]
        LogEntry GetLastLog();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetLastLog", ReplyAction = "http://tempuri.org/ICRUDClientService/GetLastLogResponse")]
        System.Threading.Tasks.Task<LogEntry> GetLastLogAsync();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetRanking", ReplyAction = "http://tempuri.org/ICRUDClientService/GetRankingResponse")]
        Entities.RankingCredentials[] GetRanking();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetRanking", ReplyAction = "http://tempuri.org/ICRUDClientService/GetRankingResponse")]
        System.Threading.Tasks.Task<Entities.RankingCredentials[]> GetRankingAsync();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetStatistics", ReplyAction = "http://tempuri.org/ICRUDClientService/GetStatisticsResponse")]
        Entities.StatisticCredentials[] GetStatistics();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/ICRUDClientService/GetStatistics", ReplyAction = "http://tempuri.org/ICRUDClientService/GetStatisticsResponse")]
        System.Threading.Tasks.Task<Entities.StatisticCredentials[]> GetStatisticsAsync();

        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/GetClients", ReplyAction="http://tempuri.org/ICRUDClientService/GetClientsResponse")]
        Business.ClientDTO[] GetClients();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICRUDClientService/GetClients", ReplyAction="http://tempuri.org/ICRUDClientService/GetClientsResponse")]
        System.Threading.Tasks.Task<Business.ClientDTO[]> GetClientsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICRUDClientServiceChannel : CRUDClient.WebServices.ICRUDClientService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CRUDClientServiceClient : System.ServiceModel.ClientBase<CRUDClient.WebServices.ICRUDClientService>, CRUDClient.WebServices.ICRUDClientService {
        
        public CRUDClientServiceClient() {
        }
        
        public CRUDClientServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CRUDClientServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CRUDClientServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CRUDClientServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool CreateClient(Business.ClientDTO client) {
            return base.Channel.CreateClient(client);
        }
        
        public System.Threading.Tasks.Task<bool> CreateClientAsync(Business.ClientDTO client) {
            return base.Channel.CreateClientAsync(client);
        }
        
        public bool UpdateClient(Business.ClientDTO old, Business.ClientDTO newC) {
            return base.Channel.UpdateClient(old, newC);
        }
        
        public System.Threading.Tasks.Task<bool> UpdateClientAsync(Business.ClientDTO old, Business.ClientDTO newC) {
            return base.Channel.UpdateClientAsync(old, newC);
        }
        
        public bool DeleteClient(Business.ClientDTO client) {
            return base.Channel.DeleteClient(client);
        }
        
        public System.Threading.Tasks.Task<bool> DeleteClientAsync(Business.ClientDTO client) {
            return base.Channel.DeleteClientAsync(client);
        }

        public LogEntry GetLastLog(){
            return base.Channel.GetLastLog();
        }

        public System.Threading.Tasks.Task<LogEntry> GetLastLogAsync()
        {
            return base.Channel.GetLastLogAsync();
        }

        public Entities.RankingCredentials[] GetRanking()
        {
            return base.Channel.GetRanking();
        }

        public System.Threading.Tasks.Task<Entities.RankingCredentials[]> GetRankingAsync()
        {
            return base.Channel.GetRankingAsync();
        }

        public Entities.StatisticCredentials[] GetStatistics()
        {
            return base.Channel.GetStatistics();
        }

        public System.Threading.Tasks.Task<Entities.StatisticCredentials[]> GetStatisticsAsync()
        {
            return base.Channel.GetStatisticsAsync();
        }


        public Business.ClientDTO[] GetClients() {
            return base.Channel.GetClients();
        }
        
        public System.Threading.Tasks.Task<Business.ClientDTO[]> GetClientsAsync() {
            return base.Channel.GetClientsAsync();
        }
    }
}
