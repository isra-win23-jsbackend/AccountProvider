using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace AccountProvider.Functions
{
    public class SignIn(ILogger<SignIn> logger, SignInManager<UserAccount> signInManager)
    {
        private readonly ILogger<SignIn> _logger = logger;
        private readonly SignInManager<UserAccount> _signInManager = signInManager;


        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : StreamReader.Run() :: {ex.Message}");
            }

            if (body != null)
            {
                UserSignInRequest ulr = null!;

                try
                {
                    ulr = JsonConvert.DeserializeObject<UserSignInRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR : SignIn.DeserializeObject.Run() :: {ex.Message}");
                }
                if (ulr != null && !string.IsNullOrEmpty(ulr.Email) && !string.IsNullOrEmpty(ulr.Password))
                {
                    try
                    {
                        var result = await _signInManager.PasswordSignInAsync(ulr.Email, ulr.Password, ulr.IsPresistent, false);
                        if (result.Succeeded)
                        {
                            //Get token från TokenProvider

                            return new OkObjectResult("accestoken");
                        }
                        return new UnauthorizedResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ERROR : SignIn. await _signInManager.PasswordSignInAsync.Run() :: {ex.Message}");
                    }
                }
            }
            return new BadRequestResult();

        }
    }
}
