using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LFFSSK.API;

namespace LFFSSK.View
{
    /// <summary>
    /// Interaction logic for MediaPlayerView.xaml
    /// </summary>
    public partial class MediaPlayerView2 : UserControl
    {

        const string TraceCategory = "MediaPlayerView";

        private Timer imageTimer;
        int currentIndex = 0;
        public DoubleAnimation fadeOut { get; set; }
        public DoubleAnimation fadeOutFadeIn { get; set; }
        public DoubleAnimation fadeIn { get; set; }

        private Media currentMedia;
        public MediaPlayerView2()
        {
            InitializeComponent();

            fadeOut = new DoubleAnimation(0d, new TimeSpan(0, 0, 0, 0, 300));
            fadeOutFadeIn = new DoubleAnimation(0d, new TimeSpan(0, 0, 0, 0, 300));
            fadeIn = new DoubleAnimation(1d, new TimeSpan(0, 0, 0, 0, 300));

            fadeOut.Completed += fadeOut_Completed;
            fadeOutFadeIn.Completed += fadeOutFadeIn_Completed;
            fadeIn.Completed += fadeIn_Completed;

            if (imageTimer != null)
            {
                imageTimer.Elapsed -= imageTimer_Elapsed;
                imageTimer = null;
            }

            imageTimer = new Timer(10000);
            imageTimer.AutoReset = false;
            imageTimer.Elapsed += imageTimer_Elapsed;

            StartMediaPlayer();
        }

        public void StartBannerMenu()
        {
            try
            {
                //fadeOut = new DoubleAnimation(0d, new TimeSpan(0, 0, 0, 0, 300));
                //fadeOutFadeIn = new DoubleAnimation(0d, new TimeSpan(0, 0, 0, 0, 300));
                //fadeIn = new DoubleAnimation(1d, new TimeSpan(0, 0, 0, 0, 300));

                //fadeOut.Completed += fadeOut_Completed;
                //fadeOutFadeIn.Completed += fadeOutFadeIn_Completed;
                //fadeIn.Completed += fadeIn_Completed;

                //if (imageTimer != null)
                //{
                //    imageTimer.Elapsed -= imageTimer_Elapsed;
                //    imageTimer = null;
                //}

                //imageTimer = new Timer(10000);
                //imageTimer.AutoReset = false;
                //imageTimer.Elapsed += imageTimer_Elapsed;

                //StartMediaPlayer();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] StartBanner = {0}", ex.ToString()), TraceCategory);
            }
        }

        public void StopBannerMenu()
        {
            if (imageTimer != null)
            {
                imageTimer.Elapsed -= imageTimer_Elapsed;
            }
        }

        private void StartMediaPlayer()
        {
            Action showMedia = new Action(() =>
            {
                currentMedia = GetNextMedia();
                ShowMedia();
            });

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(showMedia));
            t.Start();
        }

