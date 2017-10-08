# Symbolic Algebra Library #

Portable computer algeraic calculation library written entirely in C#.

Highly embedded into your application and can be used in your mathematical projects.

# Features #

- Basic Mathematical Operations 
- Derivations
- Trigonoemtric Simplification


## Quick Usage ##
Use **Parse** Method


`var p = SymbolicVariable.Parse("x+x*y+(a-b)^3");`

`Console.WriteLine(p.ToString());`


### Derivation ###

`p.Differentiate("x");` which carry differentiation to the x parameter.
or use `|` operator in the **Parse** method.

`var p = SymbolicVariable.Parse("(sin(x)^3+cos(x)^5-x^3*y-3) | x");`








 