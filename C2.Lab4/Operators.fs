module C2.Lab4.Operators

let ( .* ) (left: float) (a1: float, a2: float) = (left * a1, left * a2)
let ( ++ ) (a1: float, a2: float) (b1: float, b2: float) = (a1 + b1, a2 + b2)
let ( -- ) (a1: float, a2: float) (b1: float, b2: float) = (a1 - b1, a2 - b2)
