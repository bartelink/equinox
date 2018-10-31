﻿module Equinox.Cosmos.Integration.VerbatimUtf8JsonConverterTests

open Equinox.Cosmos
open Newtonsoft.Json
open Swensen.Unquote
open System
open Xunit

let inline serialize (x:'t) =
    let serializer = new JsonSerializer()
    use sw = new System.IO.StringWriter()
    use w = new JsonTextWriter(sw)
    serializer.Serialize(w,x)
    sw.ToString()

type Embedded = { embed : string }
type Union =
    | A of Embedded
    | B of Embedded
    interface TypeShape.UnionContract.IUnionContract

[<Fact>]
let ``VerbatimUtf8JsonConverter serializes properly`` () =
    let unionEncoder = Equinox.UnionCodec.JsonUtf8.Create<_>(JsonSerializerSettings())
    let encoded = unionEncoder.Encode(A { embed = "\"" })
    let e : Store.Event =
        {   id = null
            p = null
            s = null
            i = Nullable 0L
            c = DateTimeOffset.MinValue
            t = encoded.caseName
            d = encoded.payload
            m = null }
    let res = serialize e
    test <@ res.Contains """"d":{"embed":"\""}""" @>