using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using AnfasAPI.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace AnfasAPI.Common
{
    public static class CommonFunctions
    {
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static string getUserId(this ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            if (!identity.IsAuthenticated)
            {
                return "";
            }
            Claim identityClaim = identity.Claims.FirstOrDefault(c => c.Type == "UserId");
            return identityClaim.Value.ToString();
        }

        public static string getUserRole(this ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            if (!identity.IsAuthenticated)
            {
                return "";
            }
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles.FirstOrDefault();
        }

        public static int getFourDigitCode()
        {
            return new Random().Next(1000, 9999);
        }

        public static string GenerateOTP()
        {
            string OTPLength = "6";

            string NewCharacters = "";

            //This one tells you which characters are allowed in this new password
            string allowedChars = "";
            //Here Specify your OTP Characters
            allowedChars = "1,2,3,4,5,6,7,8,9,0";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);

            string IDString = "";
            string temp = "";

            //utilize the "random" class
            Random rand = new Random();

            for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                IDString += temp;
                NewCharacters = IDString;
            }

            return NewCharacters;
        }

        public static string GenerateAccessToken(string userId, string role, string deviceToken)
        {
            IdentityOptions _options = new IdentityOptions();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                  new Claim("UserId",userId),
                  new Claim(_options.ClaimsIdentity.RoleClaimType,role),
                  new Claim("DeviceToken", deviceToken)
                }),
                Expires = DateTime.UtcNow.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("QWERTY12345678901LIVECARE24")), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public static string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            return filename;
        }

        public static string RenameFileName(string filename)
        {
            var outFileName = "";
            if (filename != null || filename != "")
            {
                var extension = System.IO.Path.GetExtension(filename);
                //filename = DateTime.Now.ToString() + DateTime.Now.Millisecond.ToString() + filename;
                //filename = filename.Replace(' ', '0').Replace(':', '1').Replace('/', '0');
                outFileName = Guid.NewGuid().ToString() + extension; //+ extension;
            }
            return outFileName;
        }

        public static ProviderUserDetails ValidateGoogleToken(string providerToken)
        {
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GlobalVariables.GoogleApiTokenInfoUrl, providerToken));

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception)
            {
                return null;
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

            return new ProviderUserDetails
            {
                Email = googleApiTokenInfo.email,
                FirstName = googleApiTokenInfo.given_name,
                LastName = googleApiTokenInfo.family_name,
                Locale = googleApiTokenInfo.locale,
                Name = googleApiTokenInfo.name,
                ProviderUserId = googleApiTokenInfo.sub
            };
        }

        public static List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                             .Select(day => new DateTime(year, month, day)) // Map each day to a date
                             .ToList(); // Load dates into a list
        }

        public static string GetMonthYearDayFromDate(DateTime date)
        {
            DateTime datevalue = date;
            string dy = datevalue.Day.ToString();
            string mn = datevalue.Month.ToString();
            string yy = datevalue.Year.ToString();
            string data = dy + "," + mn + "," + yy;
            return data;
        }

        public static DateTime GetDateTime(string strDate, string strTime)
        {
            DateTime dt;
            int dd = Convert.ToInt32(strDate.Substring(0, 2));
            int mm = Convert.ToInt32(strDate.Substring(2, 2));
            int yy = Convert.ToInt32("20" + strDate.Substring(4, 2));
            TimeSpan ts = GetTimeFromString(strTime);
            dt = new DateTime(yy, mm, dd, ts.Hours, ts.Minutes, 0);
            return dt;
        }

        public static TimeSpan GetTimeFromString(string strTime)
        {
            int hh = Convert.ToInt32(strTime.Substring(0, 2));
            int mins = Convert.ToInt32(strTime.Substring(3, 2));
            int ss = Convert.ToInt32(strTime.Substring(6, 2));
            return new TimeSpan(hh, mins, ss);
        }

        public static DateTime CombineDateTimestamp(DateTime date, DateTime time)
        {
            DateTime dt = date.Date + time.TimeOfDay;
            return dt;
        }

        public static DateTime CombineDateTime(DateTime date, TimeSpan SlotTo)
        {

            DateTime dt = Convert.ToDateTime(date.ToString(GlobalVariables.DefaultDateFormat) + " " + ConvertTimeSpanToString(SlotTo));
            return dt;
        }
        public static DateTime CombineServerDateTime(DateTime date, TimeSpan SlotTo)
        {

            DateTime dt = Convert.ToDateTime(date.ToString(@"dd-MM-yyyy") + " " + ConvertTimeSpanToString(SlotTo));
            return dt;
        }

        public static int GetTimeDifferenceInMinutes(DateTime fromTime, DateTime toTime)
        {
            TimeSpan ts = fromTime - toTime;
            return Convert.ToInt32(ts.TotalMinutes);
        }

        public static Ical.Net.Calendar CreateICSCalendar(DateTime? start, DateTime? end, string title, string description, string location, string organizername, string organizermail)
        {
            var calendar = new Ical.Net.Calendar();
            calendar.Method = "PUBLISH";
            CalendarEvent evt = calendar.Create<CalendarEvent>();
            evt.Class = "PUBLIC";
            evt.Summary = title;
            evt.Created = new CalDateTime(DateTime.UtcNow);
            evt.Description = description;
            evt.Start = new CalDateTime(ConvertToVCalendarDateString(start.Value));
            evt.End = new CalDateTime(ConvertToVCalendarDateString(end.Value));
            evt.Sequence = 0;
            evt.Uid = Guid.NewGuid().ToString();
            evt.Location = location;
            evt.Transparency = TransparencyType.Transparent;
            evt.Organizer = new Organizer()
            {
                CommonName = organizername,
                Value = new Uri($"mailto:{organizermail}")
            };
            Alarm reminder = new Alarm();
            reminder.Action = AlarmAction.Display;
            reminder.Trigger = new Trigger(new TimeSpan(-2, 0, 0));
            evt.Alarms.Add(reminder);
            return calendar;
        }

        public static System.Net.Mail.Attachment GetICSAttachment(Ical.Net.Calendar calendarContent, string fileName)
        {
            var serializer = new CalendarSerializer(new SerializationContext());
            var serializedCalendar = serializer.SerializeToString(calendarContent);
            var bytesCalendar = Encoding.UTF8.GetBytes(serializedCalendar);
            MemoryStream ms = new MemoryStream(bytesCalendar);
            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, "Event_" + fileName + ".ics", "text/calendar");
            return attachment;
        }

        public static string Generate13UniqueDigits()
        {
            return DateTime.UtcNow.ToString("yyMMddHHmmssf");
        }

        public static string Generate15UniqueDigits()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssf");
        }

        //16 digit timestamp
        public static string GetShortTimestamp()
        {
            return DateTime.UtcNow.ToString("yyMMddHHmmssffff");
        }

        //18 digit timestamp
        public static string GetLongTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
        }
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }


        public static string ConvertToVCalendarDateString(DateTime d)
        {
            d = d.ToUniversalTime();
            string yy = d.Year.ToString();
            string mm = d.Month.ToString("D2");
            string dd = d.Day.ToString("D2");
            string hh = d.Hour.ToString("D2");
            string mm2 = d.Minute.ToString("D2");
            string ss = d.Second.ToString("D2");
            string s = yy + mm + dd + "T" + hh + mm2 + ss + "Z"; // Pass date as vCalendar format YYYYMMDDTHHMMSSZ (includes middle T and final Z) '
            return s;
        }

        public static bool checkIsNumeric(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static string ConvertTimeSpanToString(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        public static string[] SplitStringToParts(string passedString)
        {
            return passedString.Split(" ", StringSplitOptions.None);
        }

        public static int CalculateEstimatedTimeInMinutes(DateTime AppointmentDate, TimeSpan SlotFrom)
        {
            DateTime TodayDate = DateTime.Now;
            string Date = AppointmentDate.Date.ToString(GlobalVariables.DefaultDateFormat) + " " + ConvertTimeSpanToString(SlotFrom);
            DateTime appointmentDate = Convert.ToDateTime(Date);
            TimeSpan difference = appointmentDate - TodayDate;
            return Convert.ToInt32(difference.TotalMinutes);
        }

        public static int CalculateEstimatedTimeInMinutesUTC(DateTime AppointmentDate, TimeSpan SlotFrom)
        {
            try
            {
                DateTime TodayDate = DateTime.UtcNow;
                DateTime appointmentDate = ConvertToDateTime2(AppointmentDate, SlotFrom);
                TimeSpan difference = appointmentDate - TodayDate;
                return Convert.ToInt32(difference.TotalMinutes);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static int GetAge(DateTime birthDate)
        {
            DateTime n = DateTime.Now; // To avoid a race condition around midnight
            int age = n.Year - birthDate.Year;

            if (n.Month < birthDate.Month || (n.Month == birthDate.Month && n.Day < birthDate.Day))
                age--;

            return age;
        }

        public static DateTime ConvertToDateTime1(string strDateTime)
        {
            DateTime dtFinaldate;
            string sDateTime;
            try
            {
                dtFinaldate = Convert.ToDateTime(strDateTime);
            }
            catch (Exception)
            {
                string[] sDate = strDateTime.Split('-');
                sDateTime = sDate[1] + '/' + sDate[0] + '/' + sDate[2];
                dtFinaldate = Convert.ToDateTime(sDateTime);
            }
            return dtFinaldate;
        }
        public static DateTime ConvertToDateTime2(DateTime date, TimeSpan slotTime)
        {
            DateTime dtFinaldate = new DateTime();
            string sDateTime;
            string sDate = date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            string sTime = slotTime.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
            string DateTimestring = sDate + " " + sTime;
            try
            {

            }
            catch (Exception)
            {
                string[] strDate = DateTimestring.Split('-');
                sDateTime = strDate[1] + '/' + strDate[0] + '/' + strDate[2];
                dtFinaldate = Convert.ToDateTime(sDateTime);
            }
            return dtFinaldate;
        }

    }
}
