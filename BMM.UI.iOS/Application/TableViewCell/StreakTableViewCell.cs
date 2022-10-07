using BMM.Api.Implementation.Models;
using BMM.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Foundation;
using System;
using BMM.Core.Models.POs.ListeningStreakPO;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using MvvmCross.Platforms.Ios.Binding;

namespace BMM.UI.iOS
{
    public partial class StreakTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(StreakTableViewCell));

        public StreakTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                MondayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
                TuesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
                WednesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
                ThursdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
                FridayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;

                var set = this.CreateBindingSet<StreakTableViewCell, ListeningStreakPO>();
                set.Bind(StreakLabel).To(v => v.ListeningStreak).WithConversion<StreakMessageValueConverter>();
                set.Bind(StreakSublabel).To(v => v.ListeningStreak).WithConversion<StreakSubtitleMessageValueConverter>();

                set.Bind(this)
                    .For(v => v.BindTap())
                    .To(po => po.ListeningStreakClickedCommand);
                
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

                set.Bind(TuesDayView)
                    .For(v => v.Alpha)
                    .To(v => v.ListeningStreak.ShowTuesday)
                    .WithConversion<BoolToNfloatConverter>();
                set.Bind(TuesDayView)
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

                set.Apply();
            });
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            StreakLabel.ApplyTextTheme(AppTheme.Title2AutoSize);
            StreakSublabel.ApplyTextTheme(AppTheme.Subtitle2Label3AutoSize);
        }
    }
}