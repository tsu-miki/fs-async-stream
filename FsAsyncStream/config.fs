namespace Config

open FsConfig

type Config = {
    [<DefaultValue("test")>]
    ServerKey : string
}