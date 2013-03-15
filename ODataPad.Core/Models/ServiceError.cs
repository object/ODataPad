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

        // Aliases for XAML templates
        public string Name { get { return this.ServiceName; } }
        public string Summary { get { return this.ErrorMessage; } }
        public string Description { get { return this.ErrorDescription; } }
    }
}