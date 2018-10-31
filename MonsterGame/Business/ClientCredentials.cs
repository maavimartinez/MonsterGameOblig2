using System.Runtime.Serialization;

namespace Business
{
    [DataContract]
    public class ClientCredentials
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}