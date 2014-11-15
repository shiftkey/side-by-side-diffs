# Avalon Edit Demo - Side by Side Diffs

Someone asked me on Twitter if it was possible to open source some of the diff rendering components we have in GitHub for Windows.

Rather than spend some time breaking out the component from the codebase, I sat down and built up a demo for how side-by-side diffs could look if I was using AvalonEdit. We do this on GitHub The Website, but not in our native apps, so this was an entertaining way to learn what changed I need to make to suppor tthis feature.

## What's Working?

Given a diff, the app breaks it up into the specific components and applies the necessary margin and background highlighting.

## What's Not Working?

  - word wrapping on one side breaks the other diff - how can we keep thsoe in sync?
  - tied to a specific diff - probably some :poop: edge cases
  - it should render a list of files - make it more generic/flexible