using Amazon;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimsSaves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker _localSavesBackgroundWorker;
        private BackgroundWorker _cloudSavesBackgroundWorker;
        private const string _saveLocation = @"{0}\Electronic Arts\The Sims 3\Saves\";
        private AmazonS3Config _s3Config;
        private AmazonS3Client _s3Client;
        private string _bucketName;
        public MainWindow()
        {
            InitializeComponent();
            _localSavesBackgroundWorker = new BackgroundWorker();
            _cloudSavesBackgroundWorker = new BackgroundWorker();
            _bucketName = ConfigurationManager.AppSettings.Get("S3BucketName");
            _s3Config = new AmazonS3Config() { RegionEndpoint = RegionEndpoint.USEast1 };
            _s3Client = new AmazonS3Client(_s3Config);
            _localSavesBackgroundWorker.DoWork += LoadLocalSaves;
            _cloudSavesBackgroundWorker.DoWork += LoadCloudSaves;
            _cloudSavesBackgroundWorker.RunWorkerAsync();
            _localSavesBackgroundWorker.RunWorkerAsync();
        }

        private void UpdateUi(Action updateAction)
        {
            Application.Current.Dispatcher.Invoke(updateAction);
        }

        private Action AddItem<T>(ListView listView, T item)
        {
            return new Action(() =>
            {
                listView.Items.Add(item);
            });
        }
        private async void LoadCloudSaves(object sender, DoWorkEventArgs e)
        {
            var cloudSaves = await _s3Client.ListObjectsV2Async(new Amazon.S3.Model.ListObjectsV2Request { BucketName = _bucketName });
            foreach(var save in cloudSaves.S3Objects)
            {
                UpdateUi(AddItem(CloudSaveList, new SaveItem()
                {
                    FileName = save.Key,
                    LastChangedTime = save.LastModified.ToLongTimeString()
                }));
            }
        }

        private void LoadLocalSaves(object sender, DoWorkEventArgs e)
        {
            var docsFolderPath = string.Format(_saveLocation,
                                               Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            var saves = Directory.EnumerateDirectories(docsFolderPath);
            foreach (var save in saves)
            {
                UpdateUi(
                    AddItem(LocalSaveList, new SaveItem()
                    {
                        FileName = save,
                        LastChangedTime = Directory.GetLastWriteTime(save).ToLongTimeString()
                    })
                );
            }
        }

        private void CompareLocalToCloud()
        {
            foreach(SaveItem localSave in LocalSaveList.Items)
            {
                if(CloudSaveList.Items.)
            }
        }

        private void SyncSavesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        internal class SaveItem
        {
            public string FileName { get; set; }
            public string LastChangedTime { get; set; }
        }
    }
}
