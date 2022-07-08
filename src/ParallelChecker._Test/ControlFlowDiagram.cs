using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ParallelChecker._Test {
  internal static class ControlFlowDiagram {
    private const string _NameSpace = "http://schemas.microsoft.com/vs/2009/dgml";

    private static readonly Dictionary<Type, string> _colorMap = new() {
      { typeof(BranchBlock), "AliceBlue" },
      { typeof(ConstantBlock), "AntiqueWhite" },
      { typeof(ObjectCreationBlock), "Aqua" },
      { typeof(EmptyBlock), "Aquamarine" },
      { typeof(EntryBlock), "Azure" },
      { typeof(ExitBlock), "Beige" },
      { typeof(InvocationBlock), "Bisque" },
      { typeof(LockBlock), "BlanchedAlmond" },
      { typeof(OperatorBlock), "Blue" },
      { typeof(ReadBlock), "BlueViolet" },
      { typeof(UnlockBlock), "BurlyWood" },
      { typeof(WriteBlock), "CadetBlue" },
      { typeof(ThisBlock), "Chartreuse" },
      { typeof(ArrayCreationBlock), "Chocolate" },
      { typeof(ElementSelectionBlock), "Coral" },
      { typeof(AwaitBlock), "CornflowerBlue" },
      { typeof(ArrayInitializerBlock), "Cornsilk" },
      { typeof(DiscardBlock), "Crimson" },
      { typeof(DuplicateBlock), "Cyan" },
      { typeof(CastBlock), "DarkBlue" },
      { typeof(ObjectCloneBlock), "DarkCyan" },
      { typeof(VariableSelectionBlock), "DarkGoldenrod" },
      { typeof(PropertyGetBlock), "DarkGray" },
      { typeof(PropertySetBlock), "DarkGreen" },
      { typeof(IteratorStartBlock), "DarkKhaki" },
      { typeof(IteratorNextBlock), "DarkMagenta" },
      { typeof(IteratorEndBlock), "DarkOliveGreen" },
      { typeof(ThrowBlock), "DarkOrange" },
      { typeof(EnterTryBlock), "DarkOrchid" },
      { typeof(ExitTryBlock), "DarkRed" },
      { typeof(CatchBlock), "DarkSalmon" },
      { typeof(UnknownBlock), "DarkSeaGreen" },
      { typeof(SwapBlock), "DarkSlateBlue" },
      { typeof(CollectionInitializerBlock), "DarkSlateGray" },
      { typeof(UndeclareBlock), "DarkTurquoise" },
      { typeof(TupleCreationBlock), "DarkViolet" },
      { typeof(AliasBlock), "DarkPink" },
      { typeof(LinqBlock), "DeepSkyBlue" }
    };
    private static readonly List<Type> _categories = _colorMap.Keys.ToList();

    // TODO: Optimize indexOf() away but without complicating design
    public static void Export(ControlFlowModel model, string filePath) {
      var document = BuildDiagram(model);
      using var writer = new XmlTextWriter(filePath, new UTF8Encoding());
      document.WriteTo(writer);
    }

    private static XmlDocument BuildDiagram(ControlFlowModel model) {
      var allBlocks = AllBlocks(model).ToList();
      var document = new XmlDocument();
      document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", null));
      var root = document.CreateElement("DirectedGraph", _NameSpace);
      var categories = CreateCategories(document);
      root.AppendChild(categories);
      var nodes = CreateNodes(document, allBlocks);
      root.AppendChild(nodes);
      var links = CreateLinks(document, allBlocks);
      root.AppendChild(links);
      var properties = CreateProperties(document);
      root.AppendChild(properties);
      var styles = CreateStyles(document); root.AppendChild(styles);
      document.AppendChild(root);
      return document;
    }

    private static XmlElement 
      CreateCategories(XmlDocument document) {
      var categories = document.CreateElement("Categories", _NameSpace);
      for (int index = 0; index < _categories.Count; index++) {
        var category = document.CreateElement("Category", _NameSpace);
        category.SetAttribute("Id", index.ToString());
        category.SetAttribute("Label", _categories[index].Name);
        categories.AppendChild(category);
      }
      return categories;
    }

    private static XmlElement CreateNodes(XmlDocument document, List<BasicBlock> allBlocks) {
      var nodes = document.CreateElement("Nodes", _NameSpace);
      foreach (var block in allBlocks) {
        var index = allBlocks.IndexOf(block);
        // TODO: improve label
        var blockType = block.GetType();
        var categoryId = _categories.IndexOf(blockType);
        var label = string.Format("{0} {1}", GetBlockName(block), block.Location);
        var node = document.CreateElement("Node", _NameSpace);
        node.SetAttribute("Id", index.ToString());
        node.SetAttribute("Label", label);
        node.SetAttribute("Category", categoryId.ToString());
        nodes.AppendChild(node);
      }
      return nodes;
    }

    private static XmlElement CreateLinks(XmlDocument document, List<BasicBlock> allBlocks) {
      var links = document.CreateElement("Links", _NameSpace);
      foreach (var source in allBlocks) {
        foreach (var target in Successors(source)) {
          var sourceIndex = allBlocks.IndexOf(source);
          var targetIndex = allBlocks.IndexOf(target);
          var link = document.CreateElement("Link", _NameSpace);
          link.SetAttribute("Source", sourceIndex.ToString());
          link.SetAttribute("Target", targetIndex.ToString());
          links.AppendChild(link);
        }
      }
      return links;
    }

    private static XmlElement CreateProperties(XmlDocument document) {
      var properties = document.CreateElement("Properties", _NameSpace);
      var property = document.CreateElement("Property", _NameSpace);
      property.SetAttribute("Id", "Extra");
      property.SetAttribute("Label", "Extra");
      property.SetAttribute("DataType", "System.String");
      properties.AppendChild(property);
      return properties;
    }

    private static XmlElement CreateStyles(XmlDocument document) {
      var styles = document.CreateElement("Styles", _NameSpace);
      foreach (var category in _categories) {
        var index = _categories.IndexOf(category);
        var color = _colorMap[category];
        var style = document.CreateElement("Style", _NameSpace);
        style.SetAttribute("TargetType", "Node");
        style.SetAttribute("GroupLabel", category.Name);
        style.SetAttribute("ValueLabel", "Has category");
        var condition = document.CreateElement("Condition", _NameSpace);
        condition.SetAttribute("Expression", string.Format("HasCategory('{0}')", index));
        style.AppendChild(condition);
        style.AppendChild(CreateSetter(document, "Background", color));
        style.AppendChild(CreateSetter(document, "NodeRadius", "3"));
        styles.AppendChild(style);
      }
      return styles;
    }

    private static XmlElement CreateSetter(XmlDocument xmlDoc, string property, string value) {
      var condition = xmlDoc.CreateElement("Setter", _NameSpace);
      condition.SetAttribute("Property", property);
      condition.SetAttribute("Value", value);
      return condition;
    }

    private static IEnumerable<BasicBlock> Successors(BasicBlock block) {
      var result = new List<BasicBlock>();
      if (block is EnterTryBlock tryBlock) {
        result.Add(tryBlock.Successor);
        result.Add(tryBlock.Catches);
        result.Add(tryBlock.Finally);
      } else if (block is StraightBlock straight) {
        result.Add(straight.Successor);
      } else if (block is BranchBlock branch) {
        result.Add(branch.SuccessorOnTrue);
        result.Add(branch.SuccessorOnFalse);
      } else if (block is ExitBlock) {
      } else if (block is IteratorNextBlock iteratorBranch) {
        result.Add(iteratorBranch.SuccessorOnContinue);
        result.Add(iteratorBranch.SuccessorOnFinished);
      } else if (block is ThrowBlock) {
      } else {
        throw new NotImplementedException();
      }
      return result;
    }

    private static IEnumerable<BasicBlock> AllBlocks(ControlFlowModel model) {
      return
        from routine in AllRoutines(model.CompilationModel.Compilation)
        let graph = model.GetGraph(routine)
        from block in AllBlocks(graph)
        select block;
    }

    private static IEnumerable<BasicBlock> AllBlocks(ControlFlowGraph graph) {
      var visited = new HashSet<BasicBlock>();
      var open = new Queue<BasicBlock>();
      open.Enqueue(graph.Entry);
      while (open.Count > 0) {
        var block = open.Dequeue();
        if (visited.Add(block)) {
          foreach (var successor in Successors(block)) {
            open.Enqueue(successor);
          }
        }
      }
      return visited;
    }

    private static IEnumerable<Routine> AllRoutines(Compilation compilation) {
      return
        (from method in AllClassMethods(compilation)
         select new MethodRoutine(method)).Cast<Routine>().Union
        (from method in AllLocalFunctions(compilation)
         select new MethodRoutine(method)).Union
        (from accessor in AllAccessors(compilation)
         select new PropertyRoutine(accessor)).Union
        (from lambda in AllLambdas(compilation)
         select new LambdaRoutine(lambda)).Union
        (from type in AllTypes(compilation)
         select new InitializerRoutine(type, false)).Union
        (from expression in AllLambdaProperties(compilation)
         select new ExpressionRoutine(expression, false)).Union
        (from expression in AllLambdaGetAccessors(compilation)
         select new ExpressionRoutine(expression, false)).Union
        (from expression in AllLambdaSetAccessors(compilation)
          select new ExpressionRoutine(expression, true)).Union
        (from type in AllTypes(compilation)
         select new StaticRoutine(type));
    }

    private static IEnumerable<ExpressionSyntax> AllLambdaGetAccessors(Compilation compilation) {
      return
        from property in AllProperties(compilation)
        let accessor = property.FindGetAccessor()
        where accessor != null && accessor.ExpressionBody != null
        select accessor.ExpressionBody.Expression;
    }

    private static IEnumerable<ExpressionSyntax> AllLambdaSetAccessors(Compilation compilation) {
      return
        from property in AllProperties(compilation)
        let accessor = property.FindInitOrSetAccesor()
        where accessor != null && accessor.ExpressionBody != null
        select accessor.ExpressionBody.Expression;
    }

    private static IEnumerable<ExpressionSyntax> AllLambdaProperties(Compilation compilation) {
      return
        from property in AllProperties(compilation)
        where property is PropertyDeclarationSyntax && property.AccessorList == null
        select ((PropertyDeclarationSyntax)property).ExpressionBody.Expression;
    }

    private static IEnumerable<BasePropertyDeclarationSyntax> AllProperties(Compilation compilation) {
      return
        from type in AllTypes(compilation)
        from member in type.Members
        where member is BasePropertyDeclarationSyntax
        let property = (BasePropertyDeclarationSyntax)member
        select property;
    }

    private static IEnumerable<BaseMethodDeclarationSyntax> AllClassMethods(Compilation compilation) {
      return
        from type in AllTypes(compilation)
        where type is not InterfaceDeclarationSyntax
        from member in type.Members
        where member is BaseMethodDeclarationSyntax &&
          !IsStaticConstructor(member) &&
          !member.GetModifiers().IsAbstract() &&
          !member.GetModifiers().IsExtern()
        select (BaseMethodDeclarationSyntax)member;
    }

    private static IEnumerable<AccessorDeclarationSyntax> AllAccessors(Compilation compilation) {
      return
        from type in AllTypes(compilation)
        from member in type.Members
        where member is BasePropertyDeclarationSyntax
        let property = (BasePropertyDeclarationSyntax)member
        where property.AccessorList != null
        from accessor in property.AccessorList.Accessors
        where accessor.Body != null
        select accessor;
    }

    private static IEnumerable<AnonymousFunctionExpressionSyntax> AllLambdas(Compilation compilation) {
      return
        from method in AllClassMethods(compilation)
        from child in method.DescendantNodes()
        where child is AnonymousFunctionExpressionSyntax
        select (AnonymousFunctionExpressionSyntax)child;
    }

    private static IEnumerable<LocalFunctionStatementSyntax> AllLocalFunctions(Compilation compilation) {
      return
        from method in AllClassMethods(compilation)
        from child in method.DescendantNodes()
        where child is LocalFunctionStatementSyntax
        select (LocalFunctionStatementSyntax)child;
    }

    private static IEnumerable<TypeDeclarationSyntax> AllTypes(Compilation compilation) {
      var allRoots =
        from tree in compilation.SyntaxTrees
        select tree.GetRoot();
      return
        from root in allRoots
        from child in root.DescendantNodes(
          n => n is CompilationUnitSyntax ||
                n is NamespaceDeclarationSyntax ||
                n is TypeDeclarationSyntax)
        where child is TypeDeclarationSyntax
        select (TypeDeclarationSyntax)child;
    }

    private static bool IsStaticConstructor(SyntaxNode node) {
      if (node is ConstructorDeclarationSyntax constructor) {
        return constructor.Modifiers.IsStatic();
      }
      return false;
    }

    private static string GetBlockName(BasicBlock block) {
      var type = block.GetType();
      var name = type.Name + " ";
      foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
        if (!typeof(BasicBlock).IsAssignableFrom(field.FieldType)) {
          name += field.GetValue(block) + " ";
        }
      }
      return CorrectName(name);
    }

    private static string CorrectName(string value) {
      var result = string.Empty;
      foreach (var character in value) {
        if (character >= '\n') {
          result += character;
        }
      }
      return result;
    }
  }
}
