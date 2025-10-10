using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BedrockEditorPro.Utilities.Constants;

namespace BedrockEditorPro.Utilities
{
    public class EditionTracker
    {
        public static List<string> ETListOfFeatureClasses = new List<string> { Database.FGeoline, Database.FGeopoly, Database.FGeopoint, Database.FLabel };

    }
}
