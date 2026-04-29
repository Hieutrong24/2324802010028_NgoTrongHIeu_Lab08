namespace ASC.Web.Areas.Accounts.Models
{
    public class ServiceEngineerViewModel
    {
        public List<ServiceEngineerRegistrationViewModel> ServiceEngineers { get; set; }
            = new List<ServiceEngineerRegistrationViewModel>();

        public ServiceEngineerRegistrationViewModel ServiceEngineer { get; set; }
            = new ServiceEngineerRegistrationViewModel();
    }
}