using System.Collections.Generic;
using System.Drawing;

namespace Tetris.GUI.Tetromino
{
    public class TetraminoProjector
    {
        private readonly ISideProjector _leftSideProjector = new LeftSideProjector();
        private readonly ISideProjector _rightSideProjector = new RightSideProjector();
        private readonly ISideProjector _topSideProjector = new TopSideProjector();
        private readonly ISideProjector _bottomSideProjector = new BottomSideProjector();
        private readonly List<ISideProjector> _sideProjectors = new List<ISideProjector>();

        public TetraminoProjector()
        {
            _sideProjectors.Add(_leftSideProjector);
            _sideProjectors.Add(_rightSideProjector);
            _sideProjectors.Add(_topSideProjector);
            _sideProjectors.Add(_bottomSideProjector);
        }

        public (int left, int top, int right, int bottom) GetBounds(bool[,] tetromino)
        {
            var size = tetromino.GetUpperBound(0);

            for (int i = 0; i <= size; i++)
            {
                for (int j = 0; j <= size; j++)
                {
                    bool value = tetromino[i, j];
                    if (!value) { continue; }
                    _sideProjectors.ForEach(projector => projector.Project(i, j));
                }
            }

            return (_leftSideProjector.Value,
                _topSideProjector.Value,
                _rightSideProjector.Value,
                _bottomSideProjector.Value);
        }
    }

    public interface ISideProjector
    {
        int Value { get; }
        void Project(int x, int y);
    }

    public class LeftSideProjector : ISideProjector
    {
        private int _x = 50;

        public int Value => _x;

        public void Project(int x, int y)
        {
            if (x < _x)
            {
                _x = x;
            }
        }
    }

    public class RightSideProjector : ISideProjector
    {
        private int _x = -1;

        public int Value => _x;

        public void Project(int x, int y)
        {
            if (x > _x)
            {
                _x = x;
            }
        }
    }

    public class TopSideProjector : ISideProjector
    {
        private int _y = 50;

        public int Value => _y;

        public void Project(int x, int y)
        {
            if (y < _y)
            {
                _y = y;
            }
        }
    }

    public class BottomSideProjector : ISideProjector
    {
        private int _y = -1;

        public int Value => _y;

        public void Project(int x, int y)
        {
            if (y > _y)
            {
                _y = y;
            }
        }
    }
}