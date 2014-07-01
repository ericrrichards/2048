using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _2048 {
    public class BoardState {
        public Tile[][] Tiles { get; set; }
        public bool Won { get; set; }
        public bool Lost { get; set; }
        public bool KeepPlaying { get; set; }

        public BoardState(int size) {
            Tiles = new Tile [size][];
            for (int i = 0; i < size; i++) {
                Tiles[i] = new Tile[size];
            }
            Won = false;
            Lost = false;
            KeepPlaying = false;
        }
    }

    public class Board {
        private readonly Tile[][] _tiles;
        private Random _rand;

        public bool Over { get; set; }
        public bool Won { get; set; }
        public bool KeepPlaying { get; set; }

        public int Score { get; set; }

        public int Size { get; private set; }

        public Board(int size=4) {
            Size = size;
            _tiles = new Tile[Size][];
            for (var x = 0; x < Size; x++) {
                _tiles[x] = new Tile[Size];
                for (var y = 0; y < Size; y++) {
                    _tiles[x][y] = new Tile(x,y);
                }
            }
        }

        public BoardState GetState() {
            var ret = new BoardState(Size);
            for (var x = 0; x < Size; x++) {
                for (var y = 0; y < Size; y++) {
                    ret.Tiles[x][y] = GetTile(x, y);
                }
            }
            ret.Won = Won;
            ret.Lost = Over;
            ret.KeepPlaying = KeepPlaying;

            return ret;
        }
        

        public void StartGame() {
            _rand = new Random();

            Won = false;
            Over = false;
            KeepPlaying = false;
            Score = 0;

            AddRandomTile(false);
            AddRandomTile(false);

        }

        public void Move(Direction dir) {

            if (IsGameTerminated()) {
                return;
            }

            var vector = GetVector(dir);
            var traversals = BuildTraversals(vector);
            var moved = false;

            PrepareTiles();

            foreach (var x in traversals.Item1) {
                foreach (var y in traversals.Item2) {
                    var cell = new Point(x, y);
                    var tile = GetTile(cell);
                    if (tile != null && tile.Value > 0) {
                        var positions = FindFarthestPosition(cell, vector);
                        var next = GetTile(positions.Item2);

                        if (next != null && next.Value == tile.Value && next.MergedFrom == null) {
                            var merged = new Tile(positions.Item2, tile.Value*2) {
                                MergedFrom = new Tuple<Tile, Tile>(tile, next)
                            };

                            InsertTile(merged);
                            RemoveTile(tile);

                            tile.Position = positions.Item2;

                            Score += merged.Value;

                            if (merged.Value == 2048) {
                                Won = true;
                            }

                        }else {
                            MoveTile(tile, positions.Item1);
                        }

                        if (cell != tile.Position) {
                            moved = true;
                        }
                    } 
                }
            }
            if (moved) {
                AddRandomTile();

                if (!MovesAvailable()) {
                    Over = true;
                }
            }

        }

        private bool MovesAvailable() { return CellsAvailable() || TileMatchesAvailable(); }

        private bool TileMatchesAvailable() {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    var tile = GetTile(x, y);
                    if (tile.Value > 0) {
                        for (var dir = Direction.Left; dir <= Direction.Down; dir++) {
                            var vector = GetVector(dir);
                            var cell = new Point(x + vector.X, y + vector.Y);
                            var other = GetTile(cell);
                            if (other != null && other.Value == tile.Value) {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool IsGameTerminated() { return (Over || (Won && !KeepPlaying)); }

        private void AddRandomTile(bool newTile=true) {
            if (CellsAvailable()) {
                var value = _rand.NextDouble() < 0.9 ? 2 : 4;
                var tile = new Tile(RandomAvailableCell(), value);
                tile.NewTile = newTile;
                InsertTile(tile);
            }
        }

        private Point RandomAvailableCell() {
            var available = AvailableCells();
            return available[_rand.Next(available.Count)];
        }

        private bool CellsAvailable() { return AvailableCells().Any(); }

        private List<Point> AvailableCells() {
            var ret = new List<Point>();
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    var tile = GetTile(x, y);
                    if (tile.Value == 0) {
                        ret.Add(new Point(x,y));
                    }
                }
            }
            return ret;
        }

        private void MoveTile(Tile tile, Point cell) {
            _tiles[tile.Position.X][tile.Position.Y] = new Tile(tile.Position, 0);
            _tiles[cell.X][cell.Y] = tile;
            tile.Position = cell;
        }

        private void RemoveTile(Tile tile) {
            _tiles[tile.Position.X][tile.Position.Y] = new Tile(tile.Position, 0);
        }

        private void InsertTile(Tile tile) {
            _tiles[tile.Position.X][tile.Position.Y] = tile;
        }

        private Tuple<Point, Point> FindFarthestPosition(Point cell, Point vector) {
            Point previous;
            do {
                previous = cell;
                cell = new Point(previous.X + vector.X, previous.Y + vector.Y);
            } while (WithinBounds(cell) && CellAvailable(cell));
            return new Tuple<Point, Point>(previous, cell);
        }

        private bool CellAvailable(Point position) {
            var tile = GetTile(position);
            return tile != null && tile.Value == 0;
        }

        private bool WithinBounds(Point position) { return position.X >= 0 && position.Y >= 0 && position.X < Size && position.Y < Size; }

        private void PrepareTiles() {
            for (int x = 0; x < Size; x++) {
                for (int y = 0; y < Size; y++) {
                    var tile = _tiles[x][y];
                    tile.MergedFrom = null;
                    tile.SavePosition();
                    tile.NewTile = false;
                }
            }
        }

        private Tuple<List<int>, List<int>> BuildTraversals(Point vector) {
            var ret = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());

            for (int pos = 0; pos < Size; pos++) {
                ret.Item1.Add(pos);
                ret.Item2.Add(pos);
            }

            if (vector.X == 1) ret.Item1.Reverse();
            if (vector.Y == 1) ret.Item2.Reverse();


            return ret;
        }

        private static Point GetVector(Direction dir) {
            switch (dir) {
                case Direction.Left:
                    return new Point(-1, 0);
                case Direction.Right:
                    return new Point(1, 0);
                case Direction.Up:
                    return new Point(0, -1);
                case Direction.Down:
                    return new Point(0, 1);
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
        }
        private Tile GetTile(int x, int y) { return GetTile(new Point(x, y)); }
        private Tile GetTile(Point cell) {
            if (WithinBounds(cell)) {
                return _tiles[cell.X][cell.Y];
            }
            return null;
        }
    }
}