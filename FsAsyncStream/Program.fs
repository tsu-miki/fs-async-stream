open System
open System.Net.Http
open System.IO
open System.Threading.Channels
open System.Threading.Tasks

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

let httpClient = new HttpClient()

let proxyStream1 (ctx: HttpContext) = task {
    use! res = httpClient.GetAsync("http://localhost:3000/stream", HttpCompletionOption.ResponseHeadersRead)
    ctx.Response.ContentType <- "text/event-stream"
    use src = res.Content.ReadAsStream()
    do! src.CopyToAsync(ctx.Response.Body)
}
let proxyStream2 (ctx: HttpContext) = task {
    use! res = httpClient.GetAsync("http://localhost:3000/stream", HttpCompletionOption.ResponseHeadersRead)
    ctx.Response.ContentType <- "text/event-stream"
    use src = new StreamReader(res.Content.ReadAsStream())
    let writer = new StreamWriter(ctx.Response.Body)
    while not src.EndOfStream do
        let! line = src.ReadLineAsync() |> Async.AwaitTask
        if not (String.IsNullOrWhiteSpace(line)) then
            do! writer.WriteLineAsync(line) |> Async.AwaitTask
            do! writer.FlushAsync() |> Async.AwaitTask
}

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()

    app.MapGet("/proxy-stream1", (fun (ctx: HttpContext) -> proxyStream1 ctx :> Task)) |> ignore
    app.MapGet("/proxy-stream2", (fun (ctx: HttpContext) -> proxyStream2 ctx :> Task)) |> ignore

    app.Run()

    0 // Exit code
