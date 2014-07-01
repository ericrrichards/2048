using System;
using System.Drawing;

namespace _2048 {
    public class Tile {
        public int Value { get; private set; }
        public Point Position { get; set; }
        public Point? PreviousPosition { get; set; }
        public Tuple<Tile, Tile> MergedFrom { get; set; }
        public bool NewTile { get; set; }

        public override string ToString() { return Position + ": " + Value; }

        public Tile(int x, int y) {
            Value = 0;
            Position = new Point(x, y);

            PreviousPosition = null;
            MergedFrom = null;
        }

        public Tile(Point position, int value) {
            Value = value;
            Position = position;

            PreviousPosition = null;
            MergedFrom = null;
        }

        public void SavePosition() {
            PreviousPosition = Position;       
        }

    }
}