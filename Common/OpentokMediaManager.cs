using OpenTokSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Common
{
    public static class OpentokMediaManager
    {
        public static string GetOpentokSession()
        {
            var OpenTok = new OpenTok(GlobalVariables.OpenTokApiKey, GlobalVariables.OpenTokApiSecret);
            var session = OpenTok.CreateSession();
            return session.Id;
        }
          
        public static double GetExpirationTime()
        {
            return (DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        //Appointments Sessions
        public static string GetOpentokVideoCallToken(string sessionId, double expireTime, string data = null, List<string> initialLayoutClassList = null)
        {
            var OpenTok = new OpenTok(GlobalVariables.OpenTokApiKey, GlobalVariables.OpenTokApiSecret);
            // Generate a token from a sessionId (fetched from database)
            return OpenTok.GenerateToken(sessionId: sessionId, expireTime: expireTime, data: data);
        } 
    }
}
