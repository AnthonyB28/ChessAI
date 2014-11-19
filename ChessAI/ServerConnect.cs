using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;

namespace ChessAI
{
    class ServerConnect
    {
        
        static void MakeRequest()
        {
            Uri serviceUri = new Uri("http://www.bencarle.com/chess/poll/GAMEID/TEAMNUMBER/TEAMSECRET/");
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(RequestComplete);
            downloader.OpenReadAsync(serviceUri);
        }

        static private void RequestComplete(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Stream responseStream = e.Result;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JSONResponse));
                JSONResponse response = (JSONResponse)serializer.ReadObject(responseStream);
                if(response.ready)
                {
                    // Our turn
                }
                else
                {
                    // Opponent's turn
                }
            }
        }

        class JSONResponse
        {
            public bool ready { get; set;}
            public float secondsleft { get; set; }
            public int lastMoveNumber { get; set; }
            public string lastMove { get; set; }
        }
    }
}
