namespace NEE.Core.Helpers
{
    public class WebServiceConnectionString
    {

        public string ConnectionString { get; private set; }

        public string Url { get; private set; }
        public string Uid { get; private set; }
        public string Pwd { get; private set; }

        /// <summary>
        /// Initializes a new instance of the WebServiceConnectionString class.
        /// </summary>
        /// <param name="connectionString">The Web-Service Connection String ("uid|pwd|url)".</param>
        public WebServiceConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            var tokens = this.ConnectionString.Split('|');
            this.Uid = tokens[0].Trim();
            this.Pwd = tokens[1].Trim();
            this.Url = tokens[2].Trim();
        }

        public WebServiceConnectionString(string username, string password, string url)
        {
            this.ConnectionString = $"{username}|{password}|{url}";
            this.Uid = username;
            this.Pwd = password;
            this.Url = url;
        }

    }
}
