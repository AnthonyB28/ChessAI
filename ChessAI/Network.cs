using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Timers;

/* 
 * Start game = http://www.bencarle.com/chess/startgame
 * Team 1 32c68cae
 * Team 2 1a77594c
 * 
 * Poll = http://bencarle.com/chess/poll/GAME_ID/TEAM_NUMBER/TEAM_SECRET/
 * Make Moves = http://www.bencarle.com/chess/move/GAMEID/TEAMNUMBER/TEAMSECRET/MOVESTRING/
 */

namespace ChessAI
{
    class Network
    {
        const int gameID = 1283;
        const int teamID = 1;
        const string teamKey = "32c68cae";
        static readonly string pollingServer = "http://www.bencarle.com/chess/poll/" + gameID + "/" + teamID + "/" + teamKey + "/";
        // Append move string with trailing "/" to make a move
        static readonly string moveServerPrefix = "http://www.bencarle.com/chess/move/"+gameID+"/"+teamID+"/"+teamKey+"/";
        static bool receivedResponse = false;
        static JSONPollResponse lastResponse = null;

        // FOR TEST
        const int teamID2 = 2;
        const string teamKey2 = "1a77594c";
        

        // Makes a request to the server which will be completed in the callback of RequestComplete
        public static JSONPollResponse MakePoll()
        {
            
            Uri pollingServerURI = new Uri(pollingServer);
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(PollCompleted);
            downloader.OpenReadAsync(pollingServerURI);
            while(!receivedResponse)
            {

            }
            receivedResponse = true;
            return lastResponse;
        }

        // Callback of MakeRequest when server response is received
        static private void PollCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Received Response");
                Stream responseStream = e.Result;
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JSONPollResponse));
                JSONPollResponse response = (JSONPollResponse)serializer.ReadObject(responseStream);
                // ready is our turn, not ready is opponent's turn
                lastResponse = response;
                receivedResponse = true;
            }
        }

        public static void MakeMove(string move)
        {
            String moveAddress = moveServerPrefix+move+"/";
            Uri moveServerURI = new Uri(moveAddress);
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(MoveCompleted);
            downloader.OpenReadAsync(moveServerURI);
        }

        static private void MoveCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Received Move Response");
            }
        }

        [DataContract]
        public class JSONPollResponse
        {
            [DataMember]
            public bool ready { get; set;}
            [DataMember]
            public float secondsleft { get; set; }
            [DataMember]
            public int lastMoveNumber { get; set; }
            [DataMember]
            public string lastMove { get; set; }
        }
    }
}
