using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace RunDynamoScript
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string pluginLoc = thisAssemblyPath;

            String tabName = "RunDynamoScript";
            application.CreateRibbonTab(tabName);
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "run");
            
            PushButtonData buttonData = new PushButtonData("Apply", "ApplyParameters", pluginLoc, "RunDynamoScript.RunDynamo");
            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(pluginLoc), "apply.png"));
            BitmapImage bitmapImage = new BitmapImage(uri);
            pushButton.LargeImage = bitmapImage;
            pushButton.ToolTip = "Apply Properties";


            //RibbonController.ShowOptionsBar(null);
            //ShowUtilsBarCommand showUtilsBarCommand = new ShowUtilsBarCommand();

            // PushButtonData okButtonData = new PushButtonData("cmdOk", "終了", pluginLoc, "BimFanParking.OkCommand"); // Ok
            // PushButtonData cancelButtonData = new PushButtonData("cmdCancel", "キャンセル", pluginLoc, "BimFanParking.CancelCommand"); // Cancel

            // IList<RibbonItem> stackedItems = ribbonPanel.AddStackedItems(okButtonData, cancelButtonData);

            // // Store the PushButton objects
            // OkButton = stackedItems[0] as PushButton;
            // CancelButton = stackedItems[1] as PushButton;

            // // Disable the buttons
            // OkButton.Visible = false;
            // CancelButton.Visible = false;

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public void EnableButtons()
        {
            //OkButton.Visible = true;
            //CancelButton.Visible = true;
        }
    }
}
