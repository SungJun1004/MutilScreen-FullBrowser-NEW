
using System;

using CefSharp.WinForms;

using CefSharp;

using System.Drawing;

using System.Timers;

using System.Windows.Forms;

using System.Diagnostics;

using CefSharp.Handler;

using System.Net;
using System.IO;
using static MutilScreen_FullBrowser.MyForm;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.DevTools.DOM;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MutilScreen_FullBrowser

{

    public partial class MyForm : Form

    {

        private static ChromiumWebBrowser browser;
        Rectangle Fullscreen_bounds;
        private System.Timers.Timer aTimer;
        public string strTimeFlag;

        public MyForm()

        {

            InitializeComponent();
            //InitializeCef();
            InitializeCefSharpBrowser();


        }

        // CefSharpBrowserManager 클래스
        public static class CefSharpBrowserManager
        {
            private static ChromiumWebBrowser browser;

            public static void Initialize(ChromiumWebBrowser browser)
            {
                CefSharpBrowserManager.browser = browser;
            }

            public static ChromiumWebBrowser GetBrowser()
            {
                return browser;
            }
        }
        private void InitializeCefSharpBrowser()
        {
          
            CefSettings setting = new CefSettings();
            setting.RemoteDebuggingPort = 8087;

            //캐시 관련 작업 폴더 셋업
            setting.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
            Cef.Initialize(setting);

            browser = new ChromiumWebBrowser(Properties.Settings.Default.strURL);
            browser.Dock = DockStyle.Fill;
            Controls.Add(browser);

            // Register PreviewKeyDown event handler for browser control

            Form2 form2 = new Form2(browser,this);

            browser.KeyboardHandler = new CustomKeyboardHandler(this, form2, aTimer);

            // Request Matthias Add Source

            browser.RequestHandler = new MyBrowserRequestHandler();
            browser.ConsoleMessage += myconsoleEvent;

            CefSharpBrowserManager.Initialize(browser);
            

        }


        private void Form1_Load(object sender, EventArgs e)

        {


            FormFullscreen();
            // InitializeURL();
            // browser.Load("");
            ReloadTimer_Start();

        }

        public void ReloadTimer_Start()
        {
            //타이머 작동 관련 함수 호출

            // nAutoRefreshTime = -1 이면 강제 리프레쉬 안함

            if (Properties.Settings.Default.nAutoRefreshTime > 0)

            {

                strTimeFlag = "Min";
                SetTimer();
                aTimer.Start();

            }
            else if (Properties.Settings.Default.nAutoRefreshTime == 0)

            {

                strTimeFlag = "24H";
                SetTimer();
                aTimer.Start();

            }
            else if (Properties.Settings.Default.nAutoRefreshTime == -1)
            {
                //타이머 작동 안함
            }

        }
        public void ReloadTimer_Stop()
        {
           // aTimer.AutoReset = false;
           // aTimer.Enabled = false;
           if (Properties.Settings.Default.nAutoRefreshTime != -1){
                aTimer.Stop();
                aTimer.Dispose();
                aTimer = null;

            }

            // MessageBox.Show("stop");
        }
        private void FormFullscreen()

        {

            this.FormBorderStyle = FormBorderStyle.None;

            this.StartPosition = FormStartPosition.Manual;



            Fullscreen_bounds = Rectangle.Empty;



            // 멀티 스크린 사이즈 체크

            foreach (var screen in Screen.AllScreens)

            {

                Fullscreen_bounds = Rectangle.Union(Fullscreen_bounds, screen.Bounds);

            }



            // 스크린 사이즈 적용

            this.Size = new Size(Fullscreen_bounds.Width - Properties.Settings.Default.nStartWidth, Fullscreen_bounds.Height);

            this.Location = new Point(Fullscreen_bounds.Left + Properties.Settings.Default.nStartWidth, Fullscreen_bounds.Top);



            //MessageBox.Show("width:" + Fullscreen_bounds.Width.ToString() + "  Height:" + Fullscreen_bounds.Height.ToString());



        }

       
        private  void FormFullscreen_resize()

        {


            //MessageBoxTimeout((System.IntPtr)0, (timeout / 1000).ToString() + "초 후 자동으로 닫힙니다.", "Auto-Close", 0, 0, timeout);
            MessageBox.Show("30초후 창의 사이즈를 재조정 합니다");

            Thread.Sleep(30000);

            Fullscreen_bounds = Rectangle.Empty;



            // 멀티 스크린 사이즈 체크

            foreach (var screen in Screen.AllScreens)

            {

                Fullscreen_bounds = Rectangle.Union(Fullscreen_bounds, screen.Bounds);

            }



            // 스크린 사이즈 적용

            this.Size = new Size(Fullscreen_bounds.Width - Properties.Settings.Default.nStartWidth, Fullscreen_bounds.Height);

            this.Location = new Point(Fullscreen_bounds.Left + Properties.Settings.Default.nStartWidth, Fullscreen_bounds.Top);

           // this.Size = new Size(800, 800);

           // MessageBox.Show("width:" + Fullscreen_bounds.Width.ToString() + "  Height:" + Fullscreen_bounds.Height.ToString());



        }


        public void InitializeURL()

        {

            Form2 OpenCustURL = new Form2(browser,this);
            OpenCustURL.StartPosition = FormStartPosition.CenterScreen;


            OpenCustURL.ShowDialog();

          //  browser.LoadUrl(Form2.URLMessage);

            Debug.WriteLine("Page Load ");

        }


       
        public void SetTimer()

        {
            aTimer = new System.Timers.Timer();


            if (Properties.Settings.Default.nAutoRefreshTime > 0)
            {
                aTimer.Interval = 1000 * 60 * Properties.Settings.Default.nAutoRefreshTime; // 입력한 주기를 기반으로 새로 고침 최소 1분 단위 가능
                MessageBox.Show(Properties.Settings.Default.nAutoRefreshTime.ToString());
            }
            else
            {
                aTimer.Interval = 1000 * 60 ; // 1분당 1회 체크

            }



            // 이벤트 핸들러 연결

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            // Timer에서 Elapsed 이벤트를 반복해서 발생

            aTimer.AutoReset = true;

            aTimer.Enabled = true;

        }


        private void OnTimedEvent(Object source, ElapsedEventArgs e)

        {



            //페이지 리로드 타이머 이벤트

            // MessageBox.Show("페이지 리로드");

            // 시스템 시계의 시간을 가져옵니다.

            DateTime Nowtime = DateTime.Now;



            // 시간이 04시가 되면 이벤트를 발생시킵니다.

            if (strTimeFlag == "24H")

            {

                if (Nowtime.Hour == 04 && Nowtime.Minute == 00) // 04시 00분에 되면 페이지 리로드

                {

                    // 마티어스의 의견에 따라 URLLoad 의 구조의 형태로 구조 변경

                    //browser.LoadUrl(Form2.URLMessage);

                    browser.Reload();

                }

            }
            else if (strTimeFlag == "Min")

            {

                // 마티어스의 의견에 따라 URLLoad 의 구조의 형태로 구조 변경

                //browser.LoadUrl(Form2.URLMessage);


                // 설정한 주기에 따라 페이지 리로드
                browser.Reload();
                MessageBox.Show("checked");

            }


        }


        public class CustomKeyboardHandler : IKeyboardHandler

        {
            private MyForm form1;
            private Form2 form2;
            private System.Timers.Timer atimer;



            public CustomKeyboardHandler(MyForm myfrom, Form2 form22,System.Timers.Timer formTimer)
            {
                form1 = myfrom;
                form2 = form22;
                atimer = formTimer;
            }

            public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)

            {

                // aTimer.Stop();

                if (type == KeyType.RawKeyDown && windowsKeyCode == (int)Keys.F5)

                {

                    // F5 키 누름 처리

                    browser.Reload();

                    return true; // 이벤트를 처리했음을 반환

                }
                else if (type == KeyType.RawKeyDown && windowsKeyCode == (int)Keys.F9)

                {

                    // F10 키 누름 처리

                    if (form1.InvokeRequired)
                    {
                        form1.Invoke(new Action(() =>
                        {
                            // UI 컨트롤에 대한 작업 실행
                            form1.FormFullscreen_resize();
                        }));
                    }
                    else
                    {
                        // 현재 UI 스레드에서 직접 작업 실행
                        form1.FormFullscreen_resize();
                    }

                    //Form2 호출 나중에 추가 처리 하자
                   


                    return true; // 이벤트를 처리했음을 반환

                }

                else if (type == KeyType.RawKeyDown && windowsKeyCode == (int)Keys.F10)

                {

                    // F10 키 누름 처리

                    form1.Invoke((MethodInvoker)delegate
                    {
                        form1.ReloadTimer_Stop();
                     //   MessageBox.Show("이벤트 처리");
                    });

                  


                    // 현재 UI 스레드에서 직접 작업 실행
                  //  form1.ReloadTimeer_Stop(); 
                   
                    form2.Show();

                   
                    return true; // 이벤트를 처리했음을 반환

                }

                else if (type == KeyType.RawKeyDown && windowsKeyCode == (int)Keys.F12)

                {

                    // F12 키 누름 처리

                    browser.GetHost().ShowDevTools();



                    return true; // 이벤트를 처리했음을 반환

                }

                else if (type == KeyType.RawKeyDown && windowsKeyCode == (int)Keys.Escape)

                {

                    //

                    Application.Exit();

                    return true; // 이벤트를 처리했음을 반환

                }

                return false; // 이벤트를 처리하지 않았음을 반환

            }



            public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)

            {

                // throw new NotImplementedException();

                return false;

            }



        }

        /// <summary>

        /// Request Matthias Add Source

        /// </summary>


        public class MyBrowserResourceRequestHandler : ResourceRequestHandler



        {
            public void OnLoadError(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
            {
                System.Diagnostics.Debug.WriteLine("OnError: " + errorCode + "\n " + errorText + "\n" + failedUrl);

             
            }

            protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)

            {

                System.Diagnostics.Debug.WriteLine("OnResourceLoadComplete: " + response.StatusCode + " " + request.Url);


            }

            protected override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
             {


                // System.Diagnostics.Debug.WriteLine("OnResourceLoadComplete: " + response.StatusCode + " " + request.Url);

                // If it is not an OK response, set the error and close the dialog

                /*  if (response.StatusCode == (int)HttpStatusCode.Forbidden)


                  {

                      if (request.Url.Contains("contentlib"))


                      {



                          System.Diagnostics.Debug.WriteLine(response.StatusCode);



                          System.Diagnostics.Debug.WriteLine(request.Url);



                          var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;



                          browser.Reload();



                      }



                  }*/

                return false;

            }
           

        }


        public class MyBrowserRequestHandler : RequestHandler
        {

            protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)

            {

                return new MyBrowserResourceRequestHandler();

            }
        }

        private static void myconsoleEvent(object sender, ConsoleMessageEventArgs e)

        {

            // System.Diagnostics.Debug.WriteLine(e.Message);

           if (e.Level >= LogSeverity.Warning)
            {

                var text_log = " " + e.Level + "\n " + e.Message + "\n" + e.Source;

                sac_Log(text_log);
                
            }

            if (e.Message.Contains("Access Denied"))

            {

                // browser.Reload();
                System.Diagnostics.Debug.WriteLine(e.Message + " DETECTED RELOADING");

            }

        }

        private static string GetDateTime()

        {

            DateTime NowDate = DateTime.Now;

            return NowDate.ToString("yyyy-MM-dd HH:mm:ss") + ":" + NowDate.Millisecond.ToString("000");

        }
        // 로그내용
       
        private static void sac_Log(String msg)

        {

            string FilePath = Directory.GetCurrentDirectory() + @"\Logs\" + "FSApp_" + DateTime.Today.ToString("yyyyMMdd") + ".log";

            string DirPath = Directory.GetCurrentDirectory() + @"\Logs";

            string temp;

            DirectoryInfo di = new DirectoryInfo(DirPath);

            FileInfo fi = new FileInfo(FilePath);

            System.Diagnostics.Debug.WriteLine("DirectoryInfo : " + di);

            System.Diagnostics.Debug.WriteLine("FileInfo : " + fi);


            try

            {

                if (di.Exists != true) Directory.CreateDirectory(DirPath);

                if (fi.Exists != true)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))

                    {

                        temp = string.Format("[{0}] {1}", GetDateTime(), msg);
                        sw.WriteLine(temp);
                        sw.Close();

                    }

                }
                else
                {

                    using (StreamWriter sw = File.AppendText(FilePath))

                    {

                        temp = string.Format("[{0}] {1}", GetDateTime(), msg);
                        sw.WriteLine(temp);
                        sw.Close();

                    }

                }

            }

            catch (Exception)
            {
            }
        }



    }



}


