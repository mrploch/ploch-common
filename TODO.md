# Current TODO List

## Rules

- Apply all of the rules and skills from entire hierarchy of rules - i.e. user, project (`c:/devnet/my/mrploch`), repository (`c:/devnet/my/mrploch/ploch-common`)

## Warnings in dotnet restore

Verify the warnings from `dotnet restore Ploch.Common.slnx` and fix them. 
When validating, you can run the script to fully cleanup the solution, `scripts/Clean-Repository.ps1`.
When validated locally, commit them, create a PR and verify that all of the PR checks have passed correctly.
If there are any problems there, resolve them (commit and push the fixes).
Then review all of the comments under this new PR and resolve them if relevant, close not relevant.
The issue for this task is [#162](https://github.com/mrploch/ploch-common/issues/162), there is a correct branch set up.

## Code Analysis Warnings

The build of `Ploch.Common.slnx` has a number of static code analyzers warnings and suggestions. 
Take all of those that are quite easy to resolve and would not break anything, then fix them.
Commit under the same PR as the above task (Warnings in dotnet restore), the same branch.
Always validate, that the PR builds correctly, that the checks are passing without errors. If there are any, resolve them.
