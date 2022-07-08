using System;

namespace ParallelChecker._Test {
  class Matrix {
    private int[,] _array;

    public Matrix(int rows, int cols) {
      _array = new int[rows, cols];
    }

    public static explicit operator Matrix(int[] array) {
      var matrix = new Matrix(1, array.Length);
      for (int index = 0; index < array.Length; index++) {
        matrix[0, index] = array[index];
      }
      return matrix;
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
      Matrix matrix1 = (Matrix)new int[] { 1, 2, 3 };
      Console.Write(matrix1[0, 0]);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}
