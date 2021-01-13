// using CommandLine;
//
// namespace Ploch.Tools.SystemUtils.UI.ConsoleApp.Commands.Spm
// {
//     public enum SpmSource
//     {
//         Jenkins,
//         LocalBuild
//     }
//
//     [Verb("reinstall-spm")]
//     public class ReinstallSpmArgs
//     {
//         [Option(Default = Spm.SpmSource.LocalBuild)]
//         public SpmSource SpmSource { get; set; }
//
//         [Option('s', "source")]
//         public string SourceLocation { get; set; }
//
//         [Option('t', "target")]
//         public string TargetPath { get; set; }
//
//         [Option('n', "servicename")]
//         public string ServiceNamePattern { get; set; }
//         
//         
//
//     }
// }