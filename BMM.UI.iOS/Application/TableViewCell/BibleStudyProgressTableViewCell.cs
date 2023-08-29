using BMM.Core.Models.POs.BibleStudy;
using BMM.Core.Translation;
using BMM.Core.ValueConverters;
using BMM.UI.iOS.Constants;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding;
using MvvmCross.Platforms.Ios.Binding.Views;

namespace BMM.UI.iOS
{
    public partial class BibleStudyProgressTableViewCell : BaseBMMTableViewCell
    {
        public static readonly NSString Key = new(nameof(BibleStudyProgressTableViewCell));

        public BibleStudyProgressTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<BibleStudyProgressTableViewCell, BibleStudyProgressPO>();

                var source = new MvxCollectionViewSource(AchivementsCollectionView, AchievementsCollectionViewCell.Key);
                AchivementsCollectionView!.RegisterNibForCell(AchievementsCollectionViewCell.Nib, AchievementsCollectionViewCell.Key);

                set
                    .Bind(source)
                    .For(s => s.ItemsSource)
                    .To(po => po.Achievements);
                
                set.Bind(ProgressTitleLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_ProgressTitle]);
                
                set.Bind(TermsLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_StreakTerms]);

                set.Bind(DaysLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_Days]);
                
                set.Bind(BoostLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_Boost]);
                
                set.Bind(PointsLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_Points]);
                
                set.Bind(AchiviementsLabel)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_Achievements]);
                
                set.Bind(StreakThisWeek)
                    .To(po => po.TextSource[Translations.BibleStudyViewModel_StreakThisWeek]);
                
                set.Bind(DaysInARow)
                    .To(po => po.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DaysInARowMessageValueConverter>();
                
                set.Bind(DaysNumberLabel)
                    .To(po => po.DaysNumber);
                
                set.Bind(BoostNumberLabel)
                    .To(po => po.BoostNumber);
                
                set.Bind(PointsNumber)
                    .To(po => po.PointsNumber);
                
                set.Bind(MondayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Monday.ToString());
                set.Bind(MondayColorView)
                    .For(v => v.BackgroundColor)
                    .To(po => po.MondayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(MondayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.ListeningStreakPO.ListeningStreak.Monday)
                    .WithConversion<BorderVisibilityConverter>();
                
                set.Bind(TuesdayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Tuesday.ToString());
                set.Bind(TuesdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.TuesdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(TuesdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.ListeningStreakPO.ListeningStreak.Tuesday)
                    .WithConversion<BorderVisibilityConverter>();
                
                set.Bind(WednesdayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Wednesday.ToString());
                set.Bind(WednesdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.WednesdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(WednesdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.ListeningStreakPO.ListeningStreak.Wednesday)
                    .WithConversion<BorderVisibilityConverter>();
                
                set.Bind(ThursdayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Thursday.ToString());
                set.Bind(ThursdayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.ThursdayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(ThursdayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.ListeningStreakPO.ListeningStreak.Thursday)
                    .WithConversion<BorderVisibilityConverter>();
                
                set.Bind(FridayView)
                    .For(v => v.BackgroundColor)
                    .To(vm => vm.ListeningStreakPO.ListeningStreak)
                    .WithConversion<DayOfTheWeekToStreakBackgroundColorConverter>(DayOfWeek.Friday.ToString());
                set.Bind(FridayColorView)
                    .For(v => v.BackgroundColor)
                    .To(v => v.FridayColor)
                    .WithConversion<HexStringToUiColorConverter>();
                set.Bind(FridayColorView.Layer)
                    .For(v => v.BorderWidth)
                    .To(v => v.ListeningStreakPO.ListeningStreak.Friday)
                    .WithConversion<BorderVisibilityConverter>();
                
                set.Bind(TermsButtonView)
                    .For(v => v.BindTap())
                    .To(po => po.TermsButtonClickedCommand);
                
                AchivementsCollectionView.Source = source;
                set.Apply();
            });
        }

        private void SetThemes()
        {
            ProgressTitleLabel.ApplyTextTheme(AppTheme.Heading2);
            TermsLabel.ApplyTextTheme(AppTheme.Subtitle1Label1);
            StreakThisWeek.ApplyTextTheme(AppTheme.Title2);
            DaysInARow.ApplyTextTheme(AppTheme.Subtitle2Label3);
            BoostNumberLabel.ApplyTextTheme(AppTheme.Title1);
            DaysNumberLabel.ApplyTextTheme(AppTheme.Title1);
            PointsNumber.ApplyTextTheme(AppTheme.Title1);
            BoostLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            PointsLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            DaysLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
            AchiviementsLabel.ApplyTextTheme(AppTheme.Subtitle3Label3);
        }

        protected override bool HasHighlightEffect => false;

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            SetThemes();

            MondayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            TuesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            WednesdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            ThursdayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            FridayColorView!.Layer.BorderColor = AppColors.PlaceholderColor.CGColor;
            
            StreakContainer.Layer.ShadowOffset = new CGSize(0, 2);
            StreakContainer.Layer.ShadowRadius = 2;
            StreakContainer.Layer.ShadowOpacity = 0.1f;
            StreakContainer.Layer.MasksToBounds = false;
            StreakContainer.Layer.BorderWidth = 0.5f;
            StreakContainer.Layer.BorderColor = AppColors.GlobalBlackSeparatorColor.CGColor;
        }
    }
}