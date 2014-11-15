# Avalon Edit Demo - Side by Side Diffs

Someone asked me on Twitter if it was possible to open source some of the diff rendering components we have in GitHub for Windows.

Rather than spend some time breaking out the component from the codebase, I sat down and built up a demo for how side-by-side diffs could look if I was using AvalonEdit. We do this on GitHub The Website, but not in our native apps, so this was an entertaining way to learn what changed I need to make to suppor tthis feature.

## What's Working?

Given a diff, the app breaks it up into the specific components and applies the necessary margin and background highlighting.

## What's Not Working?

Currently it's tied to a specific diff for a single - there's probably some edge cases not in place. I'd like to also support multiple files so you get to see a diff for the entire changeset.