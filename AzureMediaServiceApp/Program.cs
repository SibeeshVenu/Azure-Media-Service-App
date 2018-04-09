using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace AzureMediaServiceApp
{
    class Program
    {
        #region Constants 
        private static readonly string mediaServicesAccountName = ConfigurationManager.AppSettings["MediaServicesAccountName"];
        private static readonly string mediaServicesAccountKey = ConfigurationManager.AppSettings["MediaServicesAccountKey"];
        private static readonly string myAzureCon = ConfigurationManager.ConnectionStrings["myAzureStorageCon"].ConnectionString;
        private static MediaServicesCredentials _mediaServiceCredentials = null;
        #endregion
        static void Main(string[] args)
        {
            string input = string.Empty;
            _mediaServiceCredentials = new MediaServicesCredentials(mediaServicesAccountName, mediaServicesAccountKey);
            GetAllTheAssetsAndFiles(_mediaServiceCredentials);
            Console.WriteLine("Enter the asset name to be created...");
            input = Console.ReadLine();
            IAsset asset = CreateBLOBContainer(input, _mediaServiceCredentials);
            UploadImages(asset, _mediaServiceCredentials);
        }

        private static void GetAllTheAssetsAndFiles(MediaServicesCredentials _medServCredentials)
        {
            try
            {
                string result = string.Empty;
                CloudMediaContext mediaContext;
                mediaContext = new CloudMediaContext(_medServCredentials);
                StringBuilder myBuilder = new StringBuilder();
                foreach (var item in mediaContext.Assets)
                {
                    myBuilder.AppendLine(Environment.NewLine);
                    myBuilder.AppendLine("--My Assets--");
                    myBuilder.AppendLine("Name: " + item.Name);
                    myBuilder.AppendLine("++++++++++++++++++++");

                    foreach (var subItem in item.AssetFiles)
                    {
                        myBuilder.AppendLine("File Name: "+subItem.Name);
                        myBuilder.AppendLine("Size: " + subItem.ContentFileSize);
                        myBuilder.AppendLine("++++++++++++++++++++++");
                    }
                }
                Console.WriteLine(myBuilder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IAsset CreateBLOBContainer(string containerName, MediaServicesCredentials _medServCredentials)
        {
            try
            {
                string result = string.Empty;
                CloudMediaContext mediaContext;
                mediaContext = new CloudMediaContext(_medServCredentials);
                IAsset asset = mediaContext.Assets.Create(containerName, AssetCreationOptions.None);
                return asset;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string UploadImages(IAsset asset, MediaServicesCredentials _medServCredentials)
        {
            try
            {
                string _singleInputFilePath = Path.GetFullPath(@"E:\X7Md4VB.JPG");
                CloudMediaContext mediaContext;
                mediaContext = new CloudMediaContext(_medServCredentials);
                var fileName = Path.GetFileName(_singleInputFilePath);               
                var assetFile = asset.AssetFiles.Create(fileName);
                var policy = mediaContext.AccessPolicies.Create("policy for upload", TimeSpan.FromMinutes(30), AccessPermissions.Read | AccessPermissions.Write | AccessPermissions.List);
                var locator = mediaContext.Locators.CreateSasLocator(asset, policy, DateTime.UtcNow.AddDays(1));
                assetFile.Upload(_singleInputFilePath);
                return "Success!";
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
