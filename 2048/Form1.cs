using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly Dictionary<Keys, Direction> _keymap = new Dictionary<Keys, Direction>() {
            {Keys.Left, Direction.Left},
            {Keys.Right, Direction.Right},
            {Keys.Up, Direction.Up},
            {Keys.Down, Direction.Down},
        }; 

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
            if (_keymap.ContainsKey(e.KeyCode))  {
                _board.Move(_keymap[e.KeyCode]);
                Invalidate();
            }

        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            _board.Paint(g);
        }


    }

    public enum Direction {
        Left, Right, Up, Down
    }
}
