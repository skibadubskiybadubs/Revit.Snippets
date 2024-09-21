### Referenced: https://thebuildingcoder.typepad.com/blog/2016/10/how-to-create-a-new-line-style.html

"""
    Creates a new Detail Line LineStyle (Autodesk.Revit.DB.GraphicsStyle)
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

# Find existing linestyle
fec = FilteredElementCollector(doc).OfClass(LinePatternElement)
linePatternElem = next((lp for lp in fec if lp.Name == "Long dash"), None)

if linePatternElem:
    # The new linestyle will be a subcategory of the Lines category
    categories = doc.Settings.Categories
    lineCat = categories.get_Item(BuiltInCategory.OST_Lines)

    TransactionManager.Instance.EnsureInTransaction(doc)

    # Add the new linestyle
    newLineStyleCat = categories.NewSubcategory(lineCat, "--New Detail LineStyle")

    # linestyle properties 
    newLineStyleCat.SetLineWeight(8, GraphicsStyleType.Projection)
    newLineStyleCat.LineColor = Color(255, 0, 0)
    newLineStyleCat.SetLinePatternId(linePatternElem.Id, GraphicsStyleType.Projection)

    TransactionManager.Instance.TransactionTaskDone()
else:
    raise Exception("Line pattern 'Long dash' not found.")

OUT = newLineStyleCat
# Autodesk.Revit.DB.Category