using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _2048 {
    public partial class Form1 : Form {
        public const int TileSpacingPx = 15;
        public const int BorderBezelPx = 5;
        
        public const int TileSizePx = 107;
        public const int BoardSizePx = 500;

        private List<List<Tile>> _tiles;

        public Form1() {
            InitializeComponent();
            //AllowTransparency = true;
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;

            _tiles = new List<List<Tile>>() {
                new List<Tile>(){new Tile(), new Tile(), new Tile(), new Tile()},
                new List<Tile>(){new Tile(), new Tile(), new Tile(), new Tile()}, 
                new List<Tile>(){new Tile(), new Tile(), new Tile(), new Tile()},
                new List<Tile>(){new Tile(), new Tile(), new Tile(), new Tile()}
            };

            Width = BoardSizePx;
            Height = BoardSizePx;
        }

        private void Form1_Load(object sender, EventArgs e) {

            
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            var brush = new SolidBrush(Colors.Background);

            var path = RoundedRectangle.Create(ClientRectangle, BorderBezelPx);
            g.FillPath(brush, path);

            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {

                    var left = TileSpacingPx + (TileSizePx + TileSpacingPx)*x;
                    var top = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;

                    _tiles[x][y].Paint(g, left, top);
                }
            }
        }

        
    }

    static class Colors {
        public static readonly Color Background = Color.FromArgb(0xbb, 0xad, 0xa0);
        public static readonly Color EmptySquare = Color.FromArgb(0xcc, 0xc0, 0xb3);
    }

    public class Tile {
        public const int TileBezelPx = 2;

        public SolidBrush Brush { get; set; }
        public int Value { get; set; }

        public Tile() {
            Value = 0;
            Brush = new SolidBrush(Colors.EmptySquare);
        }

        public void Paint(Graphics g, int x, int y) {
            var path = RoundedRectangle.Create(x, y, Form1.TileSizePx, Form1.TileSizePx, TileBezelPx);

            g.FillPath(Brush, path);
        }
    }
}
