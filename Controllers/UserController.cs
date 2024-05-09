using EBanking.Data.Entities;
using EBanking.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EBanking.Views.User;

namespace EBanking.Controllers
{
   
    public class UserController(IUserService userService, IAccountService accountService) : Controller
    {
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly IAccountService _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            try
            {
                var newUser = _userService.RegisterUser(user.Username, user.Password, user.FullName, user.Email);
                return RedirectToAction("Login"); 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            try
            {
                var user = _userService.LoginUser(username, password);

                if (user != null)
                {
                    return RedirectToAction("MyAccounts");
                }
                else
                {
                    return RedirectToAction("Register");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Грешка: {ex.Message}");
            }
        }

        [HttpGet("myAccounts")]
        public IActionResult MyAccounts()
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userAccounts = _userService.GetUserAccounts(loggedInUserId);

                var accountTransactions = new Dictionary<UserAccount, IEnumerable<Transaction>>();

                foreach (var account in userAccounts)
                {
                    var transactions = _userService.GetAccountTransactions(account.Id);

                    accountTransactions.Add(account, transactions);
                }
                
                return View(accountTransactions);
            }
            catch (Exception)
            {
                return BadRequest("Грешка при зареждане на сметките");
            }
        }

 
         

        [HttpPost("create-account")]
        public IActionResult CreateAccount(string friendlyName)
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (loggedInUserId == null)
                {
                    return RedirectToAction("Register");
                }

                var newAccount = _accountService.CreateAccount(loggedInUserId, friendlyName);

                return RedirectToAction("MyAccounts");
            }
            catch (Exception)
            {
                return BadRequest("Грешка при създаване на сметка");
            }
        }

        [HttpPost("transfer")]
        public IActionResult Transfer(int sendingAccountId, int receiverAccountId, decimal amount)
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (loggedInUserId == null)
                {
                    return RedirectToAction("Register");
                }

                _accountService.Transfer(sendingAccountId, receiverAccountId, amount);
                return RedirectToAction("MyAccounts");
            }
            catch (Exception)
            {
                return BadRequest("Грешка при извършване на превод");
            }
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userAccounts = _userService.GetUserAccounts(loggedInUserId);

            var model = new TransferViewModel
            {
                AccountsSelectList = userAccounts.Select(a => new SelectListItem
                {
                    Text = a.FriendlyName, 
                    Value = a.Id.ToString() 
                })
            };
            return View(model);

        }

        [HttpPost("deposit")]
        public IActionResult Deposit(int accountId, decimal amount)
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var account = _userService.GetUserAccounts(loggedInUserId);

                if (account.All(a => a.Id != accountId))
                {
                    return BadRequest("Невалидна сметка за депозит");
                }
                _accountService.Deposit(accountId, amount);
                return RedirectToAction("MyAccounts");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public IActionResult Withdraw(int accountId, decimal amount)
        {
            try
            {
                var loggedInUserId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var account = _userService.GetUserAccounts(loggedInUserId);
                if (account.All(a => a.Id != accountId))
                {
                    return BadRequest("Невалидна сметка за теглене");
                }

                _accountService.Withdraw(accountId, amount);

                return RedirectToAction("MyAccounts");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
