using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
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

        private bool CanDownloadFile()
        {
            if (FileUrl == null || FileLocation == null)
                return false;
            return true;
        }

        //public ICommand DownloadFile { get { return new RelayCommand(DownloadFileExecute, CanDownloadFile);  } }
        //private void DownloadFileExecute()
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        client.DownloadFile(FileUrl, FileLocation);
        //    }
        //}

        public ICommand Browse { get { return new RelayCommand(BrowseExecute);} }
        public void BrowseExecute()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.DefaultExt = ".mp4";
            dlg.Filter = "Videos (.mp4)|*.mp4";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                FileLocation = filename;
            }
        }

        Process process = new Process();

        public ICommand CaptureStart { get { return new RelayCommand(CaptureStartExecute, CanDownloadFile); } }
        private void CaptureStartExecute()
        {
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.StartInfo.FileName = @"ffmpeg\ffmpeg.exe";

            var compressedArg = String.Format(" -i {0} -vcodec copy -an {1}", FileUrl, FileLocation);
            var uncompressedArg = String.Format(" -i {0} -t 00:00:30 -c:v libx264 {1}", FileUrl, FileLocation);

            if (IsCompressed)
                process.StartInfo.Arguments = compressedArg;
            //"ffmpeg -i 'rtsp://root:pass@172.16.1.200/profile2'  -vcodec copy -an test.mp4";
            else
                process.StartInfo.Arguments = uncompressedArg;
            //"ffmpeg -i 'rtsp://root:pass@172.16.1.200/profile2' -t 00:00:30 -c:v libx264 test.mp4";

            process.Start();
        }

        public ICommand CaptureStop { get { return new RelayCommand(CaptureStopExecute, CanDownloadFile); } }

        private void CaptureStopExecute()
        {
            if (!process.HasExited)
                process.Kill();
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
