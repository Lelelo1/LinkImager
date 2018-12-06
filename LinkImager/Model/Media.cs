using System;
namespace LinkImager.Model
{
    public class Media
    {
        public string Id { get; set; }
        public string ApplicationKey { get; set; }
        public string MediaUrl { get; set; }
        public Media(string ApplicationKey, string MediaUrl)
        {
            this.ApplicationKey = ApplicationKey;
            this.MediaUrl = MediaUrl;
        }
    }
}
