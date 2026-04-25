# Copilot Cloud Agent PR Pipeline

This repository now contains a staged custom-agent setup for deep pull request work:

- `.github/agents/repo-investigator.agent.md`
- `.github/agents/pr-review-planner.agent.md`
- `.github/agents/plan-critic.agent.md`
- `.github/agents/pr-remediation.agent.md`
- `.github/agents/pr-pipeline-orchestrator.agent.md`

## What this setup gives you

### Stage 1: repository investigation

`repo-investigator` gathers project-specific understanding before detailed review or implementation.

### Stage 2: PR review and planning

`pr-review-planner` opens the PR, inspects the diff, reads linked tickets and related PRs or issues, checks all review
comments and conversations, checks CI, and produces a remediation plan.

For non-trivial plans it is instructed to invoke `plan-critic`, which is configured to use `claude-opus-4.6`.

### Stage 3: remediation

`pr-remediation` implements the plan, validates changes, re-checks comments and CI, and loops back to planning if the
situation changes.

### Optional: one-entry orchestration

`pr-pipeline-orchestrator` is the user-facing coordinator. It sequences the specialist agents explicitly.

## Important platform limits

### GitHub.com cloud agent does not support YAML `handoffs`

GitHub documents that the `argument-hint` and `handoffs` properties are ignored for Copilot cloud agent on GitHub.com.
This means a true native handoff graph is not available there.

Because of that, this setup uses prompt-level orchestration and the `agent` tool alias instead of YAML handoffs.

### If you need hard guarantees, use external orchestration

If you need a deterministic pipeline with auditable stage boundaries, create separate agent tasks through the GitHub
Agent Tasks REST API and launch them in sequence:

1. `repo-investigator`
2. `pr-review-planner`
3. `pr-remediation`

That approach is more reliable than depending only on prompt-driven delegation inside one task.

## Cross-model review

The hidden `plan-critic` agent is configured with:

- `model: claude-opus-4.6`

The other stage agents are configured with:

- `model: gpt-5.3-codex`

This gives you the pattern you asked for: the main working agents can use Codex while non-trivial plans are reviewed by
Claude Opus.

## Required repository configuration

### 1. Keep the custom agent files in the default branch

GitHub reads custom agents from `.github/agents/*.agent.md`.

### 2. Configure writable GitHub MCP access if you want automated PR replies

By default, the built-in GitHub MCP server is read-only and scoped to the current repository. That is not enough if you
want the agent to reply to false-positive PR comments or conversations.

If you want automated comment replies and broader GitHub research, do the following in the repository settings:

1. Go to `Settings -> Copilot -> Cloud agent`.
2. Add MCP configuration using the example
   from [docs/copilot-cloud-agent-mcp.example.json](./copilot-cloud-agent-mcp.example.json).
3. Go to `Settings -> Environments`.
4. Create an environment named `copilot`.
5. Add an environment secret named `COPILOT_MCP_GITHUB_PERSONAL_ACCESS_TOKEN`.

Use a fine-grained PAT with the narrowest permissions that still allow:

- reading repository contents
- reading and writing pull request comments or review-thread replies
- reading and writing issue comments when needed
- reading Actions and check-run state

If you only need read-only research, use the GitHub read-only MCP configuration instead.

### 3. Add external ticketing MCP servers if your tickets live outside GitHub

If the associated ticket can live in Jira, Azure Boards, Linear, or another system, add the corresponding MCP server to
the repository Copilot configuration or the agent profile. Without that, the PR planner can only fully research
GitHub-native issues and pull requests.

### 4. Only add `copilot-setup-steps.yml` when your MCP servers need extra dependencies

You do not need a setup workflow for the GitHub MCP server alone. You only need
`.github/workflows/copilot-setup-steps.yml` if another MCP server requires packages or login steps that are not present
on the default runner.

## Suggested usage

### Manual staged usage

Use these agents in order:

1. `repo-investigator`
2. `pr-review-planner`
3. Review the plan
4. `pr-remediation`

### One-shot usage

Use `pr-pipeline-orchestrator` and give it:

- the PR number or URL
- whether you want plan-only or full remediation
- whether comment-reply automation is expected

### REST API orchestration

If you want a strict pipeline, create separate tasks with the Agent Tasks API. The task creation endpoint supports:

- `prompt`
- `model`
- `custom_agent`
- `base_ref`
- `create_pull_request`

Use that to run each stage separately and poll for completion before starting the next stage.

## Recommended operating policy

- Always require a written remediation plan before code changes start.
- Always require `plan-critic` review for non-trivial plans.
- Never allow the remediation stage to finish while required CI checks are still failing.
- Treat comment-reply automation as blocked until writable GitHub MCP access is configured and verified.

## What is still manual

- Repository settings for Copilot cloud agent and the `copilot` environment
- PAT creation and permission scoping
- Any external orchestrator that creates separate agent tasks through the REST API

Those parts cannot be fully committed into the repository because GitHub stores them in repository settings rather than
source control.
