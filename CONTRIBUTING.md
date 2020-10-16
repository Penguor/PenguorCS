# Contributing to Penguor

First of all, thanks for considering to contribute to Penguor (or for just reading this document).
This file provides information on how to help with shaping the Penguor language and the compiler.

## Repositories and organization

All the source code for the Penguor project is hosted on GitHub in the [Penguor Organisation](https://github.com/Penguor).
There you can find several repositories:

| Repository  | Description                                                                                                                                                        |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| PenguorCS   | This is the home to the Penguor compiler. It is written in C#.                                                                                                     |
| PenguorDocs | This repository contains the documentation for Penguor and it's grammar. It is the place where new features get developed and design guidelines can be found.      |
| PenguorVSC  | The repo contains the Visual Studio Code extension for Penguor. I (CozyPenguin) develop Penguor solely using VSCode so the syntax highlighting is essential to me. |
| PenguorHS   | This repo contains the currently not maintained and unfinished Penguor compiler in Haskell. It is using megaparsec.                                                |

Most of the repositories have at least two branches. The _main_ branch contains the code at the point of the last release, while the _develop_ branch contains the most recent code

## Reporting a bug

If you have found a bug, you should open an issue [here](https://github.com/Penguor/PenguorCS/issues/new).

## Contributing Code

### Where to start

When getting into a new open source project, it can often be quite hard to get into the code and find something easy to do for a start.
Currently, the project is to small to have any issues as it is unfinished and there is basically only one developer working on it.
But otherwise, there should be issues marked as `good first issue`.

If you have an idea how to improve Penguor yourself, you should look if anybody else already opened an issue or a pull request for it.
If this isn't the case, you should either open an issue or, if you would like to implement it yourself, join our [Discord](https://discord.gg/9PJf676) so that we can get in touch and discuss whether we would like to see your idea in Penguor.

### Developing the feature

Once you have found a bug you want to fix or a feature you want to implement, you should fork the corresponding repository and create a new branch where all the changed code resides.
Then, you should open a pull request in the original repository so that people can review your code and we are able to track your progress.

The code you write should be well-commented, this includes xml documentation comments for all public members.

### Merging

Once the GitHub actions pass and one contributor has reviewed all the code it can be merged (if it is complete).

## Getting help

If you're stuck, check out our [Discord](https://discord.gg/9PJf676) or our [Gitter](https://gitter.im/Penguor).
