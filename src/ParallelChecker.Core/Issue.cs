using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core {
  public sealed class Issue {
    public IssueCategory Category { get; }
    public string Message { get; }
    public IList<Cause> Causes { get; }

    public Issue(IssueCategory category, string message, IEnumerable<Cause> causes) {
      if (causes == null) {
        throw new ArgumentNullException(nameof(causes));
      }
      Category = category;
      Message = message ?? throw new ArgumentNullException(nameof(message));
      Causes = new List<Cause>(causes);
    }

    public override bool Equals(object obj) {
      if (obj is not Issue) {
        return false;
      }
      var other = (Issue)obj;
      return Equivalent(other);
    }

    public bool Equivalent(Issue other) {
      return 
        Category == other.Category &&
        Message.Equals(other.Message) &&
        new HashSet<Cause>(Causes).SetEquals(other.Causes);
    }

    public override int GetHashCode() {
      return
        Category.GetHashCode() * 31 +
        Message.GetHashCode();
    }

    private const int _Indentation = 2;
    private const int _Limit = 32;
    private const int _Tolerance = 16;

    public string Description {
      get {
        var message = Message;
        foreach (var cause in Causes) {
          message += GetCauseDescription(cause, _Indentation, _Limit, _Tolerance);
        }
        return message;
      }
    }

    private string GetCauseDescription(Cause cause, int indent, int limit, int tolerance) {
      string description = Environment.NewLine;
      for (int index = 0; index < indent; index++) {
        description += " ";
      }
      description += $"caused by {cause.Description}";
      var location = cause.Location;
      if (location != Location.None) {
        description += $" at \"{GetSourceExcerpt(location, limit, tolerance)}\" {GetSourcePosition(location)}";
      }
      if (cause.Origin != null) {
        description += GetCauseDescription(cause.Origin, indent + 2, limit, tolerance);
      }
      return description;
    }

    private string GetSourcePosition(Location location) {
      var lineSpan = location.GetLineSpan();
      if (!lineSpan.IsValid) {
        return string.Empty;
      }
      string path = ExtractFileName(lineSpan.Path);
      return $"in {path} line {lineSpan.StartLinePosition.Line + 1}";
    }

    private string ExtractFileName(string path) {
      var index = path.LastIndexOf('\\');
      if (index >= 0) {
        path = path.Substring(index + 1);
      }
      return path;
    }

    private string GetSourceExcerpt(Location location, int limit, int tolerance) {
      if (location.SourceTree == null) {
        return string.Empty;
      }
      var excerpt = location.SourceTree.ToString().Substring(location.SourceSpan.Start, location.SourceSpan.Length);
      return ClipExcerpt(excerpt, limit, tolerance);
    }

    private string ClipExcerpt(string input, int limit, int tolerance) {
      var output = "";
      int index = 0;
      while (index < input.Length && output.Length < limit) {
        var character = input[index];
        if (IsWhiteSpace(character)) {
          if (output.Length == 0 || !IsWhiteSpace(output[output.Length - 1])) {
            output += " ";
          }
        } else {
          output += character;
        }
        index++;
      }
      if (output.Length == limit) {
        if (output.Length > 0 && !IsWhiteSpace(output[output.Length - 1])) {
          while (index < input.Length && output.Length < limit + tolerance && IsLetterOrDigit(input[index])) {
            output += input[index];
            index++;
          }
        }
      }
      if (index < input.Length) {
        output += "...";
      }
      return output;
    }

    private bool IsWhiteSpace(char character) {
      return character <= ' '; 
    }

    private bool IsLetterOrDigit(char character) {
      return IsLetter(character) || IsDigit(character);
    }

    private bool IsLetter(char character) {
      return character >= 'A' && character <= 'Z' ||
        character >= 'a' && character <= 'z';
    }

    private bool IsDigit(char character) {
      return character >= '0' && character <= 'z';
    }
  }
}
