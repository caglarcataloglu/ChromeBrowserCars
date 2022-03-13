using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChromeBrowserCars
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ChromiumWebBrowser chrome;
        static bool isLoggedIn = false;

        private async void Form1_Load(object sender, EventArgs e) //async evaluatescript icin
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            txtUrl.Text = "https://www.cars.com/signin/?redirect_path=%2F";
            chrome = new ChromiumWebBrowser(txtUrl.Text);
            
            this.pContainer.Controls.Add(chrome);
            chrome.Dock = DockStyle.Fill;
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.LoadingStateChanged += BrowserLoadingStateChangedAsync;         

        }

        private void BrowserLoadingStateChangedAsync(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                if (!isLoggedIn)
                {
                    // enter user and password, then submit the form with class name
                    var loginScript = @"document.querySelector('#email').value = 'johngerson808@gmail.com';
                               document.querySelector('#password').value = 'test8008';
                               document.querySelector('.session-form').submit();";

                    chrome.EvaluateScriptAsync(loginScript).ContinueWith(u =>
                    {                                       
                        isLoggedIn = true;
                        Console.WriteLine("Succesfully Logged In.\n");
                    });
                }
                else 
                {
                    Console.WriteLine("Problem escalated for Loggin Operation.\n");
                }
                var filterScript = @"document.querySelector('#make-model-search-stocktype').value = 'used';
                               document.querySelector('#makes').value = 'tesla';
                               document.querySelector('#makes').dispatchEvent(new Event('change', { 'bubbles': true }));
                               document.querySelector('#models').value = 'tesla-model_s';
                               document.querySelector('#models').dispatchEvent(new Event('change', { 'bubbles': true }));
                               document.querySelector('#make-model-max-price').value = '100000';               
                               document.querySelector('#make-model-maximum-distance').value = 'all';
                               document.querySelector('#make-model-zip').value = '94596';
                               document.querySelector('.sds-button').click();";

                //JavascriptResponse response1 = await chrome.EvaluateScriptAsync("d");
                //var task = chrome.EvaluateScriptAsync("(select() { return document.getElementById('makes').value; })();", "tesla");

                var jsScript = @"

                var allList = document.querySelector('#vehicle-cards-container')
                var carList = allList.getElementsByClassName('vehicle-card-main js-gallery-click-card');
                var jsons = new Array();
                    function replacer(key, value) {
                    return typeof value === 'undefined' ? null : value;
                }
                for (var i = 0; i < carList.length; i++) {       
                    var title = carList[i].getElementsByClassName('title');
                    var mileage = carList[i].getElementsByClassName('mileage');
                    var primary_price = carList[i].getElementsByClassName('primary-price');
                    var dealer_name = carList[i].getElementsByClassName('dealer-name');
                    var rating = carList[i].getElementsByClassName('sds-rating__count');
                    var miles_from = carList[i].getElementsByClassName('miles-from');
                    var badge = carList[i].getElementsByClassName('sds-badge__label');
        
                    if(badge[1]==null)
                        var badge1=undefined
                    else
                        var badge1=badge[1].innerText
                    if(badge[2]==null)
                        var badge2=undefined
                    else
                        var badge2 = badge[2].innerText
                    if(badge[3]==null)
                        var badge3=undefined
                    else
                        var badge3=badge[3].innerText
    
                    var jsonData = JSON.stringify({ 
                            title: title[0].innerHTML, 
                            mileage: mileage[0].innerHTML,
                            primary_price: primary_price[0].innerHTML,
                            dealer_name: dealer_name[0].innerText,
                            rating: rating[0].innerHTML,
                            miles_from: miles_from[0].innerHTML,
                            badge: [badge[0].innerText,badge1,badge2,badge3]
                        }, replacer, '\t', 4);

                    console.log(jsonData);
                    jsons.push(jsonData)
                    jsons.concat('\n')
                } 
                    console.log(jsons);
            ";

                chrome.ExecuteScriptAsyncWhenPageLoaded(filterScript);
                Console.WriteLine("filterScript is executed.\n");

                chrome.EvaluateScriptAsync(jsScript).ContinueWith(u =>
                { Console.WriteLine(u.Result.Message.ToString()); 
                //return bilgileri dosyaya yazilacak.
                }
                );

            }
        }

        private void Chrome_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                txtUrl.Text = e.Address;
            }));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            chrome.Load(txtUrl.Text);
            
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            if (chrome.CanGoBack)
                chrome.Back();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (chrome.CanGoForward)
                chrome.Forward();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            chrome.Reload(true);
        }
    }
}
