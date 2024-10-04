"""
    This script is developed for Dynamo.

    The script accepts a list of lists of elements (IN[0])
    Then it iterates over each group in the project, 
    and retreives the elements IDs in each found group.
    Then it compares the input element IDs with the found group element IDs.

    Its a very heavy and non-optimized process,
    But for my project with two groups and 2 elements in each group,
    it works like a charm. Computes within milliseconds.
"""

import clr
import System
clr.AddReference('System.Core')
clr.AddReference("System")
from System.Collections.Generic import *
clr.AddReference('RhinoInside.Revit')
clr.AddReference('RevitAPI') 
clr.AddReference('RevitAPIUI')
clr.AddReference('RevitServices')
import Revit
#from System.Collections.Generic import List
import RevitServices
from RevitServices.Persistence import DocumentManager
from RevitServices.Transactions import TransactionManager
from System import Enum
from Autodesk.Revit import DB
from Autodesk.Revit.DB import *
#doc = Revit.ActiveDBDocument

doc = DocumentManager.Instance.CurrentDBDocument

model_lines_groups = IN[0]

# check if a group already exists
def group_exists(group_elements):
    collector = FilteredElementCollector(doc).OfClass(Group)
    for group in collector:
        group_members = [memberId.Value for memberId in group.GetMemberIds()]
        group_elements_ids = [DB.ElementId(elem.Id).Value for elem in model_lines_group]
        if sorted(group_members) == sorted(group_elements_ids):
            return group
    return None

try:
    TransactionManager.Instance.EnsureInTransaction(doc)
        
    # Create groups or get existing groups
    output_groups = []
    for model_lines_group in model_lines_groups:
        existing_group = group_exists(model_lines_group)
        if existing_group:
            output_groups.append(existing_group)
        else:
            ids = [DB.ElementId(elem.Id) for elem in model_lines_group]
            element_ids = List[DB.ElementId](ids)
            new_group = doc.Create.NewGroup(element_ids)
            new_group.GroupType.Name = "Cross-Section Rotation Group"
            output_groups.append(new_group)
    
    TransactionManager.Instance.TransactionTaskDone()
    OUT = output_groups

except Exception as e:
    OUT = str(e)