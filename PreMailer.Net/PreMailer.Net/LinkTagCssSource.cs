using CsQuery;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace PreMailer.Net.Sources
{
    internal class LinkTagCssSource : ICssSource
    {
        private string _cssContents;

        public LinkTagCssSource(IDomObject node, Uri baseUri)
        {
            // There must be an href
            var href = node.Attributes.First(a => a.Key.Equals("href", StringComparison.OrdinalIgnoreCase)).Value;
            Uri uri;

            if (Uri.IsWellFormedUriString(href, UriKind.Relative) && baseUri != null)
                uri = new Uri(baseUri, href);
            else // Assume absolute
                uri = new Uri(href);

            var request = WebRequest.Create(uri);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                _cssContents = reader.ReadToEnd();
            }
        }

        public string GetCss()
        {
            return _cssContents;
        }
    }
}
