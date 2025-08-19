# GitHub User Activity Console Application
Project task URL: https://roadmap.sh/projects/github-user-activity
A simple command line interface (CLI) to fetch the recent activity of a GitHub user and display it in the terminal. <br />

Handled events: <br />
- PushEvent
- PullRequestEvent
- IssuesEvent
- WatchEvent
- CreateEvent

Provide the GitHub username as an argument when running the CLI.<br />
`github-activity <username>`

Output:
```
- Pushed 3 commits to kamranahmedse/developer-roadmap
- Opened a new issue in kamranahmedse/developer-roadmap
- Starred kamranahmedse/developer-roadmap
- ...
```
