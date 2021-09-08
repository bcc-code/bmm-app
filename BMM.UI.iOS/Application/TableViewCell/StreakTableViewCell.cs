using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Implementations.Localization;
using BMM.Core.Implementations.Localization.Interfaces;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;

namespace BMM.UI.iOS
{
    public partial class StreakTableViewCell : BaseTrackTableViewCell
    {
        public static readonly NSString Key = new NSString(nameof(StreakTableViewCell));
        private IBMMLanguageBinder BMMLanguageBinder => BMMLanguageBinderLocator.TextSource;

        public StreakTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                MondayColorView.Layer.BorderColor = AppColors.StreakColorBubbleBorderColor.CGColor;
                TuesdayColorView.Layer.BorderColor = AppColors.StreakColorBubbleBorderColor.CGColor;
                WednesdayColorView.Layer.BorderColor = AppColors.StreakColorBubbleBorderColor.CGColor;
                ThursdayColorView.Layer.BorderColor = AppColors.StreakColorBubbleBorderColor.CGColor;
                FridayColorView.Layer.BorderColor = AppColors.StreakColorBubbleBorderColor.CGColor;

                MondayLabel.Text = BMMLanguageBinder[Translations.Streak_WeekdayMonday];
                TuesdayLabel.Text = BMMLanguageBinder[Translations.Streak_WeekdayTuesday];
                WednesdayLabel.Text = BMMLanguageBinder[Translations.Streak_WeekdayWednesday];
                ThursdayLabel.Text = BMMLanguageBinder[Translations.Streak_WeekdayThursday];
                FridayLabel.Text = BMMLanguageBinder[Translations.Streak_WeekdayFriday];

                var set = this.CreateBindingSet<StreakTableViewCell, CellWrapperViewModel<ListeningStreak>>();
                set.Bind(StreakLabel).To(v => v.Item).WithConversion<StreakMessageValueConverter>();
                set.Bind(StreakSublabel).To(v => v.Item).WithConversion<PerfectWeekCountValueConverter>();

                set.Bind(MondayView)
                    .For(v => v.Alpha)
                    .To(v => v.Item.ShowMonday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(MondayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.Item)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Monday.ToString());
                set.Bind(MondayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.Item.MondayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(MondayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.Item.Monday)
                    .WithConversion<BorderVisibilityConverter>();

                set.Bind(TuesDayView)
                    .For(v => v.Alpha)
                    .To(v => v.Item.ShowTuesday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(TuesDayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.Item)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Tuesday.ToString());
                set.Bind(TuesdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.Item.TuesdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(TuesdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.Item.Tuesday)
                    .WithConversion<BorderVisibilityConverter>();

                set.Bind(WednesdayView)
                    .For(v => v.Alpha)
                    .To(v => v.Item.ShowWednesday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(WednesdayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.Item)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Wednesday.ToString());
                set.Bind(WednesdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.Item.WednesdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(WednesdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.Item.Wednesday)
                    .WithConversion<BorderVisibilityConverter>();

                set.Bind(ThursdayView)
                    .For(v => v.Alpha)
                    .To(v => v.Item.ShowThursday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(ThursdayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.Item)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Thursday.ToString());
                set.Bind(ThursdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.Item.ThursdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(ThursdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.Item.Thursday)
                    .WithConversion<BorderVisibilityConverter>();

                set.Bind(FridayView)
                    .For(v => v.Alpha)
                    .To(v => v.Item.ShowFriday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(FridayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.Item)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Friday.ToString());
                set.Bind(FridayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.Item.FridayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(FridayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.Item.Friday)
                    .WithConversion<BorderVisibilityConverter>();

                set.Apply();
            });
        }
    }
}