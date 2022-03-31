module Lab5.Integrate

open System
open MatrixArithmetic
open Microsoft.FSharp.Core


let rec Process degree =
    match degree with
    | 0 -> Polynom([ 1. ])
    | 1 -> Polynom([ 0.; 1. ])
    | n ->
        2. * Polynom([ 0.; 1. ]) * (Process(n - 1))
        - Process(n - 2)

type ChebyshevT(degree: uint) =

    member _.Roots() =
        seq { for item in 0u .. (degree - 1u) -> cos (Math.PI * (2. * float item + 1.) / (2. * float degree)) }
        |> Seq.cache

let integrate (left, right, nodes) func =
    let roots = ChebyshevT(nodes).Roots()
    let len = (right - left) / 2.
    let mid = (right + left) / 2.
    let sum =
        seq { for r in roots -> sqrt (1. - r ** 2.) * func (len * r + mid) }
        |> Seq.sum

    len * Math.PI * sum / float nodes


let rec integral (l, r, nodes) func (split: int option) =
    let split = if split.IsNone then 2 else split.Value
    let whole = integrate (l, r, nodes) func
    let step = (r - l) / float split

    let windowedSum =
        seq { 0 .. split }
        |> Seq.windowed 2
        |> Seq.map (fun item -> integrate (l + float item.[0] * step, l + float item.[1] * step, nodes) func)
        |> Seq.sum



    if abs (whole - windowedSum) < 1e-5 then
        min whole windowedSum
    else
        integral (l, r, 600u) func (Some(2 * split))
