# Performing Tasks from TODO.md

- When performing a task from the `TODO.md`, always validate it is resolved / fixed - for example re-run the problematic code / command and validate output.
- If a task is a coding task, always add a comprehensive set of unit tests according to the configured .NET testing rules.
- Where possible, run the tasks in parallel, maybe using sub-agents
- Auto-accept the tools usage
- Create a commit per task
- When creating a commit message, use the Conventional Commit, as described in rules
- Allways analyze and review the code for potential issues, including best practices, conventions and other problems
- Always build and try to resolve all new warnings (and obviously the errors)
- Use the output from static code analyzers configured for this project
- You can use the JetBrains Rider MCP server which should be available
