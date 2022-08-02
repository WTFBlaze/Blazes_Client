using Blaze.API.QM;
using Blaze.API.SM;
using Blaze.API.Tags;
using System.Collections.Generic;

namespace Blaze.API
{
    public static class BlazesAPI
    {
        public const string Identifier = "WTFBlaze";

        public static List<QMSingleButton> allQMSingleButtons = new();
        public static List<QMNestedButton> allQMNestedButtons = new();
        public static List<QMToggleButton> allQMToggleButtons = new();
        public static List<QMTabButton> allQMTabButtons = new();
        public static List<QMInfo> allQMInfos = new();
        public static List<QMSlider> allQMSliders = new();
        public static List<QMLabel> allQMLabels = new();
        public static List<SMButton> allSMButtons = new();
        public static List<SMList> allSMLists = new();
        public static List<SMText> allSMTexts = new();
        public static List<SMPopup> allSMPopups = new();
        public static List<Tag> allTags = new();
    }
}
