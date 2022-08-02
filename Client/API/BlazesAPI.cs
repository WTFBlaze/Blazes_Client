using System.Collections.Generic;
using Blaze.API.QM;
using Blaze.API.SM;

namespace Blaze.Utils.API
{
    internal static class BlazesAPI
    {
        internal const string Identifier = "WTFBlaze";

        internal static List<QMSingleButton> allQMSingleButtons = new();
        internal static List<QMNestedButton> allQMNestedButtons = new();
        internal static List<QMToggleButton> allQMToggleButtons = new();
        internal static List<QMTabButton> allQMTabButtons = new();
        internal static List<QMInfo> allQMInfos = new();
        internal static List<QMSlider> allQMSliders = new();
        internal static List<QMLabel> allQMLabels = new();
        internal static List<SMButton> allSMButtons = new();
        internal static List<SMList> allSMLists = new();
        internal static List<SMText> allSMTexts = new();
        internal static List<SMPopup> allSMPopups = new();
    }
}
