"""
    This script creates new LineStyles in the Lines category of the active Revit document.
    It searches for a LinePatternElement by name, and if not found, creates a new one.
    Then it creates new LineStyles with the specified names, and assigns the LinePatternElement to them.

    USAGE:
    Application.cs:
    using Autodesk.Revit.DB.Events;
    namespace MetalFarm
    {
        public class Application : IExternalApplication
        {
            public Result OnStartup(UIControlledApplication application) //when starting Revit session
            {
                #region OnDocOpened/OnDocCreated
                application.ControlledApplication.DocumentOpened += OnDocOpened;
                application.ControlledApplication.DocumentCreated += OnDocCreated;
                #endregion
                return Result.Succeeded;
            }
            public Result OnShutdown(UIControlledApplication application) // when closing Revit session
            {
                #region OnDocOpened/OnDocCreated
                application.ControlledApplication.DocumentOpened -= OnDocOpened;
                application.ControlledApplication.DocumentCreated -= OnDocCreated;
                #endregion
                return Result.Succeeded;
            }
            public void OnDocOpened(object sender, DocumentOpenedEventArgs args) { // when a document IS opened
                //Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
                Document doc = args.Document;
                //MessageBox.Show($"Document is opened", $"Document is opened");
                #region LineStyles
                string message = string.Empty;
                CreateLineStyle createLineStyle = new CreateLineStyle();
                createLineStyle.CreateLineStyles(doc, ref message);
                #endregion
            }
            public void OnDocCreated(object sender, DocumentCreatedEventArgs args) // when a new document IS created
            {
                //Autodesk.Revit.ApplicationServices.Application app = (Autodesk.Revit.ApplicationServices.Application)sender;
                Document doc = args.Document;
                //MessageBox.Show("Document is created", "Document is created");
                #region LineStyles
                string message = string.Empty;
                CreateLineStyle createLineStyle = new CreateLineStyle();
                createLineStyle.CreateLineStyles(doc, ref message);
                #endregion
            }
        }
    }
"""

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Linq;

namespace MetalFarm
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class CreateLineStyle : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            MessageBox.Show($"Creating ModelLine", $"Creating ModelLine");
            return CreateLineStyles(doc, ref message);
        }

        public Result CreateLineStyles(Document doc, ref string message)
        {
            // List of new LineStyle names
            List<string> lineStyleNames = new List<string>
            {
                "C1", "T1", "梁成", "ラチ", "G1", "B1", "B2", "B3", "B4", "P1", "P2", "Ga", "Gc"
            };

            // LineStyle to be found or created
            // Use "Solid" for solid line pattern, or any other name to create custom pattern (lstSegments)
            string linePatternName = "Solid"; 

            // Find existing line pattern element or create a new one if not found
            Category lineCat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines); // Get the Lines category
            ElementId linePatternId = FindOrCreateLinePatternElement(doc, linePatternName);
            if (linePatternId == ElementId.InvalidElementId)
            {
                message = $"{linePatternName} LinePattern element could not be found or created";
                MessageBox.Show($"{linePatternName} LinePattern element could not be found or created", "Error");
                return Result.Failed;
            }

            // Check existing LineStyle names (subcategories) // don't create linestyles if they already exist
            HashSet<string> existingLineStyles = new HashSet<string>();
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle));
            foreach (GraphicsStyle style in collector)
            {
                if (style.GraphicsStyleCategory.Parent != null && style.GraphicsStyleCategory.Parent.Id == lineCat.Id)
                {
                    existingLineStyles.Add(style.Name);
                }
            }

            // Create LineStyles in transaction
            using (Transaction trans = new Transaction(doc, "Create Line Styles"))
            {
                try
                {
                    trans.Start();

                    foreach (string name in lineStyleNames)
                    {
                        if (!existingLineStyles.Contains(name)) // don't create linestyles if they already exist
                        {
                            CreateNewLineStyle(doc, lineCat, linePatternId, name);
                        }
                        // else
                        // {
                        //     MessageBox.Show($"Line style '{name}' already exists. Skipping creation.", "Info");
                        // }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = $"Transaction failed: {ex.Message}";
                    MessageBox.Show($"Transaction failed: {ex.Message}", "Error");
                    trans.RollBack();
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }

        // Find/Create LinePatternElement
        private ElementId FindOrCreateLinePatternElement(Document doc, string patternName)
        {
            if (patternName == "Solid")
            {
                return LinePatternElement.GetSolidPatternId();
            }

            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(LinePatternElement));

            // Find linePattern by Name
            foreach (LinePatternElement elem in collector.Cast<LinePatternElement>())
            {
                if (elem.Name == patternName)
                {
                    return elem.Id;
                }
            }

            // If not found, create a new linePattern
            LinePatternElement linePatternElement = CreateLinePatternElement(doc, patternName);
            return linePatternElement?.Id ?? ElementId.InvalidElementId;
        }

        private LinePatternElement CreateLinePatternElement(Document doc, string patternName)
        {
            // Create list of segments which define the line pattern
            List<LinePatternSegment> lstSegments = new List<LinePatternSegment>
            {
                new LinePatternSegment(LinePatternSegmentType.Dot, 0.0),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02),
                new LinePatternSegment(LinePatternSegmentType.Dash, 0.03),
                new LinePatternSegment(LinePatternSegmentType.Space, 0.02)
            };

            LinePattern linePattern = null;

            // Create actual linePattern element
            try
            {
                linePattern = new LinePattern(patternName);
                linePattern.SetSegments(lstSegments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create linePattern: {ex.Message}", "Error");
                return null;
            }

            using (Transaction trans = new Transaction(doc, "Create a linepattern element"))
            {
                try
                {
                    trans.Start();
                    LinePatternElement linePatternElement = LinePatternElement.Create(doc, linePattern);
                    trans.Commit();
                    return linePatternElement;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create linePattern element: {ex.Message}", "Error");
                    trans.RollBack();
                    return null;
                }
            }
        }

        // Create LineStyle
        private void CreateNewLineStyle(Document doc, Category lineCat, ElementId linePatternId, string name)
        {
            Category newLineStyleCat = doc.Settings.Categories.NewSubcategory(lineCat, name);
            newLineStyleCat.SetLineWeight(8, GraphicsStyleType.Projection);
            newLineStyleCat.LineColor = new Color(255, 0, 0);
            newLineStyleCat.SetLinePatternId(linePatternId, GraphicsStyleType.Projection);
        }
    }
}