        private void videoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            videoPlayer.Visibility = System.Windows.Visibility.Visible;
        }

        private void videoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoPlayer.Source = null;

            currentMedia = GetNextMedia();
            ShowMedia();
        }

        private void videoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[ERROR] videoPlayer_MediaFailed: {0} [Filename = {1}]", e.ErrorException.ToString(), currentMedia.Source), TraceCategory);
            System.Threading.Thread.Sleep(1000);

            videoPlayer.Source = null;

            currentMedia = GetNextMedia();
            ShowMedia();
        }

        private void fadeIn_Completed(object sender, EventArgs e)
        {
            imageTimer.Start();
        }

        private void fadeOutFadeIn_Completed(object sender, EventArgs e)
        {
            imagePreview.Source = null;
            imagePreview.Source = GetImage(currentMedia.Source);
            imagePreview.BeginAnimation(ImageBrush.OpacityProperty, fadeIn);
        }

        private void fadeOut_Completed(object sender, EventArgs e)
        {
            videoPlayer.MediaEnded -= videoPlayer_MediaEnded;
            videoPlayer.MediaOpened -= videoPlayer_MediaOpened;
            videoPlayer.MediaFailed -= videoPlayer_MediaFailed;

            videoPlayer.Source = new Uri(currentMedia.Source, UriKind.Absolute);

            videoPlayer.MediaEnded += videoPlayer_MediaEnded;
            videoPlayer.MediaOpened += videoPlayer_MediaOpened;
            videoPlayer.MediaFailed += videoPlayer_MediaFailed;

            videoPlayer.Play();
        }

        private void imageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentMedia = GetNextMedia();
            ShowMedia();
        }

        private void ShowMedia()
        {
            try
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.Invoke((Action)(() => { ShowMedia(); }));
                    return;
                }
                //if (currentMedia == null) return;
                if (currentMedia.MediaType == MediaType.Video)
                {
                    if (videoPlayer.Visibility != System.Windows.Visibility.Visible)
                        imagePreview.BeginAnimation(ImageBrush.OpacityProperty, fadeOut);
                    else
                    {
                        videoPlayer.MediaEnded -= videoPlayer_MediaEnded;
                        videoPlayer.MediaOpened -= videoPlayer_MediaOpened;
                        videoPlayer.MediaFailed -= videoPlayer_MediaFailed;

                        videoPlayer.Source = new Uri(currentMedia.Source, UriKind.Absolute);

                        videoPlayer.MediaEnded += videoPlayer_MediaEnded;
                        videoPlayer.MediaOpened += videoPlayer_MediaOpened;
                        videoPlayer.MediaFailed += videoPlayer_MediaFailed;

                        videoPlayer.Play();
                    }
                }
                else
                {
                    if (videoPlayer.Visibility != System.Windows.Visibility.Visible)
                        imagePreview.BeginAnimation(ImageBrush.OpacityProperty, fadeOutFadeIn);
                    else
                    {
                        videoPlayer.Visibility = System.Windows.Visibility.Hidden;

                        imagePreview.Source = GetImage(currentMedia.Source);
                        imagePreview.BeginAnimation(ImageBrush.OpacityProperty, fadeIn);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[ERROR] ShowMedia: {0}", ex.ToString()), TraceCategory);
            }
        }

        private Media GetNextMedia()
        {
            try
            {

                int index = 0;

                if (!System.IO.Directory.Exists(GeneralVar.TopBannerRepository))
                {
                    System.Threading.Thread.Sleep(60000);
                    return GetNextMedia();
                }

                //if (GeneralVar.MainWindowVM == null) return null;
                DirectoryInfo mediaDirectory = new DirectoryInfo(GeneralVar.TopBannerRepository);
                FileInfo[] AllFile = mediaDirectory.GetFiles("*.*", SearchOption.AllDirectories);


                //List<string> apiFileNames = new List<string>();
                int FileCount = AllFile.Count();
                
                    //apiFileNames = GeneralVar.MainWindowVM.BannerList.Select(banner => banner.FileName).ToList();
                    //FileCount = GeneralVar.MainWindowVM.BannerList.Select(banner => banner.FileName).Count();
                

                //apiFileNames = GeneralVar.MainWindowVM.BannerList.Select(banner => banner.FileName).ToList();
                //FileCount = GeneralVar.MainWindowVM.BannerList.Select(banner => banner.FileName).Count();

                FileInfo[] files = new FileInfo[FileCount]; ;

                    //foreach (var image in apiFileNames)
                    //{
                    //    files[index] = AllFile.Where(x => x.FullName.Contains(image)).Single();
                    //    index++;
                    //}

                foreach (var image in AllFile)
                {
                    files[index] = AllFile.Where(x => x.FullName.Contains(image.ToString())).Single();
                    index++;
                }

                if (files.Length == 0)
                {
                    System.Threading.Thread.Sleep(60000);
                    return GetNextMedia();
                }

                files = files.OrderBy(f => f.Name).ToArray();
                while (true)
                {
                    if (currentIndex >= files.Length)
                        currentIndex = 0;

                    Media media = new Media(files[currentIndex]);
                    currentIndex += 1;

                    if (media.MediaType != MediaType.Other)
                        return media;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[ERROR] ShowMedia: {0}", ex.ToString()), TraceCategory);
                System.Threading.Thread.Sleep(1000);

                return GetNextMedia();
            }
        }

        private BitmapImage GetImage(string source)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(source, UriKind.Absolute);
                image.EndInit();

                if (image.CanFreeze)
                    image.Freeze();

                return image;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[ERROR] GetImage: {0}", ex.ToString()), TraceCategory);
                return null;
            }
        }

        public enum MediaType
        {
            Image,
            Video,
            Other
        }

        private class Media
        {
            private readonly string[] imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp", };
            private readonly string[] videoExtensions = new string[] { ".avi", ".mp4", ".mkv", ".wmv", };

            public Media(FileInfo file)
            {
                Source = file.FullName;
                MediaType = GetMediaType(file.Extension);
            }

            public string Source { get; set; }
            public MediaType MediaType { get; set; }

            private MediaType GetMediaType(string extension)
            {
                if (imageExtensions.Any(e => e.Equals(extension.ToLower())))
                    return MediaPlayerView2.MediaType.Image;

                else if (videoExtensions.Any(e => e.Equals(extension.ToLower())))
                    return MediaPlayerView2.MediaType.Video;

                else return MediaPlayerView2.MediaType.Other;
            }
        }
    }
}
