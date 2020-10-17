# PenguorCS

![license](https://img.shields.io/github/license/Penguor/PenguorCS?style=flat-square)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/Penguor/PenguorCS/.NET%20Core?style=flat-square)

This repository contains the source code of the PenguorCS compiler for the Penguor language.

## Overview

Penguor is an in-development data-oriented language. Currently, it is in a _very_ early stage and it does not have many features yet. The ultimate goal is to create a fast, feature-rich language optimized for games and other applications which need to handle lots of data.

Work-in-progress documentation can soon be found [here](https://penguor.readthedocs.io/).

## Example

The following code will show a "Hello World" program written in Penguor:

```Penguor
public system Hello
{
    void execute()
    {
        print("Hello World")
    }
}
```

## Contribute

If you are new to programming language development and data-oriented languages, I recommend checking out the following resources:

- <http://craftinginterpreters.com/> An online book about compiler building.
- <http://www.dataorienteddesign.com/site.php> A book about data-oriented design.

If you want to contribute check out [CONTRIBUTING](./CONTRIBUTING.md)

## Community

- Discord: <https://discord.gg/TvzW96H>
- Gitter: <https://gitter.im/Penguor>

## License

Penguor is licensed under the [MIT-License](./LICENSE).
