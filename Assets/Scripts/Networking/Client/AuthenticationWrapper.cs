using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        if (authState == AuthState.Authenticated)
        {
            return authState;
        }

        if (authState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return authState;
        }

        await SignInAnonymouslyAsync(maxRetries);

        return authState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (authState == AuthState.Authenticating || authState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return authState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        int retries = 0;
        authState = AuthState.Authenticating;

        while (authState == AuthState.Authenticating && retries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authenticationEx)
            {
                Debug.LogError(authenticationEx);
                authState = AuthState.Error;
            }
            catch (RequestFailedException requestFailedEx)
            {
                Debug.LogError(requestFailedEx);
                authState = AuthState.Error;
            }


            retries++;
            await Task.Delay(1000);
        }

        if (authState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully, {retries} retries");
            authState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}