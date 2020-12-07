using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SuroboyoMaju.Shared.Pages;
using SuroboyoMaju.Shared.Class;
using Windows.UI.Popups;
#if __ANDROID__
using Com.OneSignal.Abstractions;
using Com.OneSignal;
#endif

namespace SuroboyoMaju
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        public App()
        {
            ConfigureFilters(global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

            this.InitializeComponent();
            this.Suspending += OnSuspending;
#if __ANDROID__
            OneSignal.Current.StartInit("bba49751-5018-421d-82f5-fe83cc866ce6").HandleNotificationOpened(HandleNotificationOpened)
              .Settings(new Dictionary<string, bool>() {
                { IOSSettings.kOSSettingsKeyAutoPrompt, false },
                { IOSSettings.kOSSettingsKeyInAppLaunchURL, false } })
              .InFocusDisplaying(OSInFocusDisplayOption.Notification)
              .EndInit();
#endif
        }

#if __ANDROID__
        private static void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            Session session = new Session();
            OSNotificationPayload payload = result.notification.payload;
            Dictionary<string, object> additionalData = payload.additionalData;
            if (additionalData != null)
            {
                //HomeNavigationPage homePage = session.getHomeNavigationPageInstance();
                Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;
                string page = additionalData["page"].ToString();
                if (page == "1")
                {
                    int id_user_pelapor = Convert.ToInt32(additionalData["id_user_pelapor"].ToString());
                    string nama_user_pelapor = additionalData["nama_user_pelapor"].ToString();
                    string id_laporan = additionalData["id_laporan"].ToString();
                    string alamat_laporan = additionalData["alamat_laporan"].ToString();
                    string tanggal_laporan = additionalData["tanggal_laporan"].ToString();
                    string waktu_laporan = additionalData["waktu_laporan"].ToString();
                    string judul_laporan = additionalData["judul_laporan"].ToString();
                    string jenis_laporan = additionalData["jenis_laporan"].ToString();
                    string deskripsi_laporan = additionalData["deskripsi_laporan"].ToString();
                    string lat_laporan = additionalData["lat_laporan"].ToString();
                    string lng_laporan = additionalData["lng_laporan"].ToString();
                    string tag = additionalData["tag"].ToString();
                    string thumbnail_gambar = additionalData["thumbnail_gambar"].ToString();
                    int status_laporan = Convert.ToInt32(additionalData["status_laporan"].ToString());
                    int? jumlah_konfirmasi = 0;
                    if (tag == "kriminalitas"){
                        jumlah_konfirmasi = Convert.ToInt32(additionalData["jumlah_konfirmasi"].ToString());
                    }
                    else{
                        jumlah_konfirmasi = null;
                    }
                    ReportDetailPageParams param = new ReportDetailPageParams(id_user_pelapor, nama_user_pelapor, id_laporan, alamat_laporan, tanggal_laporan, waktu_laporan, judul_laporan, jenis_laporan, deskripsi_laporan, lat_laporan, lng_laporan, tag, thumbnail_gambar, status_laporan, jumlah_konfirmasi);
                    session.setReportDetailPageParams(param);
                    rootFrame.Navigate(typeof(ReportDetailPage));
                }else if (page == "2")
                {
                    int id_chat = Convert.ToInt32(additionalData["id_chat"].ToString());
                    int id_user_penerima = Convert.ToInt32(additionalData["id_user_penerima"].ToString());
                    int id_user_pengirim = Convert.ToInt32(additionalData["id_user_pengirim"].ToString());
                    string nama_display = additionalData["nama_display"].ToString();
                    ChatPageParams param = new ChatPageParams(id_chat, id_user_penerima, id_user_pengirim, nama_display);
                    session.setChatPageParams(param);
                    rootFrame.Navigate(typeof(PersonalChatPage));
                }
            }
        }
#endif

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif
            Frame rootFrame = Windows.UI.Xaml.Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(SplashScreenPage), e.Arguments);
                }
                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        /// <summary>
        /// Configures global logging
        /// </summary>
        /// <param name="factory"></param>
        static void ConfigureFilters(ILoggerFactory factory)
        {
            factory
                .WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
                )
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
                .AddConsole(LogLevel.Information);
#endif
        }
    }
}
