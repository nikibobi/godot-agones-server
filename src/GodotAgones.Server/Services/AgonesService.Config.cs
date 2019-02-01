using System;
using System.IO;
using System.Net.Http;

namespace GodotAgones.Server.Services
{
    public partial class AgonesService
    {
        public class Config
        {
            const string Api = "stable.agones.dev";
            const string Version = "v1alpha1";
            const string Namespace = "default";
            const string ServiceAccountPath = "/var/run/secrets/kubernetes.io/serviceaccount";

            public static Action<HttpClient> GetActive()
            {
                bool inCluster = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IN_CLUSTER"));
                if (inCluster)
                {
                    return ServiceAccount;
                }
                else
                {
                    return KubectlProxy;
                }
            }

            public static void ServiceAccount(HttpClient c)
            {
                string tokenPath = Path.Combine(ServiceAccountPath, "token");
                string token = File.ReadAllText(tokenPath);
                c.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                string host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
                string port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
                c.BaseAddress = GetBaseUri($"https://{host}:{port}");
            }

            public static void KubectlProxy(HttpClient c)
            {
                c.BaseAddress = GetBaseUri("http://127.0.0.1:8001");
            }

            private static Uri GetBaseUri(string baseUrl)
            {
                return new Uri($"{baseUrl}/apis/{Api}/{Version}/namespaces/{Namespace}/");
            }
        }
    }
}