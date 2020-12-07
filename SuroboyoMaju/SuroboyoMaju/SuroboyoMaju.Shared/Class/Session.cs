using SuroboyoMaju.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuroboyoMaju.Shared.Class
{
    class Session
    {
        private static string tokenAuthorization { get; set; }
        private static int pageState = 0;
        private static int filterState = 0;
        private static int jumlahLaporanState = 0;
        private static User userLogin { get; set; }
        private static HomeNavigationPage pageInstance { get; set; }
        private static HomeNavigationPageKepalaKeamanan homePageKepalaKeamananInstance { get; set; }
        private static FilterParams filterParams { get; set; }
        private static ReportDetailPageParams reportDetailPageParam { get; set; }
        public static ChatPageParams chatPageParam { get; set; }
        private static ConfirmReportParams confirmReportParam { get; set; }
        private static KantorPolisi kantorPolisiSelected { get; set; }
        private readonly static string URL_WEBVIEW = "http://adrian-webview.ta-istts.com/";
        private readonly static string URL_ASSETS = "http://adrian-assets.ta-istts.com/";
        private readonly static string URL_GAMBAR_LAPORAN = "http://adrian-webservice.ta-istts.com/public/uploads/";

        public void setJumlahLaporanState(int jumlahLaporan)
        {
            jumlahLaporanState = jumlahLaporan;
        }

        public int getJumlahLaporanState()
        {
            return jumlahLaporanState;
        }


        public void setPageState(int page)
        {
            pageState = page;
        }

        public int getPageState()
        {
            return pageState;
        }

        public void setHomeNavigationPageKepalaKeamananInstance(HomeNavigationPageKepalaKeamanan page)
        {
            homePageKepalaKeamananInstance = page;
        }

        public HomeNavigationPageKepalaKeamanan getHomePageKepalaKeamananInstance()
        {
            return homePageKepalaKeamananInstance;
        }

        public void setHomeNavigationPageInstance(HomeNavigationPage page)
        {
            pageInstance = page;
        }

        public HomeNavigationPage getHomeNavigationPageInstance()
        {
            return pageInstance;
        }

        public FilterParams getFilterParams()
        {
            return filterParams;
        }

        public int getFilterState()
        {
            return filterState;
        }

        public void setFilterState(int state)
        {
            filterState = state;
        }

        public void setFilterParams(FilterParams param)
        {
            filterParams = param;
        }

        public ChatPageParams getChatPageParams()
        {
            return chatPageParam;
        }

        public void setChatPageParams(ChatPageParams param)
        {
            chatPageParam = param;
        }

        public void setReportDetailPageParams(ReportDetailPageParams param)
        {
            reportDetailPageParam = param;
        }

        public ReportDetailPageParams getReportDetailPageParams()
        {
            return reportDetailPageParam;
        }

        public ConfirmReportParams getConfirmReportParams()
        {
            return confirmReportParam;
        }

        public void setConfirmreportParam(ConfirmReportParams param)
        {
            confirmReportParam = param;
        }


        public void setTokenAuthorization(string token)
        {
            tokenAuthorization = token;
        }

        public string getTokenAuthorization()
        {
            return tokenAuthorization;
        }

        public void setUserLogin(User user)
        {
            userLogin = user;
        }

        public User getUserLogin()
        {
            return userLogin;
        }

        public KantorPolisi getKantorPolisi()
        {
            return kantorPolisiSelected;
        }
        public void setKantorPolisi(KantorPolisi kantorPolisi)
        {
            kantorPolisiSelected = kantorPolisi;
        }

        public string getUrlGambarLaporan()
        {
            return URL_GAMBAR_LAPORAN;
        }

        public string getUrlWebView()
        {
            return URL_WEBVIEW;
        }

        public string getUrlAssets()
        {
            return URL_ASSETS;
        }
    }
}
