#if JSON_NET_EXISTS

using DA_Assets.FCU.Exceptions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace DA_Assets.FCU
{
    public static class WebClient
    {
        public static int webRequestDelay = 100;
        public static float pbarProgress;
        public static string pbarContent = "0 kB";

        public static IEnumerator Authorize(Action<string> actionResult)
        {
            string code = "";
            bool gettingCode = true;

            Thread thread = null;

            Console.WriteLine(Localization.OPEN_AUTH_PAGE);

            thread = new Thread(x =>
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 1923);

                server.Bind(endpoint);
                server.Listen(1);

                Socket socket = server.Accept();

                byte[] bytes = new byte[1000];
                socket.Receive(bytes);
                string rawCode = Encoding.UTF8.GetString(bytes);

                string toSend = "HTTP/1.1 200 OK\nContent-Type: text/html\nConnection: close\n\n" + @"
                    <html>
                        <head>
                            <style type='text/css'>body,html{background-color: #000000;color: #fff;font-family: Segoe UI;text-align: center;}h2{left: 0; position: absolute; top: calc(50% - 25px); width: 100%;}</style>
                            <title>Wait for redirect...</title>
                            <script type='text/javascript'> window.onload=function(){window.location.href='https://figma.com';}</script>
                        </head>
                        <body>
                            <h2>Authorization completed. The page will close automatically.</h2>
                        </body>
                    </html>";
                bytes = Encoding.UTF8.GetBytes(toSend);

                NetworkStream stream = new NetworkStream(socket);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                stream.Close();
                socket.Close();
                server.Close();

                code = rawCode.GetBetween("?code=", "&state=");
                gettingCode = false;
                thread.Abort();
            });

            thread.Start();

            int state = Random.Range(0, int.MaxValue);
            string formattedOauthUrl = string.Format(Constants.OAUTH_URL, Constants.CLIENT_ID, Constants.REDIRECT_URI, state.ToString());

            Application.OpenURL(formattedOauthUrl);

            while (string.IsNullOrWhiteSpace(code) && gettingCode)
            {
                yield return new WaitForSecondsRealtime(0.01f);
            }

            if (string.IsNullOrWhiteSpace(code) == false)
            {
                Console.WriteLine(Localization.TRY_GET_API_KEY);

                EditorCoroutineUtility.StartCoroutineOwnerless(GetToken(code,
                (token) =>
                {
                    actionResult.Invoke(token);
                },
                (figmaError) =>
                {
                    throw new InvalidAuthException();
                }));
            }
            else
            {
                throw new InvalidAuthException();
            }
        }

        public static IEnumerator GetToken(string code, Action<string> success, Action<FigmaError> error)
        {
            string query = string.Format(Constants.AUTH_URL, Constants.CLIENT_ID, Constants.CLIENT_SECRET, Constants.REDIRECT_URI, code);//ClientCode

            EditorCoroutineUtility.StartCoroutineOwnerless(_MakeRequest<AuthResult>(new Request
            {
                Query = query,
                RequestType = RequestType.Post,
                WWWForm = new WWWForm()
            }, 
            (authResult) =>
            {
                if (string.IsNullOrWhiteSpace(authResult.access_token))
                {
                    error.Invoke(new FigmaError 
                    { 
                        Status = 404,
                        Error = "No access token."
                    });
                }
                else
                {
                    success.Invoke(authResult.access_token);
                }
            }, error));

            yield return null;
        }
        public static IEnumerator GetProject(Action<FigmaProject> successAction, Action<FigmaError> errorAction)
        {
            string query = string.Format(Constants.API_LINK, FigmaConverterUnity.Instance.mainSettings.ProjectUrl.GetBetween("file/", "/"));

            FigmaConverterUnity.Instance.StartCoroutine(_MakeRequest<FigmaProject>(new Request
            {
                Query = query,
                RequestType = RequestType.Get,
                RequestHeader = new RequestHeader
                {
                    Name = "Authorization",
                    Value = $"Bearer {FigmaConverterUnity.Instance.mainSettings.ApiKey}"
                }
            }, successAction, errorAction));

            yield return null;
        }

        public static IEnumerator GetImageLinksForFObjects(List<FObject> fobjects, Action<List<FObject>> result)
        {
            Console.WriteLine(Localization.START_ADDING_LINKS);

            List<string> _fobjects = fobjects
                .Where(x => x.Visible != false)
                .Select(x => x.Id).ToList();

            int gettedCount = 0;
            string projectId = FigmaConverterUnity.Instance.mainSettings.ProjectUrl.GetBetween("file/", "/");
            string extension = FigmaExtensions.GetImageExtension();
            float scale = FigmaExtensions.GetImageScale();
            string token = FigmaConverterUnity.Instance.mainSettings.ApiKey;

            List<IdsLinksContainer> idsLinksContainers = new List<IdsLinksContainer>();
            List<List<string>> idChunks = _fobjects.ToChunks(FigmaExtensions.GetImportSpeed());

            foreach (List<string> chunk in idChunks)
            {
                gettedCount += chunk.Count();
                Console.WriteLine(string.Format(Localization.GETTING_LINKS, gettedCount, _fobjects.Count()));

                EditorCoroutineUtility.StartCoroutineOwnerless(_MakeRequest<IdsLinksContainer>(new Request
                {
                    RequestType = RequestType.Get,
                    Query = CreateImagesQuery(projectId, extension, scale, chunk),
                    RequestHeader = new RequestHeader
                    {
                        Name = "Authorization",
                        Value = $"Bearer {token}"
                    }
                },
                (idsLinksContainer) =>
                {
                    idsLinksContainers.Add(idsLinksContainer);
                },
                (figmaError) =>
                {
                    switch (figmaError.Status)
                    {
                        case 429:
                            Console.Error(Localization.API_LIMIT);
                            break;
                        case 413:
                            Console.Error(Localization.CLOUDFRONT_ERROR);
                            break;
                        default:
                            Console.Error(string.Format(Localization.UNKNOWN_ERROR, figmaError.Status, figmaError.Error, Constants.TG_LINK));
                            break;
                    }

                    idsLinksContainers.Add(null);
                }));
            }

            while (idChunks.Count() != idsLinksContainers.Count())
            {
                yield return new WaitForSecondsRealtime(0.01f);
            }

            Dictionary<string, string> idsLinks = new Dictionary<string, string>();

            foreach (IdsLinksContainer idsLinksContainer in idsLinksContainers)
            {
                if (idsLinksContainer == null)
                {
                    continue;
                }

                idsLinks.AddRange(idsLinksContainer.IdsLinks);
            }

            List<FObject> _children = new List<FObject>();

            foreach (string id in idsLinks.Keys)
            {
                FObject childWithLink = fobjects.FirstOrDefault(x => x.Id == id);

                if (string.IsNullOrWhiteSpace(idsLinks[id]) == false)
                {
                    if (FigmaConverterUnity.Instance.mainSettings.HTTPS)
                    {
                        childWithLink.Link = idsLinks[id];
                    }
                    else
                    {
                        childWithLink.Link = idsLinks[id].Replace("https://", "http://");
                    }

                    _children.Add(childWithLink);
                }
                else
                {
                    Console.Error(string.Format(Localization.CANT_GET_IMAGE_LINK, childWithLink.GetFOBjectHierarchy()));
                }
            }

            Console.WriteLine(Localization.LINKS_ADDED);

            result.Invoke(_children);
            yield return null;
        }

        private static string CreateImagesQuery(string projectId, string extension, float scale, List<string> chunk)
        {
            string joinedIds = string.Join(",", chunk);
            if (joinedIds[0] == ',')
            {
                joinedIds = joinedIds.Remove(0, 1);
            }

            string query = $"https://api.figma.com/v1/images/{projectId}?ids={joinedIds}&format={extension}&scale={scale}";
            return query;
        }

        public static IEnumerator DownloadSpritesAsync(List<FObject> children, Action<List<FObject>> actionResult)
        {
            Console.WriteLine(Localization.START_SPRITES_DOWNLOAD);

            List<FObject> _children = new List<FObject>();

            int skiped = 0;
            Dictionary<string, byte[]> downloadedImages = new Dictionary<string, byte[]>();

            foreach (FObject child in children)
            {
                string path = child.GetAssetPath(true);
                child.AssetPath = child.GetAssetPath(false);

                bool imageFileExists = File.Exists(path);
                bool alreadyDownloaded = _children.Select(x => x.AssetPath).Contains(child.GetAssetPath(false));
                bool notDownloadable = child.IsDownloadable() == false;

                if (FigmaConverterUnity.Instance.mainSettings.ReDownloadSprites)
                {
                    if (notDownloadable || alreadyDownloaded)
                    {
                        skiped++;
                        _children.Add(child);
                        continue;
                    }
                }
                else
                {
                    if (notDownloadable || alreadyDownloaded || imageFileExists)
                    {
                        skiped++;
                        _children.Add(child);
                        continue;
                    }
                }

                try
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(_MakeRequest<byte[]>(new Request
                    {
                        RequestType = RequestType.GetFile,
                        Query = child.Link
                    },
                    (imageBytes) =>
                    {
                        downloadedImages.Add(child.GetAssetPath(true), imageBytes);
                    }, 
                    (figmaError)=> 
                    { 

                    }));
                }
                catch (Exception ex)
                {
                    skiped++;
                    Console.Error(ex);
                }

                _children.Add(child);
            }

            while (_children.Count() != (downloadedImages.Count() + skiped))
            {
                yield return new WaitForSecondsRealtime(0.01f);
            }

            foreach (var item in downloadedImages)
            {
                File.WriteAllBytes(item.Key, item.Value);
                AssetDatabase.Refresh();
            }

            Console.WriteLine(string.Format(Localization.DRAW_COMPONENTS, _children.Count()));

            actionResult.Invoke(_children);
        }

        private static IEnumerator _MakeRequest<T>(Request request, Action<T> successAction, Action<FigmaError> errorAction)
        {
            UnityWebRequest webRequest;

            if (request.RequestType == RequestType.Get || request.RequestType == RequestType.GetFile)
            {
                webRequest = UnityWebRequest.Get(request.Query);
            }
            else
            {
                webRequest = UnityWebRequest.Post(request.Query, request.WWWForm);
            }

            using (webRequest)
            {
                if (request.RequestHeader != null)
                {
                    webRequest.SetRequestHeader(request.RequestHeader.Name, request.RequestHeader.Value);
                }

                webRequest.SendWebRequest();

                while (webRequest.isDone == false)
                {
                    if (pbarProgress < 1f)
                    {
                        pbarProgress += 0.1f;
                    }
                    else
                    {
                        pbarProgress = 0;
                    }

                    pbarContent = string.Format("{0} kB", webRequest.downloadedBytes / 1024);
                    FigmaConverterUnity.Instance.Editor.Repaint();

                    yield return new WaitForSecondsRealtime(0.01f);
                }

                bool isRequestError;
#if UNITY_2020_1_OR_NEWER
                isRequestError = webRequest.result == UnityWebRequest.Result.ConnectionError;
#else
                isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif
                if (isRequestError)
                {
                    if (webRequest.error.Contains("SSL"))
                    {
                        throw new Exception(string.Format(Localization.SSL_ERROR, webRequest.error));
                    }
                    else
                    {
                        throw new Exception(webRequest.error);
                    }
                }

                T requestResult = default;

                if (request.RequestType == RequestType.GetFile)
                {
                    requestResult = (T)(object)webRequest.downloadHandler.data;
                }
                else
                {
                    string text = webRequest.downloadHandler.text;

                    WriteLog(request, text);

                    if (text.Contains($"<H1>413 ERROR</H1>"))
                    {
                        errorAction.Invoke(new FigmaError
                        {
                            Status = 413,
                            Error = Localization.CLOUDFRONT_ERROR
                        });
                        yield break;
                    }

                    try
                    {
                        int[] allowedStatuses = new int[] { 0, 200 };

                        FigmaError figmaError = JsonConvert.DeserializeObject<FigmaError>(text, jsonSerializerSettings);

                        if (allowedStatuses.Contains(figmaError.Status) == false)
                        {
                            WriteLog(request, text);
                            errorAction.Invoke(figmaError);
                            yield break;
                        }
                    }
                    catch
                    {

                    }

                    requestResult = JsonConvert.DeserializeObject<T>(text, jsonSerializerSettings);
                }

                pbarProgress = 0f;
                pbarContent = "0 kB";
                FigmaConverterUnity.Instance.Editor.Repaint();

                successAction.Invoke(requestResult);
            }
        }
        private static void WriteLog(Request request, string text)
        {
            if (FigmaConverterUnity.Instance.mainSettings.DebugMode)
            {
                string logText = text;
                string assetPath = $"{Application.dataPath}/{Constants.PUBLISHER}/{Constants.PRODUCT_NAME}";
                string logPath = Path.Combine(assetPath, Constants.LOGS_FOLDER_NAME);
                string logFileName = $"{DateTime.Now.ToString("HH-mm-ss-fff")}_{Constants.JSON_FILE_NAME}";
                string logFilePath = Path.Combine(logPath, logFileName);

                if (Directory.Exists(logPath) == false)
                {
                    Directory.CreateDirectory(logPath);
                }

                try
                {
                    JToken parsedJson = JToken.Parse(logText);
                    logText = parsedJson.ToString(Formatting.Indented);
                }
                catch
                {

                }

                logText = $"{request.Query}\n{logText}";
                File.WriteAllText(logFilePath, logText);
            }
        }
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Error = (sender, error) =>
            {
                error.ErrorContext.Handled = true;
            }
        };
        private struct Request
        {
            public string Query;
            public RequestType RequestType;
            public RequestHeader RequestHeader;
            public WWWForm WWWForm;
        }
        private class RequestHeader
        {
            public string Name;
            public string Value;
        }
        private enum RequestType
        {
            Get,
            Post,
            GetFile,
        }
        private struct AuthResult
        {
            public string access_token;
            public string expires_in;
            public string refresh_token;
        }
        private class IdsLinksContainer
        {
            [JsonProperty("err")]
            public string Err;

            [JsonProperty("images")]
            public Dictionary<string, string> IdsLinks;
        }
    }
}

#endif
