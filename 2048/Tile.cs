using System;
using System.Collections.Generic;
using System.Drawing;

namespace _2048 {
    public class Tile {
        private const int TileBezelPx = 2;
        private const int TileSpacingPx = 15;
        private const int TileSizePx = 107;

        private static readonly Dictionary<int, Color> Colors = new Dictionary<int, Color>() {
            { 0, Color.FromArgb(0xcc, 0xc0, 0xb3)},
            {2, Color.FromArgb(0xee, 0xe4, 0xda)},
            {4, Color.FromArgb(0xed, 0xe0, 0xc8)},
            {8, Color.FromArgb(0xf2, 0xb1, 0x79)},
            {16, Color.FromArgb(0xf5, 0x95, 0x63)},
            {32, Color.FromArgb(0xf6, 0x7c, 0x5f)},
            {64, Color.FromArgb(0xf6, 0x5e, 0x3b)},
            {128, Color.FromArgb(0xed, 0xcf, 0x72)},
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


        public int Value { get; set; }
        public Point Position { get; set; }
        public Point? PreviousPosition { get; set; }
        public Tuple<Tile, Tile> MergedFrom { get; set; }

        public override string ToString() { return Position.ToString() + ": " + Value; }

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

        public void Paint(Graphics g) {
            var rectangle = new Rectangle(TileSpacingPx + (TileSizePx + TileSpacingPx) * Position.X, TileSpacingPx + (TileSizePx + TileSpacingPx) * Position.Y, TileSizePx, TileSizePx);
            var path = RoundedRectangle.Create(rectangle, TileBezelPx);

            g.FillPath(new SolidBrush(Colors[Value]), path);

            if (Value > 0) {
                var sf = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                Font font;
                if (Value < 128) {
                    font = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
                } else if (Value < 1024) {
                    font = new Font(FontFamily.GenericSansSerif, 36, FontStyle.Bold);
                } else {
                    font = new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold);
                }
                g.DrawString(Value.ToString(), font, new SolidBrush(LabelColors[Value]), rectangle, sf);
            }
        }
    }
}