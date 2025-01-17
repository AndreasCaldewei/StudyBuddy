﻿using System.Threading.Tasks;
using System.Windows.Input;
using StudyBuddy.App.Api;
using StudyBuddy.App.Misc;
using StudyBuddy.App.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace StudyBuddy.App.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public IAsyncCommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public IAsyncCommand PasswordForgottenCommand { get; }
        public ICommand ImprintCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand RecoverCommand { get; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string ApiVersionAsString { get => api.ApiVersion.ToString(); }
        public string AppVersionAsString { get => api.AppVersion.ToString(); }

        public LoginViewModel(IApi api) : base(api)
        {
            LoginCommand = new AsyncCommand(Login, () => { return IsEMailValid && IsPasswordValid; });
            RegisterCommand = new Command(Register);
            PasswordForgottenCommand = new AsyncCommand(PasswordForgotten, () => { return IsEMailValid; });
            ImprintCommand = new Command(Imprint);
            InfoCommand = new Command(Info);

            api.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals("ApiVersion"))
                    NotifyPropertyChanged("ApiVersionAsString");
            };
        }

        private bool is_email_valid = false;
        public bool IsEMailValid
        {
            get { return is_email_valid; }
            set
            {
                is_email_valid = value;
                NotifyPropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
                PasswordForgottenCommand.RaiseCanExecuteChanged();
            }
        }

        private bool is_password_valid = false;
        public bool IsPasswordValid
        {
            get { return is_password_valid; }
            set
            {
                is_password_valid = value;
                NotifyPropertyChanged();
                LoginCommand.RaiseCanExecuteChanged();
                PasswordForgottenCommand.RaiseCanExecuteChanged();
            }
        }

        private void Info()
        {
            api.Device.OpenBrowser("https://gameucation.eu/");
        }

        private async void Register()
        {
            await api.Device.PushPage(new RegisterPage());
        }

        private async Task PasswordForgotten()
        {
            var answer = await api.Device.ShowMessage(
                "Wollen Sie eine E-Mail an '" + EMail + "' schicken, um das Passwort zurückzusetzen?",
                "Passwort zurücksetzen?",
                "Ja", "Nein", null);

            if (!answer)
                return;

            answer = await api.Authentication.SendPasswortResetMail(EMail);
            if (!answer)
                api.Device.ShowError("Ein Fehler ist aufgetreten!", "Fehler!", "Ok", null);
            else
                api.Device.ShowMessage("Eine E-Mail zum zurücksetzen des Passworts wurde verschickt.", "Passwort zurücksetzen");
        }

        private void Imprint()
        {
            api.Device.OpenBrowser("https://gameucation.eu/impressum/");
        }

        private async Task Login()
        {
            var uc = new UserCredentials { EMail = EMail, Password = Password };
            var result = await api.Authentication.Login(uc);

            switch(result)
            {
                case 0:
                    await api.Device.GoToPath("//ChallengesPage");
                    break;
                case 1:
                    var url = "https://backend.gameucation.eu/login/verificationrequired;email=";
                    var link = url + uc.EMail;
                    api.Device.OpenBrowser(link);
                    break;
                case 2:
                    api.Device.ShowMessageBox("E-Mail-Aresse oder Passwort ist falsch!", "Achtung!");
                    break;
                case 3:
                    api.Device.ShowMessageBox("Es konnte kein Konto mit dieser E-Mail-Adresse gefunden werden.", "Achtung!");
                    break;
                case 4:
                    api.Device.ShowMessageBox("Anmeldung nicht erfolgreich! Zugangsdaten korrekt?", "Achtung!");
                    await api.Logging.LogError("Invalid API response for login with " + uc.EMail);
                    break;
                case 5:
                    api.Device.ShowMessageBox("Anmeldung nicht erfolgreich! Zugangsdaten korrekt?", "Achtung!");
                    await api.Logging.LogError("Issue in loginfromjson, no Token/User or Token invalid");
                    break;
                default:
                    api.Device.ShowMessageBox("Anmeldung nicht erfolgreich! Zugangsdaten korrekt?", "Achtung!");
                    await api.Logging.LogError("Undocumented error");
                    break;
            }
        }
    }
}