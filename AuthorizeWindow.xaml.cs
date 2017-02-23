using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TournamentManagerApi
{
    /// <summary>
    /// Interaction logic for AuthorizeWindow.xaml
    /// </summary>
    public partial class AuthorizeWindow : Window
    {
        private readonly string _clientId;
        private readonly string _scope;

        private string AuthUrl => $"{ApiManager.AUTHORIZE_URI}?client_id={_clientId}" +
                                  $"&redirect_uri={ApiManager.REDIRECT_URI}&response_type=code&scope={_scope}";

        public string Code { get; private set; }

        public AuthorizeWindow(string clientId, string scope) {
            _clientId = clientId;
            _scope = scope;
            InitializeComponent();
            this.Loaded += (object sender, RoutedEventArgs e) => { wbMain.Navigate(AuthUrl); };
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e) {
            if (!e.Url.ToString().Contains(ApiManager.REDIRECT_URI)) return;
            Code = System.Web.HttpUtility.ParseQueryString(e.Url.Query).Get("code");
            DialogResult = true;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {}
    }
}