using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Dynamo.Applications;
using Dynamo.Core;
using RevitServices;
using static System.Net.Mime.MediaTypeNames;
using Document = Autodesk.Revit.DB.Document;

namespace RunDynamoScript
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RunDynamo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string pluginLoc = thisAssemblyPath;

            //Get application and documnet objects and start transaction
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            //string Journal_Dynamo_Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\script.dyn");
            string Journal_Dynamo_Path = Path.Combine(Path.GetDirectoryName(pluginLoc), "script.dyn");
            DynamoRevit dynamoRevit = new DynamoRevit();

            DynamoRevitCommandData dynamoRevitCommandData = new DynamoRevitCommandData();
            dynamoRevitCommandData.Application = commandData.Application;
            IDictionary<string, string> journalData = new Dictionary<string, string>
            {
                { Dynamo.Applications.JournalKeys.ShowUiKey, false.ToString() }, // don't show DynamoUI at runtime
                { Dynamo.Applications.JournalKeys.AutomationModeKey, true.ToString() }, //run journal automatically
                { Dynamo.Applications.JournalKeys.DynPathKey, Journal_Dynamo_Path }, //run node at this file path
                { Dynamo.Applications.JournalKeys.DynPathExecuteKey, true.ToString() }, // The journal file can specify if the Dynamo workspace opened from DynPathKey will be executed or not. If we are in automation mode the workspace will be executed regardless of this key.
                { Dynamo.Applications.JournalKeys.ForceManualRunKey, false.ToString() }, // don't run in manual mode
                { Dynamo.Applications.JournalKeys.ModelShutDownKey, true.ToString() },
                { Dynamo.Applications.JournalKeys.ModelNodesInfo, false.ToString() }

            };

            dynamoRevitCommandData.JournalData = journalData;
            Result externalCommandResult = dynamoRevit.ExecuteCommand(dynamoRevitCommandData);
            return externalCommandResult;

        }
    }
}
