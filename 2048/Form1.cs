using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _2048 {
    public partial class Form1 : Form {
        private const int BoardSizePx = 500;

        private readonly Board _board;

        public Form1() {
            InitializeComponent();
            //AllowTransparency = true;
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;

            _board = new Board();

            Width = BoardSizePx;
            Height = BoardSizePx;
        }

        private void Form1_Load(object sender, EventArgs e) {
            _board.StartGame();


        }
        private Dictionary<Keys, Direction> Keymap = new Dictionary<Keys, Direction>() {
            {Keys.Left, Direction.Left},
            {Keys.Right, Direction.Right},
            {Keys.Up, Direction.Up},
            {Keys.Down, Direction.Down},
        }; 

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
            if (Keymap.ContainsKey(e.KeyCode))  {
                _board.Shift(Keymap[e.KeyCode]);
                Invalidate();
            }

        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            _board.Paint(g);
        }


    }

    public class Board {
        public const int BoardSizePx = 500;

        public const int BorderBezelPx = 5;
        private static readonly Color Background = Color.FromArgb(0xbb, 0xad, 0xa0);

        private readonly List<Tile> _tiles = new List<Tile>();
        private Random _rand;

        public Board() {
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    _tiles.Add(new Tile(x, y));
                }
            }

        }

        public void StartGame() {
            _rand = new Random();
            var p1 = _rand.Next(_tiles.Count);
            var p2 = _rand.Next(_tiles.Count);
            while (p2 == p1) {
                p2 = _rand.Next(_tiles.Count);
            }

            _tiles[p1].Value = 2;
            _tiles[p2].Value = 2;

            //GetTile(new Point(1, 0)).Value = 2;
        }

        public void Paint(Graphics g) {
            var brush = new SolidBrush(Background);

            var path = RoundedRectangle.Create(0, 0, BoardSizePx, BoardSizePx, BorderBezelPx);
            g.FillPath(brush, path);

            foreach (var tile in _tiles) {
                tile.Paint(g);
            }
        }

        public void Shift(Direction dir) {
            switch (dir) {
                case Direction.Left:
                    for (int x = 0; x < 4; x++) {
                        for (int y = 0; y < 4; y++) {
                            MoveToStop(GetTile(x, y), Direction.Left);
                        }
                    }
                    break;
                case Direction.Right:
                    for (int x = 3; x >= 0; x--) {
                        for (int y = 0; y < 4; y++) {
                            MoveToStop(GetTile(x, y), Direction.Right);
                        }
                    }
                    break;
                case Direction.Up:
                    for (int y = 3; y >= 0; y--) {
                        for (int x = 0; x < 4; x++) {
                            MoveToStop(GetTile(x, y), Direction.Up);
                        }
                    }
                    break;
                case Direction.Down:
                    for (int y = 0; y < 3; y++) {
                        for (int x = 0; x < 4; x++) {
                            MoveToStop(GetTile(x, y), Direction.Down);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
        }

        private void MoveToStop(Tile tile, Direction dir) {
            var done = false;
            while (!done && GetStopCondition(tile, dir)) {
                var neighbor = GetNeighbor(tile, dir);
                done = Combine(neighbor, tile);
                tile = neighbor;
            }
        }

        private static bool GetStopCondition(Tile tile, Direction dir) {
            switch (dir) {
                case Direction.Left:
                    return tile.Position.X > 0;
                case Direction.Right:
                    return tile.Position.X < 3;
                case Direction.Up:
                    return tile.Position.Y > 0;
                case Direction.Down:
                    return tile.Position.Y < 3;
                default:
                    return false;
            }
        }


        private bool Combine(Tile dest, Tile src) {
            if (dest.Value == 0 && src.Value > 0) {
                GetTile(dest.Position).Value = src.Value;
                GetTile(src.Position).Value = 0;
                return false;
            }
            if (src.Value == dest.Value) {
                GetTile(dest.Position).Value = dest.Value * 2;
                GetTile(src.Position).Value = 0;
                return true;
            }
            return false;
        }

        private Tile GetTile(Point pos) { return _tiles.First(t => t.Position == pos); }
        private Tile GetTile(int x, int y) { return GetTile(new Point(x, y)); }

        private Tile GetNeighbor(Tile tile, Direction dir) {
            switch (dir) {
                case Direction.Left:
                    return GetTile(tile.Position.X - 1, tile.Position.Y);
                case Direction.Right:
                    return GetTile(tile.Position.X + 1, tile.Position.Y);
                case Direction.Up:
                    return GetTile(tile.Position.X, tile.Position.Y - 1);
                case Direction.Down:
                    return GetTile(tile.Position.X, tile.Position.Y + 1);
                default:
                    return tile;
            }
        }
    }

    public enum Direction {
        Left, Right, Up, Down
    }

    public class Tile {
        public const int TileBezelPx = 2;
        public const int TileSpacingPx = 15;
        public const int TileSizePx = 107;

        private static readonly Dictionary<int, Color> Colors = new Dictionary<int, Color>() {
            { 0, Color.FromArgb(0xcc, 0xc0, 0xb3)},
            {2, Color.FromArgb(0xee, 0xe4, 0xda)},
            {4, Color.FromArgb(0xed, 0xe0, 0xc8)},
            {8, Color.FromArgb(0xf2, 0xb1, 0x79)},
            {16, Color.FromArgb(0xf5, 0x95, 0x63)},
            {32, Color.FromArgb(0xf6, 0x7c, 0x5f)},
            {64, Color.FromArgb(0xf6, 0x5e, 0x3b)},
            {128, Color.FromArgb(0xed, 0xf7, 0x72)},
            {256, Color.FromArgb(0xed, 0xcc, 0x61)},
            {512, Color.FromArgb(0xed, 0xc8, 0x50)},
            {1024, Color.FromArgb(0xed, 0xc5, 0x3f)},
            {2048, Color.FromArgb(0xed, 0xc2, 0x2e)},
        };
        private static readonly Dictionary<int, Color> LabelColors = new Dictionary<int, Color>() {
            { 0, Color.FromArgb(0, 0, 0)},
            {2, Color.FromArgb(0x77, 0x6e, 0x65)},
            {4, Color.FromArgb(0x77, 0x6e, 0x65)},
            {8, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {16, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {32, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {64, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {128, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {256, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {512, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {1024, Color.FromArgb(0xf9, 0xf6, 0xf2)},
            {2048, Color.FromArgb(0xf9, 0xf6, 0xf2)},
        };

        private readonly Rectangle _rectangle;

        public int Value { get; set; }
        public Point Position { get; set; }

        public override string ToString() { return Position.ToString() + ": " + Value; }

        public Tile(int x, int y) {
            Value = 0;
            Position = new Point(x, y);

            _rectangle = new Rectangle(TileSpacingPx + (TileSizePx + TileSpacingPx) * Position.X, TileSpacingPx + (TileSizePx + TileSpacingPx) * Position.Y, TileSizePx, TileSizePx);
        }

        public void Paint(Graphics g) {
            var path = RoundedRectangle.Create(_rectangle, TileBezelPx);

            g.FillPath(new SolidBrush(Colors[Value]), path);

            if (Value > 0) {
                var sf = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Value.ToString(), new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold), new SolidBrush(LabelColors[Value]), _rectangle, sf);
            }
        }
    }
}
