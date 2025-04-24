open System
open System.Net.Http
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

open FsConfig

open Config

type MessageJson = { role: string; content: string }

type LLMRequestJson =
    { model: string
      messages: MessageJson
      stream: bool }

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore


    app.MapGet(
        "/conversations",
        Func<Task<string>>(fun () ->
            async {
                let config =
                  match EnvConfig.Get<Config>() with
                  | Ok config -> config
                  | Error error -> failwith "Failed to load configuration"

                printfn "%A" config.ServerKey
                // use client = new HttpClient()
                // client.DefaultRequestHeaders.Add("Authorization","Bearer ${API_KEY}")

                // let request =
                //     { model = "gpt-4o-mini"
                //       messages =
                //         { role = "user"
                //           content = "Hello, how can I help you?" }
                //       stream = true }
                // let! response = clinet.PostAsync()
                return "Hello World!"
            }
            |> Async.StartAsTask)
    )
    |> ignore

    app.Run()

    0 // Exit code
