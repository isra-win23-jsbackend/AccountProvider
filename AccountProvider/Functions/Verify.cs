using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions
{
    public class Verify(ILogger<Verify> logger, UserManager<UserAccount> userManager)
    {
        private readonly ILogger<Verify> _logger = logger;
        private readonly UserManager<UserAccount> _userManager = userManager;



        [Function("Verify")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : verify.StreamReader.Run() :: {ex.Message}");
            }
            if (body != null)
            {
               VerifyRequest vr = null!;

                try
                {
                    vr = JsonConvert.DeserializeObject<VerifyRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR : Verify.DeserializeObject.Run() :: {ex.Message}");
                }

                if( vr != null && !string.IsNullOrEmpty(vr.Email) && !string.IsNullOrEmpty(vr.VeryfyCode))
                {
                    //Verify code using VerificationProvider


                    try
                    {
                        using var http = new HttpClient();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(vr), Encoding.UTF8, "application/json");
                        //var response = await http.PostAsync("https://verificationcodeprovider.azurewebsites.net/api/verify", content);

                        /*(response.IsSuccessStatusCode)*/
                        if (true)
                        {


                            var userAccount = await _userManager.FindByEmailAsync(vr.Email);
                            if (userAccount != null)
                            {
                                userAccount.EmailConfirmed = true;
                                await _userManager.UpdateAsync(userAccount);


                                if (await _userManager.IsEmailConfirmedAsync(userAccount))
                                {
                                    return new OkResult();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR : SignUP.PostAsync(VerificationCodeprovider) :: {ex.Message}");
                    }

                    return new OkResult();



                }
            }
            return new UnauthorizedResult();


        }
    }
}
