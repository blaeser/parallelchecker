using Microsoft.CodeAnalysis;
using ParallelChecker.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ParallelChecker._Test {
  internal class IssueReport {
    public static void Export(IEnumerable<Issue> issues, string filePath) {
      var sorted = from issue in issues orderby IssueName(issue) select issue;
      var document = CreateXml(sorted);
      WriteXml(filePath, document);
    }
    
    private static void WriteXml(string filePath, XmlDocument document) {
      using var writer = new XmlTextWriter(filePath, new UTF8Encoding());
      document.WriteTo(writer);
    }

    private static XmlDocument CreateXml(IEnumerable<Issue> issues) {
      var document = new XmlDocument();
      var rootElement = document.CreateElement("ParallelIssues");
      document.AppendChild(rootElement);
      ExportIssues(issues, rootElement);
      return document;
    }

    private static void ExportIssues(IEnumerable<Issue> issues, XmlElement parentElement) {
      var document = parentElement.OwnerDocument;
      foreach (var issue in issues) {
        var issueElement = document.CreateElement("Issue");
        issueElement.SetAttribute("Message", issue.Message);
        parentElement.AppendChild(issueElement);
        var locations = from cause in issue.Causes select cause.Location;
        foreach (var location in SortedLocations(locations)) {
          var locationElement = document.CreateElement("Location");
          locationElement.SetAttribute("Value", location.ToString());
          issueElement.AppendChild(locationElement);
        }
      }
    }

    private static string IssueName(Issue issue) {
      var name = issue.Message + " ";
      var locations = from cause in issue.Causes select cause.Location;
      foreach (var location in SortedLocations(locations)) {
        name += location;
      }
      return name;
    }

    private static IEnumerable<Location> SortedLocations(IEnumerable<Location> locations) {
      return from location in locations orderby location.ToString() select location;
    }
  }
}
