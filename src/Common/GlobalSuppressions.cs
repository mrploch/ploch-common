using System;
using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
                    "SA1636:File header copyright text should match",
                    Justification = "We don't use the header",
                    Scope = "module")]

[assembly:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
                    "SA1641:File header company name text should match",
                    Justification = "We don't use the header",
                    Scope = "module")]

[assembly:
    SuppressMessage("StyleCop.CSharp.DocumentationRules",
                    "SA1633:The file header is missing or not located at the top of the file",
                    Justification = "We don't use the header",
                    Scope = "module")]

[assembly:
    SuppressMessage("Security",
                    "CA5394:Do not use insecure randomness",
                    Justification = "The randomizers generate non-security-sensitive sample/test data. Cryptographically secure randomness is unnecessary here and would degrade performance.",
                    Scope = "namespaceanddescendants",
                    Target = "~N:Ploch.Common.Randomizers")]

[assembly: CLSCompliant(true)]
