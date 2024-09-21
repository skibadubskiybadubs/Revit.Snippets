"""
    Output all possible LineStyles (GraphicStyle elements) in the current document.
    By iterating through all SubCategories of BuiltInCategory.OST_Lines.
    
    PROS:
    + GraphicsStyle element can be directly assigned to ModellLines.

    CONS: 
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

# Get the LineStyles category
line_styles_category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines)
subcategories = line_styles_category.SubCategories

# Collect all GraphicsStyle elements
collector = FilteredElementCollector(doc).OfClass(GraphicsStyle).WhereElementIsNotElementType().ToElements()

line_styles_list = []
subcategory_ids = [subcategory.Id for subcategory in subcategories]

for gs in collector:
    if gs.GraphicsStyleCategory.Id in subcategory_ids:
        line_styles_list.append([gs.Name, gs])

OUT = line_styles_list

"""
    OUTPUT:
    [
        [
            [
            <Room Separation>,
            Autodesk.Revit.DB.GraphicsStyle
            ],
            [
            <Sketch>,
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
            ]
        ]
    ]
"""