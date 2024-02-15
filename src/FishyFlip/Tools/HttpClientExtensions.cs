﻿// <copyright file="HttpClientExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Text.Json.Serialization.Metadata;

namespace FishyFlip.Tools;

/// <summary>
/// Provides extension methods for HttpClient.
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="T">The type of the request body.</typeparam>
    /// <typeparam name="TK">The type of the response body.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="typeT">The JsonTypeInfo of the request body.</param>
    /// <param name="typeTK">The JsonTypeInfo of the response body.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="body">The request body.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Http response message as the result.</returns>
    internal static async Task<Result<TK>> Post<T, TK>(
       this HttpClient client,
       string url,
       JsonTypeInfo<T> typeT,
       JsonTypeInfo<TK> typeTK,
       JsonSerializerOptions options,
       T body,
       CancellationToken cancellationToken,
       ILogger? logger = default)
    {
        var jsonContent = JsonSerializer.Serialize(body, typeT);
        StringContent content = new(jsonContent, Encoding.UTF8, "application/json");
        logger?.LogDebug($"POST {url}: {jsonContent}");
        using var message = await client.PostAsync(url, content, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsNullOrEmpty() && message.IsSuccessStatusCode)
        {
            response = "{ }";
        }

        logger?.LogDebug($"POST {url}: {response}");
        TK? result = JsonSerializer.Deserialize<TK>(response, typeTK);
        return result!;
    }

    /// <summary>
    /// Sends a POST request with a StreamContent body to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TK">The type of the response body.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="type">The JsonTypeInfo of the response body.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="body">The StreamContent request body.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Http response message as the result.</returns>
    internal static async Task<Result<TK>> Post<TK>(
       this HttpClient client,
       string url,
       JsonTypeInfo<TK> type,
       JsonSerializerOptions options,
       StreamContent body,
       CancellationToken cancellationToken,
       ILogger? logger = default)
    {
        logger?.LogDebug($"POST STREAM {url}: {body.Headers.ContentType}");
        using var message = await client.PostAsync(url, body, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsNullOrEmpty() && message.IsSuccessStatusCode)
        {
            response = "{ }";
        }

        logger?.LogDebug($"POST {url}: {response}");
        TK? result = JsonSerializer.Deserialize<TK>(response, type);
        return result!;
    }

    /// <summary>
    /// Sends a POST request with a StreamContent body to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TK">The type of the response body.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="type">The JsonTypeInfo of the response body.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Http response message as the result.</returns>
    internal static async Task<Result<TK>> Post<TK>(
        this HttpClient client,
        string url,
        JsonTypeInfo<TK> type,
        JsonSerializerOptions options,
        CancellationToken cancellationToken,
        ILogger? logger = default)
    {
        logger?.LogDebug($"POST {url}");
        using var message = await client.PostAsync(url, null, cancellationToken: cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsNullOrEmpty() && message.IsSuccessStatusCode)
        {
            response = "{ }";
        }

        logger?.LogDebug($"POST {url}: {response}");
        TK? result = JsonSerializer.Deserialize<TK>(response, type);
        return result!;
    }

    /// <summary>
    /// Sends a GET request to the specified Uri and retrieves the response as a Blob.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Blob response message as the result.</returns>
    internal static async Task<Result<Blob?>> GetBlob(
       this HttpClient client,
       string url,
       JsonSerializerOptions options,
       CancellationToken cancellationToken,
       ILogger? logger = default)
    {
        logger?.LogDebug($"GET {url}");
        using var message = await client.GetAsync(url, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        var blob = await message.Content.ReadAsByteArrayAsync(cancellationToken);
        string response = await message.Content.ReadAsStringAsync(cancellationToken);

        logger?.LogDebug($"GET BLOB {url}: {response}");
        return new Blob(blob);
    }

    /// <summary>
    /// Sends a GET request to the specified Uri and decodes the response as a CAR (Content-Addressable Archive).
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <param name="progress">The progress reporter for the decoding process. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Success response message as the result.</returns>
    internal static async Task<Result<Success?>> GetCarAsync(
        this HttpClient client,
        string url,
        JsonSerializerOptions options,
        CancellationToken cancellationToken,
        ILogger? logger = default,
        OnCarDecoded? progress = null)
    {
        logger?.LogDebug($"GET {url}");
        using var message = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        await using var stream = await message.Content.ReadAsStreamAsync(cancellationToken);
        await CarDecoder.DecodeCarAsync(stream, progress);
        return new Success();
    }

    /// <summary>
    /// Sends a GET request to the specified Uri and downloads the response as a CAR (Content-Addressable Archive) file.
    /// </summary>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="filePath">The path where the file should be saved.</param>
    /// <param name="fileName">The name of the file to be saved.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Success response message as the result.</returns>
    internal static async Task<Result<Success?>> DownloadCarAsync(
        this HttpClient client,
        string url,
        string filePath,
        string fileName,
        JsonSerializerOptions options,
        CancellationToken cancellationToken,
        ILogger? logger = default)
    {
        logger?.LogDebug($"GET {url}");

        using var message = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        var fileDownload = Path.Combine(filePath, StringExtensions.GenerateValidFilename(fileName));
        await using (var content = File.Create(fileDownload))
        {
            await using var stream = await message.Content.ReadAsStreamAsync(cancellationToken);
            await stream.CopyToAsync(content, cancellationToken);
        }

        return new Success();
    }

    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type of the response body.</typeparam>
    /// <param name="client">The HttpClient instance.</param>
    /// <param name="url">The Uri the request is sent to.</param>
    /// <param name="type">The JsonTypeInfo of the response body.</param>
    /// <param name="options">The JsonSerializerOptions for the request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <param name="logger">The logger to use. This is optional and defaults to null.</param>
    /// <returns>The Task that represents the asynchronous operation. The value of the TResult parameter contains the Http response message as the result.</returns>
    internal static async Task<Result<T?>> Get<T>(
        this HttpClient client,
        string url,
        JsonTypeInfo<T> type,
        JsonSerializerOptions options,
        CancellationToken cancellationToken,
        ILogger? logger = default)
    {
        logger?.LogDebug($"GET {url}");
        using var message = await client.GetAsync(url, cancellationToken);
        if (!message.IsSuccessStatusCode)
        {
            ATError atError = await CreateError(message!, options, cancellationToken, logger);
            return atError!;
        }

        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        if (response.IsNullOrEmpty() && message.IsSuccessStatusCode)
        {
            response = "{ }";
        }

        logger?.LogDebug($"GET {url}: {response}");
        return JsonSerializer.Deserialize<T>(response, type);
    }

    private static async Task<ATError> CreateError(HttpResponseMessage message, JsonSerializerOptions options, CancellationToken cancellationToken, ILogger? logger = default)
    {
        string response = await message.Content.ReadAsStringAsync(cancellationToken);
        ATError atError;
        ErrorDetail? detail = default;
        if (string.IsNullOrEmpty(response))
        {
            atError = new ATError((int)message.StatusCode, detail);
        }
        else
        {
            try
            {
                detail = JsonSerializer.Deserialize<ErrorDetail>(response, ((SourceGenerationContext)options.TypeInfoResolver!).ErrorDetail);
                atError = new ATError((int)message.StatusCode, detail);
            }
            catch (Exception)
            {
                atError = new ATError((int)message.StatusCode, null);
            }
        }

        logger?.LogError($"ATError: {atError.StatusCode} {atError.Detail?.Error} {atError.Detail?.Message}");
        return atError;
    }
}
