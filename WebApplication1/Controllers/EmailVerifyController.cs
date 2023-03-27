using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmailVerifyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EmailVerifyController> _logger;   
        private readonly EmailDBContext _dbContext;
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public EmailVerifyController(HttpClient httpClient,
            ILogger<EmailVerifyController> logger,EmailDBContext context)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:1234");
            _logger = logger;
            _dbContext = context;
        }

        [HttpPost]
        public async Task<IActionResult> SingleEmailVerifyAsync([FromBody] EmailSingle email)
        {
            var responseContent="";
            if (email == null || string.IsNullOrWhiteSpace(email.Address))
            {
                return NotFound();
            }
            List<EmailVerificationResult> emailsingle = new List<EmailVerificationResult>();

            var threshold = DateTime.UtcNow.ToLocalTime().AddHours(-24);

            if (Regex.IsMatch(email.Address, emailPattern))
            {
                var existingEmail = _dbContext.Emails.FirstOrDefault(x => x.Address == email.Address
                    && x.date >= threshold);
                EmailVerificationResult emailSingleDB = null;
                if (existingEmail==null)
                {
                    var response = await _httpClient.
                        PostAsJsonAsync("/api/EmailVerify/SingleEmailVerify", email);
                    responseContent = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<EmailVerificationResult>>(responseContent);
                    var result = results.FirstOrDefault();
                    var reason = result?.reason;
                    var emailSingle = new Emails
                    {
                        Address = email.Address,
                        date = DateTime.UtcNow.ToLocalTime(),
                        status = reason
                    };
                    _dbContext.Emails.Add(emailSingle);
                    _dbContext.SaveChanges();
                    return Ok(results);
                }
                else
                {
                    emailSingleDB = new EmailVerificationResult
                    {
                        email = existingEmail.Address,
                        reason = existingEmail.status
                    };
                    emailsingle.Add(emailSingleDB);
                    var emailLog = new EmailLogDB
                    {
                        email = email.Address,
                        timestamp = DateTime.UtcNow.ToLocalTime(),
                    };
                    _dbContext.EmailLogs.Add(emailLog);
                    _dbContext.SaveChanges();
                    return Ok(emailsingle);
                }

            }
            else
            {
                var emailInvalid = new EmailVerificationResult
                {
                    email = email.Address,
                    reason = "Invalid Email Address Pattern"
                };
                emailsingle.Add(emailInvalid);
                return Ok(emailsingle);
            }

        }
 
        [HttpPost]
        public async Task<IActionResult> BulkEmailsVerifyAsync([FromBody] EmailSingle []email)
        {
            if (email == null)
            {
                return NotFound(string.Empty);
            }
            var threshold = DateTime.UtcNow.ToLocalTime().AddMinutes(4);
            var emailInDB = _dbContext.Emails.Where(x => email.Select(y => y.Address).
                    Contains(x.Address) && x.date <= threshold).ToList();

            var emailNotInDB = email.Where(e =>!_dbContext.Emails.Any(x => x.Address == e.Address) ||
                _dbContext.Emails.Any(x => x.Address == e.Address &&
                x.date > threshold)).ToList();
           
            List<EmailVerificationResult> emailBulkDB = new List<EmailVerificationResult>();
            foreach (var e in emailInDB)
            {
                if (Regex.IsMatch(e.Address, emailPattern))
                {
                    var EB = new EmailVerificationResult
                    {
                        email = e.Address,
                        reason = e.status
                    };
                    emailBulkDB.Add(EB);
                    var emailLog = new EmailLogDB
                    {
                        email = e.Address,
                        timestamp = DateTime.Now.ToLocalTime(),
                    };
                    _dbContext.EmailLogs.Add(emailLog);
                    _dbContext.SaveChanges();
                }
            }

            if (emailNotInDB.Count > 0)
            {
                var batches = emailNotInDB
                .Select((email, index) => new { email, index })
                .GroupBy(x => x.index / 10)
                .Select(g => g.Select(x => x.email).ToList())
                .ToList();
                List<EmailVerificationResult> emailBulkNotInDB = new List<EmailVerificationResult>();
                List<EmailVerificationResult> emailBulk=new List<EmailVerificationResult>();
                foreach (var batch in batches)
                {
                    EmailSingle[] emailNotDB = new EmailSingle[0];
                    foreach (var e in batch)
                    {
                        if (Regex.IsMatch(e.Address, emailPattern))
                        {
                            var em = new EmailSingle
                            {
                                Address = e.Address
                            };
                            Array.Resize(ref emailNotDB, emailNotDB.Length + 1);
                            emailNotDB[emailNotDB.Length - 1] = em;
                        }
                        else
                        {
                            var emailInvalid = new EmailVerificationResult
                            {
                                email = e.Address,
                                reason = "Invalid Email Address Pattern"
                            };
                            emailBulkNotInDB.Add(emailInvalid);
                        }
                    }
                    var response = await _httpClient.
                            PostAsJsonAsync("/api/EmailVerify/BulkEmailsVerify", emailNotDB);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<EmailVerificationResult>>(responseContent);
                   
                    foreach (var e in emailNotDB)
                    {
                        var result = results.FirstOrDefault(rc => rc.email == e.Address);
                        var reason = result?.reason;
                        var eB = new Emails
                        {
                            Address = e.Address,
                            date = DateTime.UtcNow.ToLocalTime(),
                            status = reason
                        };
                        _dbContext.Emails.Add(eB);
                        _dbContext.SaveChanges();
                        
                    }
                    emailBulkNotInDB.AddRange(results);
                }
                emailBulk = emailBulkDB.Concat(emailBulkNotInDB).ToList();
                return Ok(emailBulk);
            }
            else
            {
                return Ok(emailBulkDB.ToList());
            }
            
        }
    }
}
