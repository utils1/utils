//////////////////////////////////////////
/////// create socket byass proxy1 ////////
Uri testUri = new Uri("http://ip or host:port");
Uri proxy = null;
using (WebClient wc = new WebClient())
{
	proxy = wc.Proxy.GetProxy(testUri);
}
TcpClient t = new TcpClient(proxy.DnsSafeHost, proxy.Port);



//////////////////////////////////////////
/////// create socket byass proxy2 ////////

static TcpClient connectViaHTTPProxy(
    string targetHost, 
    int targetPort, 
    string httpProxyHost, 
    int httpProxyPort, 
    string proxyUserName, 
    string proxyPassword)
{
    var uriBuilder = new UriBuilder
    {
        Scheme = Uri.UriSchemeHttp,
        Host = httpProxyHost,
        Port = httpProxyPort
    };

    var proxyUri = uriBuilder.Uri;

    var request = WebRequest.Create(
        "http://" + targetHost + ":" + targetPort);

    var webProxy = new WebProxy(proxyUri);

    request.Proxy = webProxy;
    request.Method = "CONNECT";

    var credentials = new NetworkCredential(
        proxyUserName, proxyPassword);

    webProxy.Credentials = credentials;

    var response = request.GetResponse();

    var responseStream = response.GetResponseStream();
    Debug.Assert(responseStream != null);

    const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

    var rsType = responseStream.GetType();
    var connectionProperty = rsType.GetProperty("Connection", Flags);

    var connection = connectionProperty.GetValue(responseStream, null);
    var connectionType = connection.GetType();
    var networkStreamProperty = connectionType.GetProperty("NetworkStream", Flags);

    var networkStream = networkStreamProperty.GetValue(connection, null);
    var nsType = networkStream.GetType();
    var socketProperty = nsType.GetProperty("Socket", Flags);
    var socket = (Socket)socketProperty.GetValue(networkStream, null);

    return new TcpClient { Client = socket };
}