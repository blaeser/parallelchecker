using System.Threading.Tasks;
using System;

namespace ParallelChecker._Test {
  class Matrix {
    private int[,] _array;

    public Matrix(int rows, int cols) {
      _array = new int[rows, cols];
    }

    public int this[int row, int col] {
      get {
        return _array[row, col];
      }
      set {
        _array[row, col] = value;
      }
    }

    public static void Main() {
      var matrix = new Matrix(1, 2);
      Task.Run(() =>
      {
        matrix[0, 0] = 1;
        matrix[0, 1] = 2;
      });
      Console.Write(matrix[0, 1]);
    }
  }
}
