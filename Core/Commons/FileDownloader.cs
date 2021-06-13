using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

public static class FileDownloader
{
    private const string GOOGLE_DRIVE_DOMAIN2 = "https://drive.google.com/file";

    public static FileInfo DownloadFileFromURLToPath(string url, string path)
    {
        if (url.StartsWith(GOOGLE_DRIVE_DOMAIN2))
            return DownloadGoogleDriveFileFromURLToPath(url, path);
        return null;
    }

    private static FileInfo DownloadFile(string url, string path)
    {
        try
        {
            using (var cookieAwareWebClient = new CookieAwareWebClient())
            {
                cookieAwareWebClient.DownloadFile(url, path);
                return new FileInfo(path);
            }
        }
        catch (WebException ex)
        {
            return null;
        }
    }

    // Downloading large files from Google Drive prompts a warning screen and
    // requires manual confirmation. Consider that case and try to confirm the download automatically
    // if warning prompt occurs
    private static FileInfo DownloadGoogleDriveFileFromURLToPath(string url, string path)
    {
        // You can comment the statement below if the provided url is guaranteed to be in the following format:
        // https://drive.google.com/uc?id=FILEID&export=download
        url = GetGoogleDriveDownloadLinkFromUrl(url);
        FileInfo downloadedFile;
        downloadedFile = DownloadFile(url, path);
        return downloadedFile;
    }

    // Handles 3 kinds of links (they can be preceeded by https://):
    // - drive.google.com/open?id=FILEID
    // - drive.google.com/file/d/FILEID/view?usp=sharing
    // - drive.google.com/uc?id=FILEID&export=download
    public static string GetGoogleDriveDownloadLinkFromUrl(string url)
    {
        Regex regex = new Regex(@"[-\w]{25,}");
        Match match = regex.Match(url);
        if (!match.Success) return null;
        var id = match.Value;
        return string.Format("https://drive.google.com/uc?id={0}&export=download", id);
    }
}

// Web client used for Google Drive
public class CookieAwareWebClient : WebClient
{
    private class CookieContainer
    {
        Dictionary<string, string> _cookies;

        public string this[Uri url]
        {
            get
            {
                string cookie;
                if (_cookies.TryGetValue(url.Host, out cookie))
                    return cookie;

                return null;
            }
            set
            {
                _cookies[url.Host] = value;
            }
        }

        public CookieContainer()
        {
            _cookies = new Dictionary<string, string>();
        }
    }

    private CookieContainer cookies;

    public CookieAwareWebClient() : base()
    {
        cookies = new CookieContainer();
    }

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest request = base.GetWebRequest(address);

        if (request is HttpWebRequest)
        {
            string cookie = cookies[address];
            if (cookie != null)
                ((HttpWebRequest)request).Headers.Set("cookie", cookie);
        }

        return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        WebResponse response = base.GetWebResponse(request, result);

        string[] cookies = response.Headers.GetValues("Set-Cookie");
        if (cookies != null && cookies.Length > 0)
        {
            string cookie = "";
            foreach (string c in cookies)
                cookie += c;

            this.cookies[response.ResponseUri] = cookie;
        }

        return response;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        WebResponse response = base.GetWebResponse(request);

        string[] cookies = response.Headers.GetValues("Set-Cookie");
        if (cookies != null && cookies.Length > 0)
        {
            string cookie = "";
            foreach (string c in cookies)
                cookie += c;

            this.cookies[response.ResponseUri] = cookie;
        }

        return response;
    }
}