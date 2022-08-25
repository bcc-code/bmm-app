using System;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.Core.ViewModels;
using BMM.UI.iOS;
using BMM.UI.iOS.Constants;
using CoreAnimation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views.Gestures;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.ViewModels;
using UIKit;

namespace CityIndex.Mobile.iOS.ViewControllers.Settings
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext)]
    public partial class ListeningStreakDetailsViewController : BaseViewController<ListeningStreakDetailsViewModel>
    {
        private const float AlphaPercentage = 0.5f;

        public ListeningStreakDetailsViewController() : base(nameof(ListeningStreakDetailsViewController))
        {
        }

        public override Type ParentViewControllerType { get; }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AlphaView.Alpha = 0;

            ContainerView.ClipsToBounds = true;
            ContainerView.Layer.CornerRadius = 36;
            ContainerView.Layer.MaskedCorners = CACornerMask.MinXMinYCorner | CACornerMask.MaxXMinYCorner;
            UIView.Animate(
                0.3f,
                0.3f,
                UIViewAnimationOptions.CurveLinear,
                () => AlphaView.Alpha = AlphaPercentage,
                null);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            AlphaView.Alpha = 0;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var set = this.CreateBindingSet<ListeningStreakDetailsViewController, ListeningStreakDetailsViewModel>();
            
            set.Bind(TapToCloseView.Tap())
                .For(v => v.Command)
                .To(vm => vm.CloseCommand);

            set.Bind(Title)
                .To(vm => vm.TextSource[Translations.Streak_Message]);
            
            set.Bind(Subtitle)
                .To(vm => vm.ListeningStreak.EligibleUntil)
                .WithConversion<StreakEligibleUntilMessageTextValueConverter>();
            
            set.Bind(Subtitle)
                .For(v => v.BindVisible())
                .To(vm => vm.ListeningStreak.EligibleUntil)
                .WithConversion<StreakEligibleUntilMessageVisibilityValueConverter>();
                        
            set.Bind(DaysInARowLabel)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DaysInARowMessageValueConverter>();
            
            set.Bind(PerfectWeeksLabel)
                .To(vm => vm.ListeningStreak)
                .WithConversion<PerfectWeekCountValueConverter>();
            
            set.Bind(MondayView)
                .For(v => v.Alpha)
                .To(v => v.ListeningStreak.ShowMonday)
                .WithConversion<BoolToNfloatConverter>();
            set.Bind(MondayView)
                .For(v => v.BackgroundColor)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Monday.ToString());
            set.Bind(MondayColorView)
                .For(v => v.BackgroundColor)
                .To(v => v.ListeningStreak.MondayColor)
                .WithConversion<HexStringToUiColorConverter>();
            set.Bind(MondayColorView.Layer)
                .For(v => v.BorderWidth)
                .To(v => v.ListeningStreak.Monday)
                .WithConversion<BorderVisibilityConverter>();

            set.Bind(TuesdayView)
                .For(v => v.Alpha)
                .To(v => v.ListeningStreak.ShowTuesday)
                .WithConversion<BoolToNfloatConverter>();
            set.Bind(TuesdayView)
                .For(v => v.BackgroundColor)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Tuesday.ToString());
            set.Bind(TuesdayColorView)
                .For(v => v.BackgroundColor)
                .To(v => v.ListeningStreak.TuesdayColor)
                .WithConversion<HexStringToUiColorConverter>();
            set.Bind(TuesdayColorView.Layer)
                .For(v => v.BorderWidth)
                .To(v => v.ListeningStreak.Tuesday)
                .WithConversion<BorderVisibilityConverter>();

            set.Bind(WednesdayView)
                .For(v => v.Alpha)
                .To(v => v.ListeningStreak.ShowWednesday)
                .WithConversion<BoolToNfloatConverter>();
            set.Bind(WednesdayView)
                .For(v => v.BackgroundColor)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Wednesday.ToString());
            set.Bind(WednesdayColorView)
                .For(v => v.BackgroundColor)
                .To(v => v.ListeningStreak.WednesdayColor)
                .WithConversion<HexStringToUiColorConverter>();
            set.Bind(WednesdayColorView.Layer)
                .For(v => v.BorderWidth)
                .To(v => v.ListeningStreak.Wednesday)
                .WithConversion<BorderVisibilityConverter>();

            set.Bind(ThursdayView)
                .For(v => v.Alpha)
                .To(v => v.ListeningStreak.ShowThursday)
                .WithConversion<BoolToNfloatConverter>();
            set.Bind(ThursdayView)
                .For(v => v.BackgroundColor)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Thursday.ToString());
            set.Bind(ThursdayColorView)
                .For(v => v.BackgroundColor)
                .To(v => v.ListeningStreak.ThursdayColor)
                .WithConversion<HexStringToUiColorConverter>();
            set.Bind(ThursdayColorView.Layer)
                .For(v => v.BorderWidth)
                .To(v => v.ListeningStreak.Thursday)
                .WithConversion<BorderVisibilityConverter>();

            set.Bind(FridayView)
                .For(v => v.Alpha)
                .To(v => v.ListeningStreak.ShowFriday)
                .WithConversion<BoolToNfloatConverter>();
            set.Bind(FridayView)
                .For(v => v.BackgroundColor)
                .To(vm => vm.ListeningStreak)
                .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Friday.ToString());
            set.Bind(FridayColorView)
                .For(v => v.BackgroundColor)
                .To(v => v.ListeningStreak.FridayColor)
                .WithConversion<HexStringToUiColorConverter>();
            set.Bind(FridayColorView.Layer)
                .For(v => v.BorderWidth)
                .To(v => v.ListeningStreak.Friday)
                .WithConversion<BorderVisibilityConverter>();

            SetThemes();
            set.Apply();
        }
        
        private void SetThemes()
        {
            MondayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            TuesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            WednesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            ThursdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            FridayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            
            Title.ApplyTextTheme(AppTheme.Title1);
            Subtitle.ApplyTextTheme(AppTheme.Subtitle3Label4);
            MondayLetter.ApplyTextTheme(AppTheme.Subtitle3Label1);
            TuesdayLetter.ApplyTextTheme(AppTheme.Subtitle3Label1);
            WednesdayLetter.ApplyTextTheme(AppTheme.Subtitle3Label1);
            ThursdayLetter.ApplyTextTheme(AppTheme.Subtitle3Label1);
            FridayLetter.ApplyTextTheme(AppTheme.Subtitle3Label1);
            DaysInARowLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            PerfectWeeksLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }
    }
}