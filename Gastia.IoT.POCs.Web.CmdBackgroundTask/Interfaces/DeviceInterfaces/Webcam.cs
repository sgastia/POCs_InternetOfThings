using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Gastia.IoT.POCs.Web.CmdBackgroundTask.Interfaces.DeviceInterfaces
{
    internal class Webcam
    {
        private MediaCapture mediaCapture;
        private StorageFile photoFile;
        private StorageFile recordStorageFile;
        private StorageFile audioFile;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        private readonly string VIDEO_FILE_NAME = "video.mp4";
        private readonly string AUDIO_FILE_NAME = "audio.mp3";
        private bool isPreviewing;
        private bool isRecording;

        /// <summary>
        /// 'Initialize Audio and Video' button action function
        /// Dispose existing MediaCapture object and set it up for audio and video
        /// Enable or disable appropriate buttons
        /// - DISABLE 'Initialize Audio and Video' 
        /// - DISABLE 'Start Audio Record'
        /// - ENABLE 'Initialize Audio Only'
        /// - ENABLE 'Start Video Record'
        /// - ENABLE 'Take Photo'
        /// </summary>
        public async Task<string> InitVideo()
        {
            //TODO: Disable all buttons until initialization completes

            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    if (isPreviewing)
                    {
                        await mediaCapture.StopPreviewAsync();
                        isPreviewing = false;
                    }
                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        isRecording = false;
                    }
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                Debug.WriteLine("Initializing camera to capture audio and video...");
                
                // Use default initialization
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                // Set callbacks for failure and recording limit exceeded
                string message = "Device successfully initialized for video recording!";
                mediaCapture.Failed += new MediaCaptureFailedEventHandler(mediaCapture_Failed);
                mediaCapture.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(mediaCapture_RecordLimitExceeded);
                Debug.WriteLine(message);


                /*
                // Start Preview                
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
                string message = "{\"message\":\"Camera preview succeeded\"}";
                Debug.WriteLine(message);
                */

                return message;
                
            }
            catch (Exception ex)
            {
                string message = "Unable to initialize camera for audio/video mode: " + ex.Message;
                Debug.WriteLine(message);
                return message;
            }
        }

        /// <summary>
        /// 'Take Photo' button click action function
        /// Capture image to a file in the default account photos folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>The physical path of the taken photo</returns>
        public async Task<string> TakePhoto(StorageFolder folder)
        {
            try
            {
                photoFile = await folder.CreateFileAsync(PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
                Debug.WriteLine("Take Photo succeeded: " + photoFile.Path);

                //IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                //Falta ver si esto sirve para la web tambien

                return photoFile.Name;
            }
            catch (Exception ex)
            {
                string message = "Error in Webcam.TakePhoto(): " + ex.Message;
                Debug.WriteLine(message);
                Cleanup();
                return message;
            }

        }

        /// <summary>
        /// 'Initialize Audio Only' button action function
        /// Dispose existing MediaCapture object and set it up for audio only
        /// Enable or disable appropriate buttons
        /// - DISABLE 'Initialize Audio Only' 
        /// - DISABLE 'Start Video Record'
        /// - DISABLE 'Take Photo'
        /// - ENABLE 'Initialize Audio and Video'
        /// - ENABLE 'Start Audio Record'        
        /// </summary>
        public async Task<string> InitAudioOnly()
        {
            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    if (isPreviewing)
                    {
                        await mediaCapture.StopPreviewAsync();
                        isPreviewing = false;
                    }
                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        isRecording = false;
                    }
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                Debug.WriteLine("Initializing camera to capture audio only...");
                mediaCapture = new MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio;
                settings.MediaCategory = Windows.Media.Capture.MediaCategory.Other;
                settings.AudioProcessing = Windows.Media.AudioProcessing.Default;
                await mediaCapture.InitializeAsync(settings);

                // Set callbacks for failure and recording limit exceeded
                Debug.WriteLine("Device successfully initialized for audio recording!" + "\nPress \'Start Audio Record\' to record");
                mediaCapture.Failed += new MediaCaptureFailedEventHandler(mediaCapture_Failed);
                mediaCapture.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(mediaCapture_RecordLimitExceeded);

                //TODO: Enable buttons for audio


                //TODO: Enable Audio and video Only Init button


                return "Audio initialized";
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to initialize camera for audio mode: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 'Start Video Record' button click action function
        /// Button name is changed to 'Stop Video Record' once recording is started
        /// Records video to a file in the default account videos folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void RecordVideo()
        {
            /*
            try
            {
                takePhoto.IsEnabled = false;
                recordVideo.IsEnabled = false;
                playbackElement.Source = null;

                if (recordVideo.Content.ToString() == "Start Video Record")
                {
                    takePhoto.IsEnabled = false;
                    status.Text = "Initialize video recording";
                    String fileName;
                    fileName = VIDEO_FILE_NAME;

                    recordStorageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                    status.Text = "Video storage file preparation successful";

                    MediaEncodingProfile recordProfile = null;
                    recordProfile = MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.Auto);

                    await mediaCapture.StartRecordToStorageFileAsync(recordProfile, recordStorageFile);
                    recordVideo.IsEnabled = true;
                    recordVideo.Content = "Stop Video Record";
                    isRecording = true;
                    status.Text = "Video recording in progress... press \'Stop Video Record\' to stop";
                }
                else
                {
                    takePhoto.IsEnabled = true;
                    status.Text = "Stopping video recording...";
                    await mediaCapture.StopRecordAsync();
                    isRecording = false;

                    var stream = await recordStorageFile.OpenReadAsync();
                    playbackElement.AutoPlay = true;
                    playbackElement.SetSource(stream, recordStorageFile.FileType);
                    playbackElement.Play();
                    status.Text = "Playing recorded video" + recordStorageFile.Path;
                    recordVideo.Content = "Start Video Record";
                }
            }
            catch (Exception ex)
            {
                if (ex is System.UnauthorizedAccessException)
                {
                    status.Text = "Unable to play recorded video; video recorded successfully to: " + recordStorageFile.Path;
                    recordVideo.Content = "Start Video Record";
                }
                else
                {
                    status.Text = ex.Message;
                    Cleanup();
                }
            }
            finally
            {
                recordVideo.IsEnabled = true;
            }
            */
        }

        /// <summary>
        /// 'Start Audio Record' button click action function
        /// Button name is changes to 'Stop Audio Record' once recording is started
        /// Records audio to a file in the default account video folder
        /// </summary>
        public async void RecordAudio()
        {
            /*
            recordAudio.IsEnabled = false;
            playbackElement3.Source = null;

            try
            {
                if (recordAudio.Content.ToString() == "Start Audio Record")
                {
                    audioFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync(AUDIO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);

                    status.Text = "Audio storage file preparation successful";

                    MediaEncodingProfile recordProfile = null;
                    recordProfile = MediaEncodingProfile.CreateM4a(Windows.Media.MediaProperties.AudioEncodingQuality.Auto);

                    await mediaCapture.StartRecordToStorageFileAsync(recordProfile, audioFile);

                    isRecording = true;
                    recordAudio.IsEnabled = true;
                    recordAudio.Content = "Stop Audio Record";
                    status.Text = "Audio recording in progress... press \'Stop Audio Record\' to stop";
                }
                else
                {
                    status.Text = "Stopping audio recording...";

                    await mediaCapture.StopRecordAsync();

                    isRecording = false;
                    recordAudio.IsEnabled = true;
                    recordAudio.Content = "Start Audio Record";

                    var stream = await audioFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    status.Text = "Playback recorded audio: " + audioFile.Path;
                    playbackElement3.AutoPlay = true;
                    playbackElement3.SetSource(stream, audioFile.FileType);
                    playbackElement3.Play();
                }
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
                Cleanup();
            }
            finally
            {
                recordAudio.IsEnabled = true;
            }
            */
        }


        private async void Cleanup()
        {
            if (mediaCapture != null)
            {
                // Cleanup MediaCapture object
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                    isPreviewing = false;
                }
                if (isRecording)
                {
                    await mediaCapture.StopRecordAsync();
                    isRecording = false;
                }
                mediaCapture.Dispose();
                mediaCapture = null;
            }
        }

        /// <summary>
        /// Callback function for any failures in MediaCapture operations
        /// </summary>
        /// <param name="currentCaptureObject"></param>
        /// <param name="currentFailure"></param>
        private async void mediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine("MediaCaptureFailed: " + currentFailure.Message);

                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        Debug.WriteLine("\n Recording Stopped");
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    Debug.WriteLine("\nCheck if camera is diconnected. Try re-launching the app\n");
                }
            });
        }

        /// <summary>
        /// Callback function if Recording Limit Exceeded
        /// </summary>
        /// <param name="currentCaptureObject"></param>
        private async void mediaCapture_RecordLimitExceeded(Windows.Media.Capture.MediaCapture currentCaptureObject)
        {
            try
            {
                if (isRecording)
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            Debug.WriteLine("Stopping Record on exceeding max record duration");
                            await mediaCapture.StopRecordAsync();
                            isRecording = false;
                            if (mediaCapture.MediaCaptureSettings.StreamingCaptureMode == StreamingCaptureMode.Audio)
                            {
                                Debug.WriteLine("Stopped record on exceeding max record duration: " + audioFile.Path);
                            }
                            else
                            {
                                Debug.WriteLine("Stopped record on exceeding max record duration: " + recordStorageFile.Path);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error: " + e.Message);
                            throw e;
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: " + e.Message);
                throw e;
            }
        }
    }
}
