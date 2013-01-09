using System.Xml.Linq;

namespace ODataPad.Core.Models
{
    public class ServiceError
    {
        public ServiceError(string serviceName, string errorMessage, string errorDescription)
        {
            this.ServiceName = serviceName;
            this.ErrorMessage = errorMessage;
            this.ErrorDescription = errorDescription;
        }

        public string ServiceName { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorDescription { get; private set; }
    }
}