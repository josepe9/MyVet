using MyVet.Common.Models;
using MyVet.Common.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        //El orden en las clases es:
        //1. propiedades privadas
        //2. Constructor
        //3. propiedades publicas
        //4. metodos publicos
        //5. metodos privados
        //Los campos privados inician con _ underline
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;  //para el boton

        public LoginPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            Title = "Login";
            IsEnabled = true;
            _navigationService = navigationService;
            _apiService = apiService;

            Email = "jzuluaga55@hotmail.com";
            Password = "123456";
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public string Email { get; set; }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password", "Accept");
                return;
            }

            //Isrunning para que el activiti indicator de vueltas con true
            //desabilitamos el botón con IsEnabled = false
            IsRunning = true;
            IsEnabled = false;

            var request = new TokenRequest
            {
                Password = Password,
                Username = Email
            };

            var url = App.Current.Resources["UrlAPI"].ToString();

            //para consumir el GetTokenAsync necesitamos los sig parámetros:
            //la urlBase la direccion que sacamos del diccionario de recursos de App.xaml https://myvetjose.azurewebsites.net/
            //segundo parametro el prefijo Account
            //tercero el controlador /CreateToken
            //cuarto el request
            var response = await _apiService.GetTokenAsync(url, "Account", "/CreateToken", request);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Email or password incorrect.", "Accept");
                Password = string.Empty; //limpiamos el password
                return;
            }

            //Tomar el token  es necesario hacer un cast
            var token = (TokenResponse)response.Result;
            var response2 = await _apiService.GetOwnerByEmailAsync(url, "api", "/Owners/GetOwnerByEmail", "bearer", token.Token, Email);
            if (!response2.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "This user have a big problem, call support.", "Accept");
                return;
            }

            var owner = (OwnerResponse)response2.Result;
            //pasar parámetros a la vista siguiente
            var parameters = new NavigationParameters
            {
                {"owner", owner }
            };

            IsRunning = false;
            IsEnabled = true;

            Password = string.Empty;
            //lo enviamos a la página PetsPage con los parametros del objeto owner
            await _navigationService.NavigateAsync("PetsPage", parameters);
//            await App.Current.MainPage.DisplayAlert("Ok", "fuck yeahhh!!!.", "Accept");
        }

    }
}
