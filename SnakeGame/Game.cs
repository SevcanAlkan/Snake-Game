using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    /// <summary>
    /// Manages all game flow and actions. Load() method makes the game to be ready for start.
    /// </summary>
    internal class Game
    {
        /// <summary>
        /// Overall score of the current game session
        /// </summary>
        public int Score { get; private set; }
        /// <summary>
        /// Returns true when tiles loaded and the game to be ready for start.
        /// </summary>
        public bool isLoaded { get; private set; } = false;
        public bool isStarted { get; private set; } = false;
        public bool isPaused { get; private set; } = false;
        public bool isGameOver { get; private set; } = false;

        private List<Button> tiles;
        private Timer timer;
        private readonly Form window;
        private Keys lastKeyInput;
        private MovementDirection MovementDirection;
        private bool isWindowInitilized;
        private int FPS;
        private int targetCount = 5;
        private List<Coordinate> targets;
        /// <summary>
        /// It stores X and Y coordinate values for each tile of the snake.
        /// </summary>
        private List<Coordinate> snakeTiles;

        /// <summary>
        /// Initializes Winform window and list of the tiles instances.
        /// </summary>
        /// <param name="window">Winform Form instance, to define window for game. Game going to load to this instance.</param>
        public Game(Form window)
        {
            if (window == null)
                isWindowInitilized = false;

            this.window = window;

            FPS = 20;
            timer = new Timer();
            timer.Interval = 1000 / FPS;
            timer.Tick += new EventHandler(Update);

            isWindowInitilized = true;
        }

        /// <summary>
        /// Loads variables instances of the game class. This method have to called before Start() method.
        /// </summary>
        public void Load()
        {
            //Create tiles on window
            if(!isStarted)
            {
                tiles = new List<Button>();
                CreateTiles();
            } else
            {
                ClearTiles();
            }

            snakeTiles = new List<Coordinate>();

            //Set initial coordinates for snake
            snakeTiles.Add(new Coordinate(13, 15)); // Head
            snakeTiles.Add(new Coordinate(13, 14));
            snakeTiles.Add(new Coordinate(13, 13));
            snakeTiles.Add(new Coordinate(13, 12));
            snakeTiles.Add(new Coordinate(13, 11));


            MovementDirection = MovementDirection.Down;

            targets = new List<Coordinate>();

            isStarted = false;
            isPaused = false;
            isLoaded = true;
            isGameOver = false;

            //Update one time
            Update(null, null);
        }

        #region Game Flow Control

        /// <summary>
        /// Basically starts the game.
        /// </summary>
        public void Run()
        {
            timer.Start();
            isStarted = true;
            isPaused = false;
        }

        /// <summary>
        /// Pauses game flow.
        /// </summary>
        public void Pause()
        {
            timer.Stop();
            isPaused = true;
        }

        /// <summary>
        /// Resets all instances for new game.
        /// </summary>
        public void Reset()
        {
            timer.Stop();
            Load();
        }

        public void SetUserInput(KeyEventArgs e)
        {
            lastKeyInput = e.KeyCode;
        }

        #endregion

        #region Game Render Methods

        private void Update(object sender, EventArgs e)
        {
            UpdateDirection();

            List<Coordinate> tempSnakeTiles = snakeTiles.ToList();
            Coordinate head = snakeTiles.FirstOrDefault();

            //Update head position
            Coordinate nextPosition = GetNext(head, MovementDirection);

            var isEatItself = snakeTiles.Any(a => a.X == nextPosition.X && a.Y == nextPosition.Y);
            if(isEatItself)
            {
                isGameOver = true;
                Pause();
                return;
            }

            snakeTiles.RemoveAt(0);
            snakeTiles.Insert(0, nextPosition);
            snakeTiles.Insert(1, head);
            snakeTiles.RemoveAt(snakeTiles.Count - 1);

            //Update color of tiles for the snake
            foreach (var item in snakeTiles)
            {
                SetColor(item, Color.Red);
            }

            //Clear color of tiles for which is not part of the snake anymore
            foreach (var item in tempSnakeTiles.Where(a => !snakeTiles.Any(x => x.Y == a.Y && x.X == a.X)).ToList())
            {
                SetColor(item, Color.Gray);
            }

            Coordinate eatenTarget = targets.Where(a => snakeTiles.Any(x => x.Y == a.Y && x.X == a.X)).FirstOrDefault();

            if (eatenTarget.X != 0 && eatenTarget.Y != 0)
            {
                Score = Score + 100;

                targets.Remove(eatenTarget);
            }

            GrowSnakeByScore();

            GenerateTargets();

            RenderTargets();

            var scoreLabel = window.Controls.Cast<Control>().FirstOrDefault(control => String.Equals(control.Name, "lblScoreValue"));
            scoreLabel.Text = Score.ToString();
        }

        private Coordinate GetNext(Coordinate current, MovementDirection movementDirection)
        {
            Coordinate newCoordinate = new Coordinate(current.X, current.Y);

            switch (movementDirection)
            {
                case MovementDirection.Up:

                    if (current.Y == 1)
                        newCoordinate.Update(null, 25);
                    else
                        newCoordinate.Update(null, current.Y - 1);

                    break;
                case MovementDirection.Down:

                    if (current.Y == 25)
                        newCoordinate.Update(null, 1);
                    else
                        newCoordinate.Update(null, current.Y + 1);

                    break;
                case MovementDirection.Left:

                    if (current.X == 1)
                        newCoordinate.Update(25, null);
                    else
                        newCoordinate.Update(current.X - 1, null);

                    break;
                case MovementDirection.Right:

                    if (current.X == 25)
                        newCoordinate.Update(1, null);
                    else
                        newCoordinate.Update(current.X + 1, null);

                    break;
                default:
                    break;
            }

            return newCoordinate;
        }

        private Coordinate GetNewTileCoordinateForSnake()
        {
            Coordinate newCoordinate = snakeTiles.Last();

            switch (MovementDirection)
            {
                case MovementDirection.Up:

                    if (newCoordinate.Y == 1)
                        newCoordinate.Update(null, 25);
                    else
                        newCoordinate.Update(null, newCoordinate.Y - 1);

                    break;
                case MovementDirection.Down:

                    if (newCoordinate.Y == 25)
                        newCoordinate.Update(null, 1);
                    else
                        newCoordinate.Update(null, newCoordinate.Y + 1);

                    break;
                case MovementDirection.Left:

                    if (newCoordinate.X == 1)
                        newCoordinate.Update(25, null);
                    else
                        newCoordinate.Update(newCoordinate.X - 1, null);

                    break;
                case MovementDirection.Right:

                    if (newCoordinate.X == 25)
                        newCoordinate.Update(1, null);
                    else
                        newCoordinate.Update(newCoordinate.X + 1, null);

                    break;
                default:
                    break;
            }

            return newCoordinate;
        }

        private void RenderTargets()
        {
            foreach (var item in targets)
            {
                SetColor(item, Color.Blue);
            }
        }

        private void UpdateDirection()
        {
            MovementDirection? newDirection = null;

            if (lastKeyInput == Keys.W && MovementDirection != MovementDirection.Down)
            {
                newDirection = MovementDirection.Up;
            }
            else if (lastKeyInput == Keys.S && MovementDirection != MovementDirection.Up)
            {
                newDirection = MovementDirection.Down;
            }
            else if (lastKeyInput == Keys.A && MovementDirection != MovementDirection.Right)
            {
                newDirection = MovementDirection.Left;
            }
            else if (lastKeyInput == Keys.D && MovementDirection != MovementDirection.Left)
            {
                newDirection = MovementDirection.Right;
            }

            if (newDirection.HasValue && newDirection.Value != MovementDirection)
            {
                MovementDirection = newDirection.Value;
            }
        }

        private void GenerateTargets()
        {
            int count = targetCount - targets.Count;
            bool isTargetCreated = false;

            if(count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    do
                    {
                        var coordinate = GetTargetCoordinate();

                        if (!snakeTiles.Any(a => a.X == coordinate.X && a.Y == coordinate.Y)
                            && !targets.Any(a => a.X == coordinate.X && a.Y == coordinate.Y))
                        {
                            targets.Add(coordinate);
                            isTargetCreated = true;
                        }

                    } while (!isTargetCreated);

                    isTargetCreated = false;
                }
            }
        }

        private void GrowSnakeByScore()
        {
            int tileCount = Score / 200;
            int tileCountShouldAdd = tileCount - snakeTiles.Count;

            if(tileCountShouldAdd > 0)
            {
                for (int i = 0; i < tileCountShouldAdd; i++)
                {
                    snakeTiles.Add(GetNewTileCoordinateForSnake());
                }
            }
        }

        private Coordinate GetTargetCoordinate()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            int xValue = random.Next(1, 25);
            int yValue = random.Next(1, 25);

            return new Coordinate(xValue, yValue);
        }

        private void SetColor(Coordinate coordinate, Color color)
        {
            string tag = String.Format("{0},{1}", coordinate.Y, coordinate.X);

            var btn = window.Controls.Cast<Control>().FirstOrDefault(control => String.Equals(control.Tag, tag));

            if (btn != null)
            {
                btn.BackColor = color;
            }
        }

        #endregion

        //-------------------------------------

        #region Helper Methods

        private bool CreateTiles_IsValid()
        {
            if (!isWindowInitilized)
                return false;

            return true;
        }

        private bool CreateTiles()
        {
            try
            {
                if (!CreateTiles_IsValid())
                    return false;

                int heightOffset = 0;
                int tileWidth = 25;

                for (int y = 1; y <= 25; y++)
                {
                    int withOffset = 0;

                    for (int x = 1; x <= 25; x++)
                    {
                        Button btn = new Button();
                        btn.Name = "btn" + y.ToString() + x.ToString();
                        btn.Text = ""; // String.Format("{0},{1}", y, x);
                        btn.Tag = String.Format("{0},{1}", y, x);
                        btn.Enabled = false;
                        btn.Width = tileWidth;
                        btn.Height = tileWidth;
                        btn.BackColor = Color.Gray;
                        btn.Location = new Point(withOffset + 15, heightOffset + 100);

                        window.Controls.Add(btn);
                        tiles.Add(btn);

                        withOffset = withOffset + tileWidth;
                    }

                    heightOffset = heightOffset + tileWidth;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ClearTiles_IsValid()
        {
            if (!isWindowInitilized || tiles == null || !tiles.Any())
                return false;

            return true;
        }

        private bool ClearTiles()
        {
            if (!ClearTiles_IsValid())
                return false;

            foreach (var tile in tiles)
            {
                tile.BackColor = Color.Gray;
            }

            return true;
        }

        #endregion
    }
}
