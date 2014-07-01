using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace _2048 {
    public class BoardRenderer {
        class TileSprite {
            public Point Position { get; set; }
            public int Value { get; set; }
            public Point Destination { get; set; }
        }


        private const int BoardSizePx = 500;
        private const int BorderBezelPx = 5;

        private const int TileBezelPx = 2;
        private const int TileSpacingPx = 15;
        private const int TileSizePx = 107;
        private static readonly Color Background = Color.FromArgb(0xbb, 0xad, 0xa0);
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

        public bool Animating { get; set; }

        private Rectangle _tileRect;
        private Font _lgFont;
        private Font _mdFont;
        private Font _smFont;

        private List<TileSprite> _sprites = new List<TileSprite>();
        private List<TileSprite> _delayedSprites = new List<TileSprite>(); 
        private int _timeStep = 0;
        private readonly SolidBrush _backgroundBrush;
        private readonly SolidBrush _emptyTileBrush;
        private GraphicsPath _tilePath;
        public int Size { get; set; }

        public BoardRenderer(int size) {
            Size = size;
            _tileRect = new Rectangle(0, 0, TileSizePx, TileSizePx);
            _tilePath = RoundedRectangle.Create(_tileRect, TileBezelPx);
            _lgFont = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
            _mdFont = new Font(FontFamily.GenericSansSerif, 36, FontStyle.Bold);
            _smFont = new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold);

            _backgroundBrush = new SolidBrush(Background);
            _emptyTileBrush = new SolidBrush(Colors[0]);
        }




        public void Tick() {
            if (!Animating) {
                _timeStep = 0;
                return;
            }
            var done = true;
            _timeStep += 1;
            foreach (var sprite in _sprites) {
                if (sprite.Destination != sprite.Position) {
                    var dx = sprite.Destination.X - sprite.Position.X;
                    var dy = sprite.Destination.Y - sprite.Position.Y;

                    var speed = 10.0f;
                    if (dx > 0) {
                        sprite.Position = new Point((int) Math.Min(sprite.Position.X + (dx/(speed-_timeStep)), sprite.Destination.X), sprite.Position.Y);
                    } else if (dx < 0) {
                        sprite.Position = new Point((int) Math.Max(sprite.Position.X + (dx/(speed-_timeStep)), sprite.Destination.X), sprite.Position.Y);
                    }
                    if (dy > 0) {
                        sprite.Position = new Point(sprite.Position.X, (int) Math.Min(sprite.Position.Y + (dy/(speed-_timeStep)), sprite.Destination.Y));
                    } else if (dy < 0) {
                        sprite.Position = new Point(sprite.Position.X, (int) Math.Max(sprite.Position.Y + (dy/(speed-_timeStep)), sprite.Destination.Y));
                    }

                    done = false;
                }
            }
            if (done) {
                Animating = false;
            }

        }


        public void Animate(BoardState state) {
            Animating = true;
            _sprites.Clear();
            for (var x = 0; x < state.Tiles.Length; x++) {
                for (var y = 0; y < state.Tiles[x].Length; y++) {
                    var tile = state.Tiles[x][y];
                    if (tile.MergedFrom != null) {
                        var stay = tile.MergedFrom.Item2;
                        var moving = tile.MergedFrom.Item1;

                        int left = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        int top = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;
                        var destX = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        var destY = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;
                        _sprites.Add(new TileSprite() { Position = new Point(left, top), Value = stay.Value, Destination = new Point(destX, destY) });

                        _delayedSprites.Add(new TileSprite() { Position = new Point(left, top), Value = tile.Value, Destination = new Point(destX, destY) });

                        var previous = moving.PreviousPosition.GetValueOrDefault();
                        left = TileSpacingPx + (TileSizePx + TileSpacingPx) * previous.X;
                        top = TileSpacingPx + (TileSizePx + TileSpacingPx) * previous.Y;
                        _sprites.Add(new TileSprite() { Position = new Point(left, top), Value = moving.Value, Destination = new Point(destX, destY) });

                    }
                    if (tile.Value > 0 && !tile.NewTile && tile.MergedFrom == null) {
                        var previousPosition = tile.PreviousPosition;
                        int left = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        int top = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;
                        if (previousPosition != null) {
                            left = TileSpacingPx + (TileSizePx + TileSpacingPx) * previousPosition.Value.X;
                            top = TileSpacingPx + (TileSizePx + TileSpacingPx) * previousPosition.Value.Y;
                        }
                        var destX = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        var destY = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;

                        _sprites.Add(new TileSprite() { Position = new Point(left, top), Value = tile.Value, Destination = new Point(destX, destY) });

                    } else if (tile.Value > 0 && tile.NewTile) {
                        int left = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        int top = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;
                        
                        var destX = TileSpacingPx + (TileSizePx + TileSpacingPx) * x;
                        var destY = TileSpacingPx + (TileSizePx + TileSpacingPx) * y;

                        _delayedSprites.Add(new TileSprite() { Position = new Point(left, top), Value = tile.Value, Destination = new Point(destX, destY) });
                    }
                }
            }
        }

        public void Render(Graphics g) {
            PaintBackground(g);
            PaintSprites(g);
        }

        private void PaintSprites(Graphics g) {
            foreach (var sprite in _sprites) {
                g.TranslateTransform(sprite.Position.X, sprite.Position.Y);
                g.FillPath(new SolidBrush(Colors[sprite.Value]), _tilePath);
                var sf = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                Font font;
                if (sprite.Value < 128) {
                    font = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
                } else if (sprite.Value < 1024) {
                    font = new Font(FontFamily.GenericSansSerif, 36, FontStyle.Bold);
                } else {
                    font = new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold);
                }
                g.DrawString(sprite.Value.ToString(), font, new SolidBrush(LabelColors[sprite.Value]), _tileRect, sf);


                g.Transform = new Matrix();
            }
            if (!Animating) {
                foreach (var sprite in _delayedSprites) {
                    g.TranslateTransform(sprite.Position.X, sprite.Position.Y);
                    g.FillPath(new SolidBrush(Colors[sprite.Value]), _tilePath);
                    var sf = new StringFormat {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    Font font;
                    if (sprite.Value < 128) {
                        font = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold);
                    } else if (sprite.Value < 1024) {
                        font = new Font(FontFamily.GenericSansSerif, 36, FontStyle.Bold);
                    } else {
                        font = new Font(FontFamily.GenericSansSerif, 28, FontStyle.Bold);
                    }
                    g.DrawString(sprite.Value.ToString(), font, new SolidBrush(LabelColors[sprite.Value]), _tileRect, sf);


                    g.Transform = new Matrix();
                }
                _sprites.AddRange(_delayedSprites);
                _delayedSprites.Clear();
            }
        }

        private void PaintBackground(Graphics g) {


            var path = RoundedRectangle.Create(0, 0, BoardSizePx, BoardSizePx, BorderBezelPx);
            g.FillPath(_backgroundBrush, path);
            for (var x = 0; x < Size; x++) {
                for (var y = 0; y < Size; y++) {
                    g.TranslateTransform(TileSpacingPx + (TileSizePx + TileSpacingPx) * x, TileSpacingPx + (TileSizePx + TileSpacingPx) * y);
                    g.FillPath(_emptyTileBrush, _tilePath);
                    g.Transform = new Matrix();
                }
            }

        }
    }
}