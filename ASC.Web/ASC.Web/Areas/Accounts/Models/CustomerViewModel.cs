namespace ASC.Web.Areas.Accounts.Models
{
    public class CustomerViewModel
    {
        public List<CustomerRegistrationViewModel> Customers { get; set; }
            = new List<CustomerRegistrationViewModel>();

        public CustomerRegistrationViewModel Customer { get; set; }
            = new CustomerRegistrationViewModel();
    }
}