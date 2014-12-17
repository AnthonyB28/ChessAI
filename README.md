# Chess Artificial Inteligence

Marist Artificial Intelligence class final project.  
*Anthony Barranco & Phil Picinic*

### Move Search
Bunduru Chess AI uses a multithreaded Negascout (or Principal Variation Search) 
with a Quiescence Search as a secondary search.   
Negascout search builds upon the 
NegaMax framework which is a simplification of minimax search and uses alpha-beta
pruning and null window searches. It uses an MVV/LVA (most valuable victim-
least valuable attacker) sorting of moves to create optimal pruning. 
Quiescence search is our secondary search that starts when Negascout reaches the 
end at depth 0 to alleviate a horizon effect.  
Quiescence search only considers 
capture moves that are always sorted by MVV/LVA as well as alpha-beta pruning. 

The AI uses a dynamic depth that is gauged by a guess on the current and previous 
search’s branching factor and the time taken during the last search. Dynamic depth is 
also adjusted by minimum and maximum depth margins decided by the state of the game: early,
 middle, end, and late end. The states of the game are decided by the number of pieces on the board remaining.
 
 ### Evaluation
The evaluation function considers the entire board state. All pieces have assigned 
value, and the totals are added together per color. Eval has many factors besides 
just material values such as mid game and end game bonuses, isolated and doubled pawn files, 
piece location tables, attack and defend bonuses, and offset values for checks and checkmates.

### Compile
1. To compile the code on [Windows] either:
 * Import the project in [Visual Studio] 2013 and build (sln file)
  * Compile .NET 4.5 [directly]

2. To compile the code on Mac, [Mono] must be installed.
  * *Please note that it has not been tested on Mac with Mono.*

### Running the Program
Create a game at: http://bencarle.com/chess/newgame

The engine can run by running the executable with the following command line arguments: 
1. team color (true for white, false for black)
2. the game id
3. the team id
4. the secret id
 
The following is an example of team 1 playing as white in game 638:  
*ChessAI.exe true 638 1 3268cae*

## Why Use C#?

[C#] is an elegant, simple, type-safe object oriented language.   
It supports a huge array of easy-to-use libraries built right into [.NET] from [Microsoft] as well. From simple to use multithreaded functions to native Windows GUI manipulation and features.

The [.NET] foundation is also working to foster open development and pushing open source. [Microsoft] is actively building on [.NET] and pushing development all the time.
http://www.dotnetfoundation.org/

[Mono]: http://www.mono-project.com/docs/about-mono/supported-platforms/osx/
[Windows]: http://www.microsoft.com/net/downloads
[Visual Studio]:http://msdn.microsoft.com/en-us/vstudio/aa718325.aspx
[directly]: http://msdn.microsoft.com/en-us/library/ms172492%28v=vs.80%29.aspx
[Microsoft]: http://www.microsoft.com/net
[.NET]: http://www.dotnetfoundation.org/
[C#]: http://en.wikipedia.org/wiki/C_Sharp_%28programming_language%29
