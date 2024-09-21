"""
    Output all possible LineStyles in the current document.
    By iterating through the LinePatternElement class of BuiltInCategory.OST_GenericLines.
    
    PROS: 
    + LinePatternElement can be directly assigned to DetailLines.

    CONS:
    - LinePatternElement cannot be assigned to ModellLines. (its glitchy) 
    - When new LineStyles are added via code, 
    this script will not show the nwely added LineStyles. [Dunno why]
    Whereas it looks like it picks up the LineStyles added via the UI.
"""

import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitServices')
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

doc = DocumentManager.Instance.CurrentDBDocument

# Detail Lines
collector = FilteredElementCollector(doc);
fi = ElementCategoryFilter(BuiltInCategory.OST_GenericLines, True)
collection = collector.OfClass(LinePatternElement).WherePasses(fi) .ToElements()

line_styles_list = []
for line_style in collection:
    line_styles_list.append([line_style.Name, line_style])

OUT = line_styles_list


"""
    OUTPUT:
    [
        [
            [
            Center 6mm,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            Dash Dot 4.5mm,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            Dash Dot Dot 9mm,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            一点破線,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            二点破線,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            点線,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            長い破線,
            Autodesk.Revit.DB.LinePatternElement
            ],
            [
            オーバーヘッド,
            Autodesk.Revit.DB.LinePatternElement
            ]
        ]
    ]
"""