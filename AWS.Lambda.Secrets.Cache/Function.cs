using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWS.Lambda.Secrets.Cache;

// AWS Doco
// https://docs.aws.amazon.com/secretsmanager/latest/userguide/retrieving-secrets_lambda.html
public class Function
{
    /*
        PARAMETERS_SECRETS_EXTENSION_CACHE_ENABLED
         Set to true to cache parameters and secrets. Set to false for no caching. Default is true.

        PARAMETERS_SECRETS_EXTENSION_CACHE_SIZE
         The maximum number of secrets and parameters to cache. Must be a value from 0 to 1000. A value of 0 means there is no caching. This variable is ignored if both SSM_PARAMETER_STORE _TTL and SECRETS_MANAGER_TTL are 0. Default is 1000.

        PARAMETERS_SECRETS_EXTENSION_HTTP_PORT
         The port for the local HTTP server. Default is 2773.

        PARAMETERS_SECRETS_EXTENSION_LOG_LEVEL
         The level of logging the extension provides: debug, info, warn, error, or none. Set to debug to see the cache configuration. Default is info.

        PARAMETERS_SECRETS_EXTENSION_MAX_CONNECTIONS
         Maximum number of connections for HTTP clients that the extension uses to make requests to Parameter Store or Secrets Manager. This is a per-client configuration. Default is 3.

        SECRETS_MANAGER_TIMEOUT_MILLIS
         Timeout for requests to Secrets Manager in milliseconds. A value of 0 means there is no timeout. Default is 0.

        SECRETS_MANAGER_TTL
         TTL of a secret in the cache in seconds. A value of 0 means there is no caching. The maximum is 300 seconds. This variable is ignored if PARAMETERS_SECRETS_CACHE_SIZE is 0. Default is 300 seconds.

        SSM_PARAMETER_STORE_TIMEOUT_MILLIS
         Timeout for requests to Parameter Store in milliseconds. A value of 0 means there is no timeout. Default is 0.

        SSM_PARAMETER_STORE_TTL
         TTL of a parameter in the cache in seconds. A value of 0 means there is no caching. The maximum is 300 seconds. This variable is ignored if PARAMETERS_SECRETS_CACHE_SIZE is 0. Default is 300 seconds.
     */

    public Function()
    {
        WriteOutEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_CACHE_ENABLED");
        WriteOutEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_CACHE_SIZE");
        WriteOutEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_HTTP_PORT");
        WriteOutEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_LOG_LEVEL");
        WriteOutEnvironmentVariable("PARAMETERS_SECRETS_EXTENSION_MAX_CONNECTIONS");
        WriteOutEnvironmentVariable("SECRETS_MANAGER_TIMEOUT_MILLIS");
        WriteOutEnvironmentVariable("SECRETS_MANAGER_TTL");
        WriteOutEnvironmentVariable("SSM_PARAMETER_STORE_TIMEOUT_MILLIS");
        WriteOutEnvironmentVariable("SSM_PARAMETER_STORE_TTL");
        WriteOutEnvironmentVariable("SECRET_KEY_NAME");

        Environment.SetEnvironmentVariable("AWS_SECRETS_EXTENTION_HTTP_PORT", "2773");
        var httpPort = Environment.GetEnvironmentVariable("AWS_SECRETS_EXTENTION_HTTP_PORT");
        Environment.SetEnvironmentVariable("AWS_SECRETS_EXTENTION_SERVER_ENDPOINT", $"http://localhost:{httpPort}/secretsmanager/get?secretId=");
    }

    private void WriteOutEnvironmentVariable(string environmentVariable)
    {
        Console.WriteLine($"{environmentVariable}: {Environment.GetEnvironmentVariable(environmentVariable)}");
    }
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandlerAsync(string input, ILambdaContext context)
    {        
        var url = new Uri(Environment.GetEnvironmentVariable("AWS_SECRETS_EXTENTION_SERVER_ENDPOINT") + Environment.GetEnvironmentVariable("SECRET_KEY_NAME"));
        Console.WriteLine($"AWS_SECRETS_EXTENTION_SERVER_ENDPOINT: {url}");

        var token = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");
        Console.WriteLine($"AWS_SESSION_TOKEN: {token}");

        var apiClient = new ApiClientService(HttpMethod.Get, url, token);
        var result = await apiClient.ExecuteHttpRequest("");

        Console.WriteLine($"Http status code: {result.Item2}");
        Console.WriteLine($"response : {result.Item1}");

        return result.Item1.ToUpper();
    }
}
