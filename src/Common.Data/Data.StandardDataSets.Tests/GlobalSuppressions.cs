using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional", Justification = "Not important in the tests project."),
         SuppressMessage("ReSharper", "ExceptionNotDocumented", Justification = "Not important in the tests project."),
         SuppressMessage("ReSharper",
                         "PossibleMultipleEnumeration",
                         Justification = "Not important in the tests project and could reduce readability.",
                         Scope = "namespaceanddescendants"),
         SuppressMessage("ReSharper",
                         "LambdaExpressionCanBeMadeStatic",
                         Justification = "Optimizations like that are not important in the tests project.",
                         Scope = "namespaceanddescendants"),
         SuppressMessage("ReSharper",
                         "LambdaExpressionCanBeMadeStatic",
                         Justification = "Optimizations like that are not important in the tests project.",
                         Scope = "namespaceanddescendants")]