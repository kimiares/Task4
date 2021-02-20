using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UsersTask4.Models;
using UsersTask4.ViewModels;

namespace UsersTask4.Controllers
{
    
    public class HomeController : Controller
    {
       
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Name, 
                 DateOfRegistration = DateTime.Now, LastLogin = DateTime.Now, IsActive=true, Flag = false };
               
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiplyBlock(string[] usersToDeleteIds)
        {
            
            if (usersToDeleteIds != null)
            {


                foreach (var u in usersToDeleteIds)
                {
                    var user = await _userManager.FindByIdAsync(u);
                    do
                    {

                        if (user != null)
                        {
                            user.IsActive = false;
                            user.LockoutEnabled = true;
                            user.LockoutEnd = DateTime.Now.AddYears(300);
                            await _userManager.UpdateAsync(user);


                        }
                        else
                        {
                            ModelState.AddModelError("", "User Not Found");

                        }
                    }
                    while (User.Identity.Name == user.UserName);
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    

                }
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiplyUnblock(string[] usersToDeleteIds)
        {

            if (usersToDeleteIds != null)
            {


                foreach (var u in usersToDeleteIds)
                {
                    var user = await _userManager.FindByIdAsync(u);
                    if (user != null)
                    {
                        user.IsActive = true;
                        user.LockoutEnabled = false;
                        user.LockoutEnd = DateTime.Now;
                        await _userManager.UpdateAsync(user);

                        
                        


                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");

                    }

                }
            }
            return RedirectToAction("Index");

        }

        
       
        [HttpPost]
        public async Task<IActionResult> MultiplyDelete(string[] usersToDeleteIds)
        {
            if (usersToDeleteIds != null)
            {


                foreach (var u in usersToDeleteIds)
                {
                    var user = await _userManager.FindByIdAsync(u);
                    if (user != null)
                    {
                        await _userManager.DeleteAsync(user);
                        if (User.Identity.Name == user.UserName)
                        {
                            return RedirectToAction("Login", "Home");
                        }
                        //await _userManager.DeleteAsync(user);
                        

                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");

                    }

                }
            }
            return RedirectToAction("Index");




        }
       



        [HttpGet]

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [Route("Home/Login")]

        public async Task<IActionResult> Login(LoginViewModel model) // 
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        return NotFound("Unable to load user for update last login.");
                    }
                    user.LastLogin = DateTime.Now;
                    var lastLoginResult = await _userManager.UpdateAsync(user);
                    if (!lastLoginResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Unexpected error occurred setting the last login date" +
                            $" ({lastLoginResult}) for user with ID '{user.Id}'.");
                    }



                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }
        [HttpPost]
       
        public async Task<IActionResult> Logout()
        {
            
            await _signInManager.SignOutAsync();
            return RedirectToAction("Register", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
