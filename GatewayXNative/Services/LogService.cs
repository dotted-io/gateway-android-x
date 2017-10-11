using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using GatewayXNative.Models;
using Newtonsoft.Json;

namespace GatewayXNative.Services
{
    [Service]
    public class LogService : Service
    {
        public List<DotLog> DotLogs;
        public List<Position> Positions;

        public static LogService instance = null;

        public static LogService SharedInstance()
        {
            if (instance == null)
            {
                instance = new LogService();
            }
            return instance;
        }

        public LogService()
        {
            DotLogs = new List<DotLog>(100);
            Positions = new List<Position>(100);
        }

        public void Add(DotLog log)
        {
            DotLogs.Add(log);
        }

        public void Add(Position log)
        {
            Positions.Add(log);
        }

        public string GetSerializedDotLogsForPosting()
        {
            List<DotLog> dotLogs = DotLogs.Select(x => x).ToList();
            DotLogs.Clear();

            return JsonConvert.SerializeObject(dotLogs);
        }

        public string GetSerializedPositionsForPosting()
        {
            List<Position> positionLogs = Positions.Select(x => x).ToList();
            Positions.Clear();

            return JsonConvert.SerializeObject(positionLogs);
        }

        public async Task PostLogs(List<DotLog> dotLogs, List<Position> positionLogs)
        {
            List<Object> messages = new List<Object>(dotLogs.Count() + positionLogs.Count());
            foreach(DotLog d in dotLogs)
            {
                DotLogMinified dm = new DotLogMinified(d);
                WorkerMessage wm = new WorkerMessage("baseLog", dm);

                messages.Add(wm);
            }

            foreach (Position p in positionLogs)
            {
                PositionMinified pm = new PositionMinified(p);
                WorkerMessage wm = new WorkerMessage("positionLog", pm);

                messages.Add(wm);
            }

            await AWSService.SharedInstance().SendBatchMessagesAsync(messages);

            StopSelf();
        }

        // Magical code that makes the service do wonderful things.
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // This method executes on the main thread of the application.
            string dotLogsSerialized = intent.GetStringExtra("dotLogs");
            List<DotLog> dotLogs = JsonConvert.DeserializeObject<List<DotLog>>(dotLogsSerialized);

            string positionsSerialized = intent.GetStringExtra("positions");
            List<Position> positions = JsonConvert.DeserializeObject<List<Position>>(positionsSerialized);

            PostLogs(dotLogs, positions);

            return StartCommandResult.NotSticky;
        }
    }

}
