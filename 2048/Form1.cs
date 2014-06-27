using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
            if (e.KeyCode == Keys.Left) {
                _board.ShiftLeft();
                Invalidate();
            }
            if (e.KeyCode == Keys.Right) {
                _board.ShiftRight();
                Invalidate();
            }
            if (e.KeyCode == Keys.Up) {
                _board.ShiftUp();
                Invalidate();
            }
            if (e.KeyCode == Keys.Down) {
                _board.ShiftDown();
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
                    _tiles.Add(new Tile(x,y));
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

        public void ShiftLeft() {
            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    MoveToStopLeft(GetTile(x, y));
                }
            }
        }
        public void ShiftRight() {
            for (int x = 3; x >= 0; x--) {
                for (int y = 0; y < 4; y++) {
                    MoveToStopRight(GetTile(x, y));
                }
            }
        }
        public void ShiftUp() {
                for (int y = 3; y >= 0; y--) {
                    for (int x = 0; x < 4; x++) {
                    MoveToStopUp(GetTile(x, y));
                }
            }
        }
        public void ShiftDown() {
            for (int y = 0; y < 3; y++) {
                for (int x = 0; x < 4; x++) {
                    MoveToStopDown(GetTile(x, y));
                }
            }
        }

        private void MoveToStopLeft(Tile tile) {
            var done = false;
            while (!done && tile.Position.X > 0) {
                var neighbor = GetTile(tile.Position.X - 1, tile.Position.Y);
                done = Combine(neighbor, tile);
                tile = neighbor;
            }
        }
        private void MoveToStopRight(Tile tile) {
            var done = false;
            while (!done && tile.Position.X < 3) {
                var neighbor = GetTile(tile.Position.X + 1, tile.Position.Y);
                done = Combine(neighbor, tile);
                tile = neighbor;
            }
        }
        private void MoveToStopUp(Tile tile) {
            var done = false;
            while (!done && tile.Position.Y > 0) {
                var neighbor = GetTile(tile.Position.X, tile.Position.Y-1);
                done = Combine(neighbor, tile);
                tile = neighbor;
            }
        }
        private void MoveToStopDown(Tile tile) {
            var done = false;
            while (!done && tile.Position.Y < 3) {
                var neighbor = GetTile(tile.Position.X, tile.Position.Y + 1);
                done = Combine(neighbor, tile);
                tile = neighbor;
            }
        }

        private bool Combine(Tile dest, Tile src) {
            if (dest.Value == 0 && src.Value > 0) {
                GetTile(dest.Position).Value = src.Value;
                GetTile(src.Position).Value = 0;
                return false;
            }
            if (src.Value == dest.Value) {
                GetTile(dest.Position).Value = dest.Value*2;
                GetTile(src.Position).Value = 0;
                return true;
            }
            return false;
        }

        private Tile GetTile(Point pos) { return _tiles.First(t => t.Position == pos); }
        private Tile GetTile(int x, int y) { return GetTile(new Point(x, y)); }
    }

    public class Tile {
        public const int TileBezelPx = 2;
        public const int TileSpacingPx = 15;
        public const int TileSizePx = 107;

        private static readonly Dictionary<int, Color> Colors = new Dictionary<int, Color>() {
            { 0, Color.FromArgb(0xcc, 0xc0, 0xb3)},
            {2, Color.FromArgb(0xee, 0xe4, 0xda)},
            {4, Color.FromArgb(0xed, 0xe0, 0xc8)}
        };
        private static readonly Dictionary<int, Color> LabelColors = new Dictionary<int, Color>() {
            { 0, Color.FromArgb(0, 0, 0)},
            {2, Color.FromArgb(0x77, 0x6e, 0x65)},
            {4, Color.FromArgb(0x77, 0x6e, 0x65)}
        };

        private readonly Rectangle _rectangle;

        public int Value { get; set; }
        public Point Position { get; set; }

        public override string ToString() { return Position.ToString() + ": " + Value; }

        public Tile(int x, int y) {
            Value = 0;
            Position = new Point(x,y);

            _rectangle = new Rectangle(TileSpacingPx + (TileSizePx + TileSpacingPx)*Position.X, TileSpacingPx + (TileSizePx + TileSpacingPx)*Position.Y, TileSizePx, TileSizePx);
        }

        public void Paint(Graphics g) {
            var path = RoundedRectangle.Create(_rectangle, TileBezelPx);

            g.FillPath(new SolidBrush(Colors[Value]), path);

            if (Value > 0) {
                var sf = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(Value.ToString(), new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold), new SolidBrush(LabelColors[Value]), _rectangle, sf  );
            }
        }
    }
}
