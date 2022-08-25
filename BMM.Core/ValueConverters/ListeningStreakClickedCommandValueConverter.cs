using System;
using System.Globalization;
using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Commands;
using MvvmCross.Converters;

namespace BMM.Core.ValueConverters
{
    public class ListeningStreakClickedCommandValueConverter : MvxValueConverter<CellWrapperViewModel<Document>, IMvxCommand>
    {
        protected override IMvxCommand Convert(CellWrapperViewModel<Document> value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Item is not ListeningStreak listeningStreak)
                return null;
            
            var viewModel = (ExploreNewestViewModel)value.ViewModel;
            return new MvxCommand(() => viewModel.ListeningStreakClickedCommand.Execute(listeningStreak));
        }
    }
}