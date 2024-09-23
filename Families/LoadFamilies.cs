"""
    This script will load all families from a specified folder into the project, and output them as a list of Family objects.

    1. Retrieve all family names in the document
    2. Check if the family is already in the project, and return it if it is
    3. If not: Check if the family folder exists
    4. Get all family files in the folder
    5. Check if the folder contains any family files
    6. Load the family that matches the family name, and return it if it is 

    USAGE:
    // Globals.FamilyFolder is a string variable that contains the path to the folder containing the families.
    List<string> searchigFamilies = new List<string>("Family1", "Family2", "Family3");
    LoadFamilies loadFamilies = new LoadFamilies();
    List<Family> loadedFamilies = loadFamilies.LoadAllFamilies(doc, searchigFamilies);

"""


using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Linq;

namespace MetalFarm
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class LoadFamilies : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string familyName, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            return Result.Succeeded;
        }

        public List<Family> LoadAllFamilies(Document doc, List<string> familyNames)
        {
            List<Family> loadedFamilies = new List<Family>();
            List<string> failedFamilies = new List<string>();

            foreach (string familyName in familyNames)
            {
                Family family = LoadFamily(doc, familyName);
                if (family != null)
                {
                    loadedFamilies.Add(family);
                }
                else
                {
                    failedFamilies.Add(familyName);
                }
            }

            if (failedFamilies.Count > 0)
            {
                string failedFamiliesMessage = string.Join(", ", failedFamilies);
                MessageBox.Show($"The following families were not found or failed to load: {failedFamiliesMessage}", "Error");
            }

            return loadedFamilies;
        }

        public Family LoadFamily(Document doc, string familyName)
        {
            // 1. Retrieve all family names in the document
            HashSet<string> familyNamesInDoc = GetFamilyNamesInDocument(doc);

            // 2. Check if the family is already in the project
            if (familyNamesInDoc.Contains(familyName))
            {
                MessageBox.Show($"Family '{familyName}' is already in the project.", "Info");
                return GetFamilyByName(doc, familyName);
            }

            string familyFolder = Globals.FamilyFolder;
            // 3. Check if the family folder exists
            if (!Directory.Exists(familyFolder))
            {
                MessageBox.Show($"/Family folder '{familyFolder}' does not exist.", "Error");
                return null;
            }

            // 4. Get all family files in the folder
            string[] familyFiles = Directory.GetFiles(familyFolder, "*.rfa");
            // 5. Check if the folder contains any family files
            if (familyFiles.Length == 0)
            {
                MessageBox.Show($"No family files found in the /Family folder '{familyFolder}'.", "Error");
                return null;
            }

            // 6. Load the family that matches the family name
            bool familyFound = false;
            foreach (string familyFile in familyFiles)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(familyFile);

                if (fileNameWithoutExtension.Equals(familyName, StringComparison.OrdinalIgnoreCase))
                {
                    familyFound = true;
                    using (Transaction trans = new Transaction(doc, "Load Family"))
                    {
                        try
                        {
                            trans.Start();

                            // Load the family
                            Family family;
                            if (doc.LoadFamily(familyFile, out family))
                            {
                                MessageBox.Show($"Family '{familyName}' loaded successfully.", "Info");
                                trans.Commit();
                                return family;
                            }
                            else
                            {
                                MessageBox.Show($"Failed to load family '{familyName}'.", "Error");
                                trans.RollBack();
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Transaction failed: {ex.Message}", "Error");
                            trans.RollBack();
                            return null;
                        }
                    }
                }
            }
            if (!familyFound)
            {
                MessageBox.Show($"Family '{familyName}' not found in the /Family folder.", "Error");
                return null;
            }

            return null;
        }

        private HashSet<string> GetFamilyNamesInDocument(Document doc)
        {
            HashSet<string> familyNames = new HashSet<string>();

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                familyNames.Add(family.Name);
            }

            return familyNames;
        }

        private Family GetFamilyByName(Document doc, string familyName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                if (family.Name.Equals(familyName, StringComparison.OrdinalIgnoreCase))
                {
                    return family;
                }
            }

            return null;
        }
    }
}