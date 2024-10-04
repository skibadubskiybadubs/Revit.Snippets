"""
    This script is developed for Dynamo.

    The script accepts a list elements and a list of values.
    Then it iterates over elements and find its Parameters.
    Then it find its parameter that matches the parameter of interest 
    (BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
    and set the value of the parameter to the value in the list.

    In C# I would use group.LookupParameter("ParameterName").Set(value)
    But it requires parameter name, which is not perfect
    when working with different languages.
"""

import clr
clr.AddReference('RevitAPI')
clr.AddReference('RevitServices')
from Autodesk.Revit.DB import *
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager
import Revit

doc = DocumentManager.Instance.CurrentDBDocument

groups = IN[0]
values = IN[1]
g=[]
TransactionManager.Instance.EnsureInTransaction(doc)
# Iterate through groups and assign comments
for group, value in zip(groups, values):
    for param in group.Parameters:
        if param.Id == BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS:
            param.SetValue(str(value))
            g.append(param)
TransactionManager.Instance.TransactionTaskDone()

OUT = g


