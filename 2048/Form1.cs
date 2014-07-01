using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _2048 {
    public partial class Form1 : Form {
        private const int BoardSizePx = 500;

        private readonly Board _board;
        private BoardState _lastState;
        private BoardState _currentState;

        private BoardRenderer _renderer;
        Timer _timer = new Timer();

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
            _lastState = _board.GetState();
            _renderer = new BoardRenderer(_board.Size);
            _timer = new Timer();
            _timer.Interval = (int) (1000.0f/60);
            _timer.Tick += TimerOnTick;
            _timer.Start();

            _renderer.Animate(_lastState);

        }

        private void TimerOnTick(object sender, EventArgs eventArgs) {
            _renderer.Tick();
            Invalidate();
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
            if (_renderer.Animating) {
                return;
            }
            if (_keymap.ContainsKey(e.KeyCode)) {
                _board.Move(_keymap[e.KeyCode]);
                _currentState = _board.GetState();

                _renderer.Animate(_currentState);
            }

        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            var g = e.Graphics;
            _renderer.Render(g);
        }


    }

    public enum Direction {
        Left, Right, Up, Down
    }
}
