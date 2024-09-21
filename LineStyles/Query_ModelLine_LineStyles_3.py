"""
    Output all possible LineStyles (GraphicStyle elements) in the current document.
    By iterating through all SubCategories of BuiltInCategory.OST_Lines.
    
    PROS:
    + GraphicsStyle element can be directly assigned to ModellLines.
    + Finds newely added LineStyles via code (Both Detail Line and Model Line).

    CONS: 
    - There are duplicate LineStyle.Name(s) in the list, but different IDs.
"""

import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitServices')
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

doc = DocumentManager.Instance.CurrentDBDocument

# Collect all GraphicsStyle elements
collector = FilteredElementCollector(doc).OfClass(GraphicsStyle)

line_styles_list = []
for style in collector:
    if style.GraphicsStyleCategory.Parent != None and style.GraphicsStyleCategory.Parent.Name == "Lines":
        line_styles_list.append([style.Name, style])

OUT = line_styles_list

"""
    OUTPUT:
    [
        [
            [
            <--New Model LineStyle>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <--New Detail LineStyle>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Medium Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Thin Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Wide Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Lines>,
            Autodesk.Revit.DB.GraphicsStyle
            ]
        ]
    ]
"""