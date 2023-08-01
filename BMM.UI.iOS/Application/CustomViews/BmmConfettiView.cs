using BMM.UI.iOS.Constants;
using BMM.UI.iOS.CustomViews.Enums;
using CoreAnimation;
using static System.Math;

namespace BMM.UI.iOS.CustomViews
{
    public class ConfettiView : UIView
    {
        private string[] _availableImages = new[]
        {
            "confetti",
            "diamond",
            "star"
        };
        
        public UIColor[] Colors { get; set; }
        public float Intensity { get; set; }
        public ConfettiType ConfettiType { get; set; }
        public UIImage CustomImage { get; set; }
        private bool Active { get; set; }
        private CAEmitterLayer Emitter { get; set; }

        const float Pi4 = (float) PI / 4;
        const float Pi2 = (float) PI / 2;

        public ConfettiView(NSCoder coder): base(coder)
        {
            Setup();
        }

        public ConfettiView(CGRect frame) : base(frame)
        {
            Setup();
        }

        private void Setup()
        {
            Colors = new[] {
                AppColors.BibleStudyConfettiOneColor,
                AppColors.BibleStudyConfettiTwoColor,
                AppColors.BibleStudyConfettiThreeColor
            };

            Intensity = 0.5F;
            ConfettiType = ConfettiType.Confetti;
            Active = false;
        }

        public void StartConfetti()
        {
            Emitter = new CAEmitterLayer();

            Emitter.Position = new CGPoint(Frame.Size.Width / 2.0, -50);
            Emitter.Shape = "kCAEmitterLayerLine";
            Emitter.Size = new CGSize(Frame.Size.Width, 1);

            var cells = new CAEmitterCell[Colors.Length];
            for (var i = 0; i < Colors.Length; i++)
            {
                cells[i] = ConfettiWithColor(Colors[i]);
            }

            Emitter.Cells = cells;

            Layer.AddSublayer(Emitter);
            Active = true;
        }

        public void StopConfetti()
        {
            if (Emitter != null) Emitter.BirthRate = 0;
            Active = false;
        }

        private CAEmitterCell ConfettiWithColor(UIColor color)
        {
            var confetti = new CAEmitterCell
            {
                BirthRate = 6,
                LifeTime = 14F * Intensity,
                LifetimeRange = 0,
                Color = color.CGColor,
                Velocity = 350.0F * Intensity,
                VelocityRange = 80.0F * Intensity,
                EmissionLongitude = Pi2,
                EmissionRange = Pi4,
                Spin = 3.5F * Intensity,
                SpinRange = 4F * Intensity,
                ScaleRange = Intensity,
                ScaleSpeed = -0.1F * Intensity,
                Contents = GetRandomImage()?.CGImage
            };
            return confetti;
        }

        private UIImage GetRandomImage()
        {
            string randomFilename = _availableImages
                .OrderBy(x => Guid.NewGuid().ToString())
                .First();
            
            return UIImage.FromBundle($"{randomFilename}.png");
        }
    }
}