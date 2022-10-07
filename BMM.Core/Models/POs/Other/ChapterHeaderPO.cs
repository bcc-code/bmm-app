using BMM.Api.Implementation.Models;
using BMM.Core.Models.POs.Base;

namespace BMM.Core.Models.POs.Other
{
    public class ChapterHeaderPO : DocumentPO
    {
        public ChapterHeaderPO(ChapterHeader chapterHeader) : base(chapterHeader)
        {
            ChapterHeader = chapterHeader;
        }
        
        public ChapterHeader ChapterHeader { get; }
    }
}