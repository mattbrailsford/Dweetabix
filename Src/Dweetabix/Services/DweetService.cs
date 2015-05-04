using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dweetabix.Helpers;
using Dweetabix.Models;

namespace Dweetabix.Services
{
    public class DweetService : IDisposable
    {
        public event EventHandler<DweetEventArgs> DweetReceived;

        private WebClient _webClient;

        private string _thing;
        private int _timeout;

        public DweetService(string thing, int timeout)
        {
            _thing = thing;
            _timeout = timeout;

            _webClient = new WebClient();
        }

        public void ListenForDweets()
        {
            LogHelper.Info("Connecting to dweet.io...");

            _webClient.OpenReadCompleted += dweet_OpenReadCompleted;
            _webClient.OpenReadAsync(new Uri("https://dweet.io/listen/for/dweets/from/" + _thing));
            
            // If web client is busy, then connection is open
            if(_webClient.IsBusy)
                LogHelper.Success("Connected to dweet.io");
        }

        protected void dweet_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw e.Error;

                using (var reader = new StreamReader(e.Result))
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            var ln = reader.ReadLine();

                            // Starts with a quote so assume it's a JSON encoded string
                            if (ln.StartsWith("\""))
                            {
                                ln = JsonConvert.DeserializeObject<string>(ln);
                            }

                            // Json object so parse as a dweet
                            if (ln.StartsWith("{") || ln.StartsWith("["))
                            {
                                var dweet = JsonConvert.DeserializeObject<Dweet>(ln);
                                NotifyDweet(dweet);
                            }

                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                var webEx = ex as WebException;
                if (webEx != null && webEx.Status == WebExceptionStatus.ConnectionClosed)
                {
                    LogHelper.Info("Dweet connection timed out.");
                }
                else if(webEx != null && webEx.Status == WebExceptionStatus.RequestCanceled)
                {
                    // If request is canceled, just exit without reconnecting
                    // LogHelper.Info("Dweet connection closed.");
                    return;
                }

                StopListeningForDweets();
                ListenForDweets();
            }
        }

        public void StopListeningForDweets()
        {
            _webClient.OpenReadCompleted -= dweet_OpenReadCompleted;
            _webClient.CancelAsync();
        }

        public void Update()
        { }

        protected void NotifyDweet(Dweet dweet)
        {
            if (DweetReceived != null)
                DweetReceived(this, new DweetEventArgs(dweet));
        }

        public void Dispose()
        {
            StopListeningForDweets();

            _webClient.Dispose();
        }
    }

    #region EventArgs
	
	public class DweetEventArgs : EventArgs
    {
        public DweetEventArgs(Dweet dweet)
	    {
            Dweet = dweet;
        }

        public Dweet Dweet { get; set; }
    }
 
	#endregion
}
