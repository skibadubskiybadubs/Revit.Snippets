"""
    Creates a new Model Line LineStyle (Autodesk.Revit.DB.GraphicsStyle)
    in the active Revit document.

    PROS: 
    + Can be assigned to both ModelLines and DetailLines
    + Can be accessed via code: Query_ModelLine_LineStyles_3.py
    + Visible in UI

    CONS:
    - Can't be accessed by code: Query_ModelLine_LineStyles_1/2.py
    - Can't be accessed by Dynamo's Line Pattern node.
    - Maybe will have difficulties implementing in C# (?) because of that.
"""

import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitServices')
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager
doc = DocumentManager.Instance.CurrentDBDocument


TransactionManager.Instance.EnsureInTransaction(doc)

# Get the LineStyles category
line_styles_category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines)

# Create a new subcategory for the new LineStyle
new_line_style_name = "--New Model LineStyle"
new_line_style_category = doc.Settings.Categories.NewSubcategory(line_styles_category, new_line_style_name)

# LineSTyle properties
new_line_style_category.SetLineWeight(5, GraphicsStyleType.Projection)
new_line_style_category.LineColor = Color(0, 0, 255)  # Red color

# # Create and assign a new LinePatternElement (something is odd here)
# line_pattern = LinePatternElement.Create(doc, 1)
# line_pattern.GetLinePattern().Segments.Add(LinePatternSegment(LinePatternSegmentType.Dash, 0.5))
# line_pattern.GetLinePattern().Segments.Add(LinePatternSegment(LinePatternSegmentType.Space, 0.25))
# new_line_style_category.SetLinePatternId(line_pattern.Id, GraphicsStyleType.Projection)


TransactionManager.Instance.TransactionTaskDone()
OUT = new_line_style_category
# Autodesk.Revit.DB.Category