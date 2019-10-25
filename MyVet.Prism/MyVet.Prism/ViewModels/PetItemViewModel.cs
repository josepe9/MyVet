using MyVet.Common.Models;
using Prism.Commands;
using Prism.Navigation;

namespace MyVet.Prism.ViewModels
{
    public class PetItemViewModel : PetResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectPetComman;
        public PetItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectPetCommand => _selectPetComman ?? (_selectPetComman = new DelegateCommand(SelectPet));

        private async void SelectPet()
        {
            //para enviar parametros a otra pagina: pet es el nombre del parametro y this es el valor
            // en este caso es igual a PetResponse
            var parameters = new NavigationParameters
            {
                { "pet", this }
            };

            await _navigationService.NavigateAsync("PetPage");
        }
    }

}
