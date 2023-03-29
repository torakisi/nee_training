using NEE.Core.Exceptions;
using NEE.Core.Helpers;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace XServices.Idika
{
    public class AmkaWebServiceConnectionString : WebServiceConnectionString
    {
        public AmkaWebServiceConnectionString(string connectionString) : base(connectionString) { }
        public AmkaWebServiceConnectionString(string username, string password, string url) : base(username, password, url) { }
    }
    public class AmkaServiceGateway
    {
        private AmkaWebServiceConnectionString _amkaWsConStr;       // AMKA : "uid|pwd|url"

        public AmkaServiceGateway(AmkaWebServiceConnectionString amkaWsConStr)
        {
            _amkaWsConStr = amkaWsConStr;
        }

        public bool CanConnect()
        {
            try
            {
                var client = this.CreateAmkaClient();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private XService.Idika.AMKA_ServiceReference.ServiceSoapClient CreateAmkaClient()
        {
            var binding = _amkaWsConStr.Url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ? (HttpBindingBase)(new BasicHttpsBinding()) : (HttpBindingBase)(new BasicHttpBinding());
            var client = new XService.Idika.AMKA_ServiceReference.ServiceSoapClient(binding, new EndpointAddress(_amkaWsConStr.Url));
            return client;
        }

        public async Task<XService.Idika.AMKA_ServiceReference.WS_Response> GetAmkaRegistryInfo(string amka, string afm)
        {
            var client = this.CreateAmkaClient();
            try
            {
                // NOTE: this service works as: ( AMKA=? or AFM=? ), meaning that it can return two different people, one matched by AMKA and one by AFM !!!
                var res = await client.AFM2DAsync(_amkaWsConStr.Uid, _amkaWsConStr.Pwd, amka, afm);
                return res.Body.AFM2DResult;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw new XSRemoteCallFailed(nameof(IDIKAService), ex);
            }
            finally
            {
                client.Close();
            }
        }

    }
}
