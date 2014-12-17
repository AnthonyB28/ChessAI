using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

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
    /// <summary>
    /// Handles connection to Carle's server.
    /// </summary>
    class Network
    {
        private JSONPollResponse lastResponse;
        private string moveServerPrefix;
        private string teamKey;
        private string pollingServer;
        private int gameID;
        private int teamID;
        private volatile bool receivedResponse;
        private volatile bool moveSending;

        public Network(int gameID, int teamID, string teamKey)
        {
            this.gameID = gameID;
            this.teamID = teamID;
            this.teamKey = teamKey;
            pollingServer = "http://www.bencarle.com/chess/poll/" + gameID + "/" + teamID + "/" + teamKey + "/";
            moveServerPrefix = "http://www.bencarle.com/chess/move/" + gameID + "/" + teamID + "/" + teamKey + "/";
            receivedResponse = false;
            lastResponse = null;
            moveSending = false;
        }

        /// <summary>
        /// Attempts to poll server and returns response set by callback asynchronously.
        /// </summary>
        /// <returns></returns>
        public JSONPollResponse RequestPoll()
        {
            receivedResponse = false;
            Poll();
            while(!receivedResponse)
            {

            }
            receivedResponse = true;
            return lastResponse;
        }

        /// <summary>
        /// Makes a request to the server with a callback
        /// </summary>
        private void Poll()
        {
            Uri pollingServerURI = new Uri(pollingServer);
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(PollCompleted);
            downloader.OpenReadAsync(pollingServerURI);
        }

        /// <summary>
        /// Callback of MakeRequest when server response is received.
        /// If fails, will reattempt to Poll()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PollCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            bool error = false;
            if (e.Error == null)
            {
                Console.WriteLine("Received Response");
                Stream responseStream = e.Result;
                try
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JSONPollResponse));
                    JSONPollResponse response = (JSONPollResponse)serializer.ReadObject(responseStream);
                    // ready is our turn, not ready is opponent's turn
                    lastResponse = response;
                    receivedResponse = true;
                }
                catch 
                {
                    error = true;
                }
            }
            else
            {
                error = true;
            }
            WebClient s = sender as WebClient;
            s.CancelAsync();
            if(error)
            {
                Console.WriteLine("Error receiving poll response. Retrying.");
                Poll();
            }
        }

        /// <summary>
        /// Send the server our move.
        /// </summary>
        /// <param name="move">chess syntax move to make</param>
        public void MakeMove(string move)
        {
            String moveAddress = moveServerPrefix+move+"/";
            Console.WriteLine(moveAddress);
            Uri moveServerURI = new Uri(moveAddress);
            WebClient downloader = new WebClient();
            moveSending = true;
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(MoveCompleted);
            downloader.OpenReadAsync(moveServerURI);
            while (moveSending)
            {

            }
        }

        /// <summary>
        /// Callback for a valid move from server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Received Move Response");
                moveSending = false;
                WebClient s = sender as WebClient;
                s.CancelAsync();
            }
        }

        /// <summary>
        /// JSON response structure from the server
        /// </summary>
        [DataContract]
        public class JSONPollResponse
        {
            [DataMember]
            public bool ready { get; set;}
            [DataMember]
            public float secondsleft { get; set; }
            [DataMember]
            public int lastmovenumber { get; set; }
            [DataMember]
            public string lastmove { get; set; }
            [DataMember]
            public bool? gameover { get; set; }
        }
    }
}
