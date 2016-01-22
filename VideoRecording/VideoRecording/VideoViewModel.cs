using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

namespace VideoRecording
{
    class VideoViewModel : INotifyPropertyChanged
    {
        private readonly Video _video;
        public VideoViewModel()
        {
            _video = new Video();
        }
        public string FileUrl
        {
            get { return _video.FileUrl; }
            set
            {
                _video.FileUrl = value;
                OnPropertyChanged("FileUrl");
            }
        }
        public string FileLocation
        {
            get { return _video.FileLocation; }
            set
            {
                _video.FileLocation = value;
                OnPropertyChanged("FileLocation");
            }
        }
        public bool IsCompressed
        {
            get { return _video.IsCompressed; }
            set
            {
                _video.IsCompressed = value;
                OnPropertyChanged("IsCompressed");
            }
        }

        public ICommand DownloadFile { get { return new RelayCommand(DownloadFileExecute);  } }
        public void DownloadFileExecute()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(FileUrl, FileLocation);
            }
        }

        public ICommand Browse { get { return new RelayCommand(BrowseExecute);} }
        public void BrowseExecute()
        {
            // Create OpenFileDialog
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                FileLocation = filename;
            }
        }

        private void Compression()
        {
            if (IsCompressed)
                Process.Start("cmd", "ffmpeg -i 'rtsp://root:pass@172.16.1.200/profile2'  -vcodec copy -an test.mp4");
            else
                Process.Start("cmd", "ffmpeg -i 'rtsp://root:pass@172.16.1.200/profile2' -t 00:00:30 -c:v libx264 test.mp4");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
