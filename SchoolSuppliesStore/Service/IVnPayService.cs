namespace SchoolSuppliesStore.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequest model);
        VnPaymentResponse PaymentExecute(IQueryCollection collection);
    }
}
