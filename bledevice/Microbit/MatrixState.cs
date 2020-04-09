using System;
using System.Collections.Generic;
using System.Linq;

namespace bledevice.Microbit
{
    public class MatrixState
    {
        public static readonly MatrixState EMPTY = new MatrixState(
            "     ",
            "     ",
            "     ",
            "     ",
            "     "
        );

        public static readonly MatrixState HEART_FULL = new MatrixState(
            " * * ",
            "*****",
            "*****",
            " *** ",
            "  *  "
        );

        public static readonly MatrixState HEART_EMPTY = new MatrixState(
            " * * ",
            "* * *",
            "*   *",
            " * * ",
            "  *  "
        );

        public static readonly MatrixState CIRCLE_FULL = new MatrixState(
            " *** ",
            "*****",
            "*****",
            "*****",
            " *** "
        );

        public static readonly MatrixState CIRCLE_EMPTY = new MatrixState(
            " *** ",
            "*   *",
            "*   *",
            "*   *",
            " *** "
        );

        public static readonly MatrixState SQUARE_FULL = new MatrixState(
            "*****",
            "*****",
            "*****",
            "*****",
            "*****"
        );

        public static readonly MatrixState SQUARE_EMPTY = new MatrixState(
            "*****",
            "*   *",
            "*   *",
            "*   *",
            "*****"
        );

        public static readonly MatrixState DIAMOND_FULL = new MatrixState(
            "  *  ",
            " *** ",
            "*****",
            " *** ",
            "  *  "
        );

        public static readonly MatrixState DIAMOND_EMPTY = new MatrixState(
            "  *  ",
            " * * ",
            "*   *",
            " * * ",
            "  *  "
        );

        public static readonly MatrixState CHECK_MARK = new MatrixState(
            "    *",
            "   * ",
            "* *  ",
            " *   ",
            "     "
        );

        public static readonly MatrixState X_MARK = new MatrixState(
            "*   *",
            " * * ",
            "  *  ",
            " * * ",
            "*   *"
        );

        public static readonly MatrixState ARROW_UP = new MatrixState(
            "  *  ",
            " *** ",
            "* * *",
            "  *  ",
            "  *  "
        );

        public static readonly MatrixState ARROW_DOWN = new MatrixState(
            "  *  ",
            "  *  ",
            "* * *",
            " *** ",
            "  *  "
        );

        public static readonly MatrixState ARROW_LEFT = new MatrixState(
            "  *  ",
            " *   ",
            "*****",
            " *   ",
            "  *  "
        );

        public static readonly MatrixState ARROW_RIGHT = new MatrixState(
            "  *  ",
            "   * ",
            "* ***",
            "   * ",
            "  *  "
        );

        public static readonly MatrixState ARROW_UPLEFT = new MatrixState(
            "**** ",
            "**   ",
            "* *  ",
            "*  * ",
            "    *"
        );

        public static readonly MatrixState ARROW_UPRIGHT = new MatrixState(
            " ****",
            "   **",
            "  * *",
            " *  *",
            "*    "
        );

        public static readonly MatrixState ARROW_DOWNLEFT = new MatrixState(
            "    *",
            "*  * ",
            "* *  ",
            "**   ",
            "**** "
        );

        public static readonly MatrixState ARROW_DOWNRIGHT = new MatrixState(
            "*    ",
            " *  *",
            "  * *",
            "   **",
            " ****"
        );

        internal readonly byte[] State = new byte[5];

        public MatrixState(IEnumerable<byte> state)
        {
            int i = 0;
            foreach (var b in state)
            {
                State[i] = (byte) (b & 0x1F);
                i++;
                if (i >= 4) break;
            }
        }

        public MatrixState(params byte[][] state)
        {
            var rowNo = 0;
            foreach (var row in state)
            {
                State[rowNo] = RowToByte(row);
                rowNo++;
            }
        }

        public MatrixState(params string[] state)
        {
            var rowNo = 0;
            foreach (var row in state)
            {
                State[rowNo] = RowToByte(row);
                rowNo++;
            }
        }

        public MatrixState Invert()
        {
            return new MatrixState(from b in State select (byte) ~b);
        }

        private static byte RowToByte(IEnumerable<byte> row)
        {
            var pos = 4;
            byte ret = 0;
            foreach (var cell in row)
            {
                var bit = (byte) ((cell != 0 ? 1 : 0) << pos);
                ret |= bit;
                pos--;
            }

            return ret;
        }

        private static byte RowToByte(string row)
        {
            var pos = 4;
            byte ret = 0;
            foreach (var cell in row)
            {
                var bit = (byte) ((cell != ' ' && cell != '0' ? 1 : 0) << pos);
                ret |= bit;
                pos--;
            }

            return ret;
        }
    }
}