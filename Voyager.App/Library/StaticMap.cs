using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.Text;
using VGER_WAPI_CLASSES;

namespace Voyager.App.Library
{

    /// <summary>
    /// Class to generate a static map using the Google StaticMaps API
    /// http://code.google.com/apis/maps/documentation/staticmaps/
    /// There is an article supporting this code available at 
    /// http://www.codeproject.com/KB/aspnet/csharp-google-static-map.aspx
    /// </summary>
    public class StaticMap
    {
        /// <summary>
        /// configuration object to be passed to fetch value from appsettings.json
        /// </summary>
        public static IConfiguration _configuration;

        /// <summary>
        /// Renders an image for display
        /// </summary>
        /// <returns></returns>
        /// <remarks>Primarily this just creates an ImageURL string</remarks>
        public string Render()
        {
            string qs = "https://maps.googleapis.com/maps/api/staticmap?center={0},{1}&zoom={2}&size={3}x{4}&maptype={5}";
            string mkqs = "";

            qs = string.Format(qs, LatCenter, LngCenter, Zoom, Width, Height, Type.ToString().ToLower());


            // add markers
            foreach (var marker in _markers)
            {

                mkqs += string.Format("{0},{1},{2}|",
                    marker.Lat,
                    marker.Lng,
                    GetMarkerParams(marker.Size, marker.Color, marker.Character));
            }

            if (mkqs.Length > 0)
            {
                qs += "&markers=" + mkqs.Substring(0, (mkqs.Length - 1));
            }

            qs += "&key=" + APIKey;

            return qs;
        }

        /// <summary>
        /// Build the correct string for marker parameters
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// (Some marker sizes such as 'tiny' won't accept character values, 
        ///     this function makes sure they'll be rendered even if the inputed parameters are wrong
        /// </remarks>
        private static string GetMarkerParams(mSize size, mColor color, string character)
        {
            string marker;

            // check if can render character
            if ((size == mSize.Normal) || (size == mSize.Mid))
            {
                if (size == mSize.Normal)
                {
                    marker = color.ToString().ToLower() + character;
                }
                else
                {
                    marker = size.ToString().ToLower() + color.ToString().ToLower() + character;
                }
            }
            else
            {
                // just render size and color - character not supported
                marker = size.ToString().ToLower() + color.ToString().ToLower();
            }

            return marker;

        }

        /// <summary>
        /// Defines a single map point to be added to a map
        /// </summary>
        public class Marker
        {

            private string _char = "";

            /// <summary>
            /// Optional single letter character
            /// </summary>
            public string Character
            {
                get { return _char; }
                set { _char = value; }
            }

            #region Auto-Properties

            /// <summary>
            /// Lattitude on map
            /// </summary>
            public Double Lat { get; set; }

            /// <summary>
            /// Longitude on map
            /// </summary>
            public Double Lng { get; set; }

            /// <summary>
            /// Size of map in pixels (width X height)
            /// </summary>
            public StaticMap.mSize Size { get; set; }

            /// <summary>
            /// Color of marker in map
            /// </summary>
            public StaticMap.mColor Color { get; set; }

            #endregion

        }

        /// <summary>
        /// All Map rendering properties as enums
        /// </summary>
        #region Marker enums

        public enum mFormat
        {
            gif = 0,
            jpg = 1,
            png = 2
        }

        public enum mSize
        {
            Normal = 0,
            Mid = 1,
            Small = 2,
            Tiny = 3
        }

        public enum mColor
        {
            Black = 0,
            Brown = 1,
            Green = 2,
            Purple = 3,
            Yellow = 4,
            Blue = 5,
            Gray = 6,
            Orange = 7,
            Red = 8,
            White = 9
        }

        public enum mType
        {
            Roadmap = 0,
            Mobile = 1
        }

        #endregion

        /// <summary>
        /// StaticMap props
        /// </summary>
        #region Properties

        private List<Marker> _markers = new List<Marker>();
        private StaticMap.mType _type = StaticMap.mType.Roadmap;

        /// <summary>
        /// List of all markers to be displayed on the map
        /// </summary>
        public List<Marker> Markers
        {
            get { return _markers; }
            set { _markers = value; }
        }

        /// <summary>
        /// Type of map to fetch
        /// e.g. roadmap
        /// </summary>
        public StaticMap.mType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Google maps API key - required!
        /// </summary>
        private static string APIKey
        {
            get
            {
                return _configuration.GetValue<string>("SystemSettings:GoogleAPIKey");
            }
        }

        #region Auto-Properties

