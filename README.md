# GitHub User Activity Console Application
A simple command line interface (CLI) to fetch the recent activity of a GitHub user and display it in the terminal. <br />

Provide the GitHub username as an argument when running the CLI.<br />
`github-activity <username>`

Fetch the recent activity of the specified GitHub user using the GitHub API. You can use the following endpoint to fetch the user’s activity:<br />

```
https://api.github.com/users/<username>/events
Example: https://api.github.com/users/kamranahmedse/events
```
Display the fetched activity in the terminal. <br />
Output:
```
- Pushed 3 commits to kamranahmedse/developer-roadmap
- Opened a new issue in kamranahmedse/developer-roadmap
- Starred kamranahmedse/developer-roadmap
- ...
```