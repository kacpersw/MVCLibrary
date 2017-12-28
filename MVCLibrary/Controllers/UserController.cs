﻿using MVCLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCLibrary.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsUserVerified, ActivationCode")] Users user)
        {
            bool Status = false;
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                var isExist = IsEmailExist(user.EmailID);

                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email już istnieje w bazie");
                    return View(user);
                }

                user.ActivationCode = Guid.NewGuid();

                user.Pass = Crypto.Hash(user.Pass);
                user.ConfirmPass = Crypto.Hash(user.ConfirmPass);
                user.IsUserVerified = true;

                using (LibraryEntities dbContext = new LibraryEntities())
                {
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();

                    message = "Rejestracja zakończona pomyślnie. Link aktywacyjny wysłano na maila";

                    Status = true;
                }
            }
            else
            {
                message = "Nieprawidłowe żądanie";
            }


            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin user, string ReturnUrl="")
        {
            string message = string.Empty;

            using(LibraryEntities dbContext = new LibraryEntities())
            {
                var us = dbContext.Users.Where(u => u.EmailID == user.EmailID).FirstOrDefault();

                if (us != null)
                {
                    if(string.Compare(Crypto.Hash(user.Password),us.Pass)==0)
                    {
                        int timeout = user.RememberMr ? 525600 : 20;
                        var ticket = new FormsAuthenticationTicket(user.EmailID, user.RememberMr, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Niepoprawne dane";
                    }
                }
                else
                {
                    message = "Niepoprawne dane";
                }
            }

            ViewBag.Message = message;

            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }


        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;

            using (LibraryEntities dbContext = new LibraryEntities())
            {
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                var v = dbContext.Users.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

                if (v != null)
                {
                    v.IsUserVerified = true;
                    dbContext.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Nieprawidłowe żądanie";
                }
            }

            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (LibraryEntities dbContext = new LibraryEntities())
            {
                var user = dbContext.Users.Where(u => u.EmailID == emailID).FirstOrDefault();
                return user != null;
            }
        }
    }
}