        public Double LatCenter { get; set; }
        public Double LngCenter { get; set; }
        public int Zoom { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        #endregion

        #endregion

        #region Sample Map rendering
        private void SampleMap1(object sender, EventArgs e)
        {

            // create new map object
            var marker = new Marker();
            var map = new StaticMap
            {
                Width = 300,
                Height = 300,
                Zoom = 15,
                LatCenter = 55.8592110,
                LngCenter = -4.2466380

            };

            // add markerss
            marker.Lat = 55.8598393;
            marker.Lng = -4.2480182;
            marker.Size = StaticMap.mSize.Normal;
            marker.Color = StaticMap.mColor.Purple;
            marker.Character = "1";
            map.Markers.Add(marker);

            marker = new StaticMap.Marker();
            marker.Lat = 55.8584424;
            marker.Lng = -4.2457652;
            marker.Size = StaticMap.mSize.Small;
            marker.Color = StaticMap.mColor.Green;
            marker.Character = "2"; // this won't be displayed - marker is too small
            map.Markers.Add(marker);


            // render map
            //imgMap.ImageUrl = map.Render();
        }

        private void SampleMap2(object sender, EventArgs e)
        {

            // create new map object
            var marker = new StaticMap.Marker();
            var map = new StaticMap
            {
                Width = 300,
                Height = 300,
                Zoom = 16,
                LatCenter = 55.8592110,
                LngCenter = -4.2466380

            };

            // add markers from datasource
            // (in this case a temporary DataTable, it could/should come from a database or other data source)
            var dt = this.GetData();

            foreach (DataRow row in dt.Rows)
            {
                marker = new StaticMap.Marker();
                marker.Lat = Convert.ToDouble(row["Lat"]);
                marker.Lng = Convert.ToDouble(row["Lng"]);

                // convert our string values to strongly-typed enums
                marker.Size = Tools.ConvertToEnum<StaticMap.mSize>(row["Size"].ToString());
                marker.Color = Tools.ConvertToEnum<StaticMap.mColor>(row["Color"].ToString());

                map.Markers.Add(marker);
            }



            // render map
            //imgMap.ImageUrl = map.Render();
        }

        private void SampleMap3(object sender, EventArgs e)
        {

            // create new map object
            var marker = new StaticMap.Marker();
            var map = new StaticMap
            {
                Width = 175,
                Height = 175,
                Zoom = 15,
                LatCenter = 55.8592110,
                LngCenter = -4.2466380

            };

            // add marker to center point
            marker.Lat = 55.8603110;
            marker.Lng = -4.2449380;
            marker.Size = StaticMap.mSize.Normal;
            marker.Color = StaticMap.mColor.Purple;
            marker.Character = "1";
            map.Markers.Add(marker);

            // render map
            //imgMap.ImageUrl = map.Render();
        }
        #endregion

        /// <summary>
        /// Populates a temporary table with marker data
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// In the real world, this would come from a database
        /// </remarks>
        private DataTable GetData()
        {
            var dt = this.GetGeoTable();

            var row = dt.NewRow();

            // eyes
            row["Lat"] = 55.859647; row["Lng"] = -4.247010; row["Color"] = "Green"; row["Size"] = "Normal";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.859659; row["Lng"] = -4.245915; row["Color"] = "Green"; row["Size"] = "Normal";
            dt.Rows.Add(row);

            // nose
            row = dt.NewRow();
            row["Lat"] = 55.859346; row["Lng"] = -4.246452; row["Color"] = "Blue"; row["Size"] = "Small";
            dt.Rows.Add(row);

            // mouth
            row = dt.NewRow();
            row["Lat"] = 55.858996; row["Lng"] = -4.245722; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.858852; row["Lng"] = -4.245894; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.858731; row["Lng"] = -4.246237; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.858731; row["Lng"] = -4.246645; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.858816; row["Lng"] = -4.246988; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["Lat"] = 55.858948; row["Lng"] = -4.247139; row["Color"] = "Red"; row["Size"] = "Mid";
            dt.Rows.Add(row);

            return dt;
        }

        /// <summary>
        /// Creates a data table
        /// </summary>
        /// <returns></returns>
        private DataTable GetGeoTable()
        {
            var dt = new DataTable();

            DataColumn dCol;

            dCol = new DataColumn();
            dCol.DataType = System.Type.GetType("System.Int32");
            dCol.ColumnName = "id"; // primary key
            dCol.AutoIncrement = true;
            dt.Columns.Add(dCol);

            dCol = new DataColumn();
            dCol.DataType = System.Type.GetType("System.Double");
            dCol.ColumnName = "Lat";
            dt.Columns.Add(dCol);

            dCol = new DataColumn();
            dCol.DataType = System.Type.GetType("System.Double");
            dCol.ColumnName = "Lng";
            dt.Columns.Add(dCol);

            dCol = new DataColumn();
            dCol.DataType = System.Type.GetType("System.String");
            dCol.ColumnName = "Color";
            dt.Columns.Add(dCol);

            dCol = new DataColumn();
            dCol.DataType = System.Type.GetType("System.String");
            dCol.ColumnName = "Size";
            dt.Columns.Add(dCol);

            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static GeocoderLocation Locate(string query)
        {
            string URL = "https://maps.googleapis.com/maps/api/geocode/xml?sensor=false&key=GoogleAPIKey&address=" + HttpUtility.UrlEncode(query);
            URL = URL.Replace("key=GoogleAPIKey", "key=" + APIKey);
            WebRequest request = WebRequest.Create(URL);

            request.Proxy = WebRequest.DefaultWebProxy;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    XDocument document = XDocument.Load(new StreamReader(stream));

                    XElement longitudeElement = document.Descendants("lng").FirstOrDefault();
                    XElement latitudeElement = document.Descendants("lat").FirstOrDefault();

                    if (longitudeElement != null && latitudeElement != null)
                    {
                        return new GeocoderLocation
                        {
                            Longitude = Double.Parse(longitudeElement.Value, CultureInfo.InvariantCulture),
                            Latitude = Double.Parse(latitudeElement.Value, CultureInfo.InvariantCulture)
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// function to render image by the url
        /// </summary>
        /// <param name="query"></param>
        /// <param name="inputFileName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool RenderImage(string query, string inputFileName, out string output)
        {
            try
            {
                byte[] buffer;
                string MapImagePath = _configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath");
                string MapVirtualPath = _configuration.GetValue<string>("SystemSettings:ProposalFilesVirtualPath");
                string outputFilePath = Path.Combine(MapImagePath, inputFileName);
                if (!Directory.Exists(MapImagePath)) Directory.CreateDirectory(MapImagePath);
                if (!File.Exists(outputFilePath))
                {
                    query = query.Replace("key=GoogleAPIKey", "key=" + APIKey);
                    WebRequest request = WebRequest.Create(query);
                    request.Proxy = WebRequest.DefaultWebProxy;
                    request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                int len = (int)(response.ContentLength);
                                buffer = br.ReadBytes(len);
                                br.Close();

                                using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                                {
                                    using (BinaryWriter w = new BinaryWriter(fs))
                                    {
                                        try
                                        {
                                            w.Write(buffer);
                                            output = "/" + MapVirtualPath + "/" + inputFileName;
                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            output = ex.Message;
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    output = "/" + MapVirtualPath + "/" + inputFileName;
                    return true;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// function to render image by the url
        /// </summary>
        /// <param name="query"></param>
        /// <param name="inputFileName"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool RenderExpediaImage(string HotelCode, string inputFileName, out string output)
        {
            try
            {
                byte[] buffer;
                string filePath = _configuration.GetValue<string>("SystemSettings:ProposalDocumentFilePath");
                string virtualPath = _configuration.GetValue<string>("SystemSettings:ProposalFilesVirtualPath");
                string outputFilePath = Path.Combine(filePath, inputFileName);
                if (!File.Exists(outputFilePath))
                {
                    string APIkey = "2ilk6c40pmua7glf8i8hq2n2u";
                    string SecretKey = "b6jocn3618434";
                    string UnixTime = ((DateTimeOffset)(DateTime.UtcNow)).ToUnixTimeSeconds().ToString();
                    string sign = CreateMD5(APIkey + SecretKey + UnixTime);
                    string query = "http://test.ean.com/ean-services/rs/hotel/v3/info";
                    query += "?apiExperience=PARTNER_BOT_CACHE&cid=477240&apiKey=ExpediaAPIKey&sig=ExpediaSignature&minorRev=24&customerUserAgent=Explorer/2.1&customerIpAddress=192.168.252.2&locale=en_US&currencyCode=USD&hotelId=ExpediaHotelId";
                    query = query.Replace("apiKey=ExpediaAPIKey", "apiKey=" + APIKey);
                    query = query.Replace("sig=ExpediaSignature", "sig=" + sign);
                    query = query.Replace("hotelId=ExpediaHotelId", "hotelId=" + HotelCode);
                    WebRequest request = WebRequest.Create(query);
                    request.Proxy = WebRequest.DefaultWebProxy;
                    request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                int len = (int)(response.ContentLength);
                                buffer = br.ReadBytes(len);
                                br.Close();

                                using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                                {
                                    using (BinaryWriter w = new BinaryWriter(fs))
                                    {
                                        try
                                        {
                                            w.Write(buffer);
                                            output = "/"+ virtualPath + "/" + inputFileName;
                                            return true;
                                        }
                                        catch (Exception ex)
                                        {
                                            output = ex.Message;
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    output = "/" + virtualPath + "/" + inputFileName;
                    return true;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                return false;
            }
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
    }

    /// <summary>
    /// Generic helper functions
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// Converts Integers to enum types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Enum int value</param>
        /// <returns></returns>
        /// <example>
        /// Enums.ConvertToEnum/<enum.type/>([EnumAsInt]);
        /// </example>
        public static T ConvertToEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }


        /// <summary>
        /// Converts String to enum types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Enum string value</param>
        /// <returns></returns>
        /// <example>
        /// Enums.ConvertToEnum/<enum.type/>([EnumAsString]);
        /// </example>
        public static T ConvertToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

    ///// <summary>
    ///// Geocoder Location class
    ///// </summary>
    //public class GeocoderLocation
    //{
    //    /// <summary>
    //    /// Langitude in map
    //    /// </summary>
    //    public double Longitude { get; set; }

    //    /// <summary>
    //    /// Latitude in map
    //    /// </summary>
    //    public double Latitude { get; set; }
    //}
}
