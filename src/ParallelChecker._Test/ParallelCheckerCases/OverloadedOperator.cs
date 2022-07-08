using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Matrix {
    private int[,] _array;

    public int Rows => _array.GetLength(0);
    public int Cols => _array.GetLength(1);

    public Matrix(int rows, int cols) {
      _array = new int[rows, cols];
    }

    public static Matrix operator +(Matrix first, Matrix second) {
      var result = new Matrix(first.Rows, first.Cols);
      for (int row = 0; row < first.Rows; row++) {
        for (int col = 0; col < first.Cols; col++) {
          result[row, col] = first[row, col] + second[row, col];
        }
      }
      return result;
    }

    public static Matrix operator -(Matrix first, Matrix second) {
      for (int row = 0; row < first.Rows; row++) {
        for (int col = 0; col < first.Cols; col++) {
          first[row, col] -= second[row, col];
        }
      }
      return first;
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
      var matrix1 = new Matrix(1, 2);
      var matrix2 = new Matrix(1, 2);
      Task.Run(() => { var matrix3 = matrix1 + matrix2; });
      var matrix4 = matrix1 - matrix2;
    }
  }
}
