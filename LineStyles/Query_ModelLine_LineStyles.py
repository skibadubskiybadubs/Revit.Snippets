"""
    Output all possible LineStyles NAMES! in the current document.
    By iterating through all SubCategories of BuiltInCategory.OST_Lines.
    
    CONS: 
    - Autodesk.Revit.DB.Category cannot be assigned to ModellLines.
    - When new LineStyles are added via code, 
    this script will not show the nwely added LineStyles. [Dunno why]
    Whereas it looks like it picks up the LineStyles added via the UI.
    - Also this won't output the LineStyle object, just the name and category.
"""

import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitServices')
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager

doc = DocumentManager.Instance.CurrentDBDocument

# Model Lines
line_styles_category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines)
subcategories = line_styles_category.SubCategories

line_styles_list = []
for line_style in subcategories:
    line_styles_list.append([line_style.Name, line_style])

OUT = line_styles_list

"""
    OUTPUT:
    [
        [
            [
            <Medium Lines>,
            Autodesk.Revit.DB.Category
            ],
            [
            <Axis of Rotation>,
            Autodesk.Revit.DB.Category
            ],
            [
            <Centerline>,
            Autodesk.Revit.DB.Category
            ],
            [
            <Space Separation>,
            Autodesk.Revit.DB.Category
            ],
            [
            <Hidden Lines>,
            Autodesk.Revit.DB.Category
            ],
            [
            <Lines>,
            Autodesk.Revit.DB.Category
            ]
        ]
    ]
"""