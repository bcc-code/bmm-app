using System.Drawing;
using Acr.UserDialogs;
using BMM.Api.Implementation.Clients;
using BMM.Api.Implementation.Clients.Contracts;
using BMM.Api.Implementation.Models;
using BMM.Core.Constants;
using BMM.Core.Extensions;
using BMM.Core.GuardedActions.Base;
using BMM.Core.GuardedActions.SuggestEdit.Interfaces;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ViewModels;
using Microsoft.Maui.Devices;
using MvvmCross.Navigation;

namespace BMM.Core.GuardedActions.SuggestEdit;

public class SubmitSuggestedEditAction : 
    GuardedAction,
    ISubmitSuggestedEditAction
{
    private readonly ITranscriptionClient _transcriptionClient;
    private readonly IUserDialogs _userDialogs;
    private readonly IBMMLanguageBinder _bmmLanguageBinder;
    private readonly IMvxNavigationService _mvxNavigationService;

    public SubmitSuggestedEditAction(
        ITranscriptionClient transcriptionClient,
        IUserDialogs userDialogs,
        IBMMLanguageBinder bmmLanguageBinder,
        IMvxNavigationService mvxNavigationService)
    {
        _transcriptionClient = transcriptionClient;
        _userDialogs = userDialogs;
        _bmmLanguageBinder = bmmLanguageBinder;
        _mvxNavigationService = mvxNavigationService;
    }
    
    private SuggestEditViewModel DataContext => this.GetDataContext();
    
    protected override async Task Execute()
    {
        var transcriptions = DataContext.Transcriptions.Where(t => t.HasChanges)
            .Select(t => new SuggestEditTranscription()
            {
                NewText = t.Text,
                OriginalText = t.Transcription.Text,
                SegmentIndex = t.Transcription.Id
            })
            .ToList();

        await _transcriptionClient.PostSuggestEdit(
            DataContext.NavigationParameter.TrackPO.Id,
            SuggestEditViewModel.DefaultTranscriptionLanguage,
            transcriptions);

        await DataContext.CloseCommand.ExecuteAsync();
        
        _userDialogs.Toast(new ToastConfig(_bmmLanguageBinder[Translations.SuggestEditViewModel_SubmitSuccess])
        {
            Duration = TimeSpan.FromSeconds(5),
            BackgroundColor = Color.Green,
            MessageTextColor = Color.White
        });
    }

    protected override bool CanExecute()
    {
        return base.CanExecute() && DataContext.Transcriptions.Any(t => t.HasChanges);
    }
}