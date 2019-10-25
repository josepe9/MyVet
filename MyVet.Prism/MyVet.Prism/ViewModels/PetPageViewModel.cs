using MyVet.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyVet.Prism.ViewModels
{
    public class PetPageViewModel : ViewModelBase
    {
        //objeto para recibir los parametros 
        private PetResponse _pet;

        public PetPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public PetResponse Pet
        {
            get => _pet;
            set => SetProperty(ref _pet, value);
        }

        //metodo que recibe los parametros enviados 
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            //validar si el parametro llego con el nombre que se le dio 
            if (parameters.ContainsKey("pet"))
            {
                Pet = parameters.GetValue<PetResponse>("pet");
                Title = Pet.Name;
            }
        }
    }

}
