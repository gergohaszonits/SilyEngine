using SilyEngine.Abstractions.Services;
using SilyEngine.Terminal;
using SilyEngine.TerminalSnake;


var snakeGame = 
new SilyEngineRunner<SnakeGameLogic, TerminalGraphicDrawer, TerminalInputReader>(tickRate: 144);

await snakeGame.RunAsync(async(a, b) => 
{
    //kód ami minden ticknel fut
});