﻿using System;
using System.Threading.Tasks;
using StudyBuddy.App.Api;
using StudyBuddy.App.ViewModels;

namespace StudyBuddy.App.Test.Mocks
{
    public class AuthenticationServiceMock : IAuthenticationService
    {
        public event LoginStateChangedHandler LoginStateChanged;
        private readonly string base_url;
        public string Token { get; private set; } = string.Empty;
        public UserViewModel CurrentUser { get; private set; }
        public bool IsLoggedIn => Token != string.Empty;

        public async Task<bool> LoginFromJson(string content)
        {
            var result = await Login(new UserCredentials()
            {
                EMail="alexander.stuckenholz@hshl.de",
                Password = "secret"
            });

            return result == 1;
        }

        public void Logout()
        {
            CurrentUser = null;
            Token = string.Empty;
        }

        public async Task<bool> IsTokenValid(string token)
        {
            return true;
        }

        public async Task<bool> SendPasswortResetMail(string email)
        {
            return true;
        }

        public async Task<int> Login(UserCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.EMail))
                throw new Exception("Missing Email!");

            if (string.IsNullOrEmpty(credentials.Password))
                throw new Exception("Missing Password!");

            return await Task.Run(() =>
            {
                CurrentUser = new UserViewModel() { ID = 1, Firstname = "Test", Lastname = "Test" };
                Token = "secret_token";

                if (LoginStateChanged != null)
                    LoginStateChanged(this, new LoginStateChangedArgs(true, CurrentUser, Token));

                return 0;
            });
        }
    }
}