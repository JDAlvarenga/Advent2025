# Advent of Code 2025

Solutions to 2025 [Advent of Code](https://adventofcode.com)

The real inputs are to be placed in a file directly inside `input/` with the lowercase name of the solution class (i.e. `day01`)

| Problem                                                    | Input                                                                                   | Solution                                  |
|------------------------------------------------------------|-----------------------------------------------------------------------------------------|-------------------------------------------|
| [Secret Entrance](https://adventofcode.com/2025/day/1)     | [day01](Advent2025/input/samples/day01)                                                 | [Day01.cs](Advent2025/Solutions/Day01.cs) |
| [Gift Shop](https://adventofcode.com/2025/day/2)           | [day02](Advent2025/input/samples/day02)                                                 | [Day02.cs](Advent2025/Solutions/Day02.cs) |
| [Lobby](https://adventofcode.com/2025/day/3)               | [day03](Advent2025/input/samples/day03)                                                 | [Day03.cs](Advent2025/Solutions/Day03.cs) |
| [Printing Department](https://adventofcode.com/2025/day/4) | [day04](Advent2025/input/samples/day04)                                                 | [Day04.cs](Advent2025/Solutions/Day04.cs) |
| [Cafeteria](https://adventofcode.com/2025/day/5)           | [day05](Advent2025/input/samples/day05)                                                 | [Day05.cs](Advent2025/Solutions/Day05.cs) |
| [Trash Compactor](https://adventofcode.com/2025/day/6)     | [day06](Advent2025/input/samples/day06)                                                 | [Day06.cs](Advent2025/Solutions/Day06.cs) |
| [Laboratories](https://adventofcode.com/2025/day/7)        | [day07](Advent2025/input/samples/day07)                                                 | [Day07.cs](Advent2025/Solutions/Day07.cs) |
| [Playground](https://adventofcode.com/2025/day/8)          | [day08](Advent2025/input/samples/day08)                                                 | [Day08.cs](Advent2025/Solutions/Day08.cs) |
| [Movie Theater](https://adventofcode.com/2025/day/9)       | [day09](Advent2025/input/samples/day09)                                                 | [Day09.cs](Advent2025/Solutions/Day09.cs) |
| [Factory](https://adventofcode.com/2025/day/10)            | [day10](Advent2025/input/samples/day10)                                                 | [Day10.cs](Advent2025/Solutions/Day10.cs) |
| [Reactor](https://adventofcode.com/2025/day/11)            | [day11_1](Advent2025/input/samples/day11_1) [day11_2](Advent2025/input/samples/day11_2) | [Day11.cs](Advent2025/Solutions/Day11.cs) |

## Notes

* Part 2 of [day 10](Advent2025/Solutions/Day10.cs) attempts to solve the problem by recursively finding the minimum button presses to make all joltage values even, then divide by 2. This worked for all but one test case of the provided input. Said test case was solved by modeling the problem as a system of linear equations in an [online tool](https://online-optimizer.appspot.com/?model=builtin:default.mod)