namespace WebApplication1
{
    public class EmailVerificationResult
    {
        public string email { get; set; }
        public bool valid { get; set; }
        public bool disposable { get; set; }
        public bool valid_mx { get; set; }
        public bool mx_reachable { get; set; }
        public bool catchall { get; set; }
        public string reason { get; set; }
        
    }
}
