using System;
using BMM.Core.Models.POs.Base;
using MvvmCross.Commands;

namespace BMM.Core.Models.POs
{
    public class StandardIconOptionPO : BasePO
    {
        public StandardIconOptionPO(
            string title,
            string imagePath,
            IMvxCommand clickCommand)
        {
            Title = title;
            ImagePath = imagePath;
            ClickCommand = clickCommand;
        }
        
        public string Title { get; }
        public string ImagePath { get; }
        public IMvxCommand ClickCommand { get; }
    }
}