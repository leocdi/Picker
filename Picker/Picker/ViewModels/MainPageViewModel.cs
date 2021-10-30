using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Picker;
using IS.Toolkit.XamarinForms.Controls;

namespace Picker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";
        }


        private ObservableCollection<AvailableValue> _pickerItems;
        public ObservableCollection<AvailableValue> PickerItems
        {
            get { return _pickerItems; }
            set { SetProperty(ref _pickerItems, value); }
        }

        private AvailableValue _selectedPickerItem;
        public AvailableValue SelectedPickerItem
        {
            get { return _selectedPickerItem; }
            set { SetProperty(ref _selectedPickerItem, value); }
        }



        public override void Initialize(INavigationParameters parameters)
        {
            PickerItems = CreatePickerItems();
            SelectedPickerItem = PickerItems.ElementAt(50);
        }

        private  ObservableCollection<AvailableValue> CreatePickerItems()
        {
            List<AvailableValue> pickItems = new List<AvailableValue>();

            for (int i = 0; i < 100; i++)
            {
                pickItems.Add(new AvailableValue() { Value = i, Label = $"ItemNumber{i}" });
            }

            return new ObservableCollection<AvailableValue>(pickItems);
        }
    }
}
