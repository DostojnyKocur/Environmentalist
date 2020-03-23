# Environmentalist

Environmentalist is a simple tool to generate configuration files based on a template.

## Usage

The tool takes one parameter with a configuration file:

`Environmentalist.exe configfile.conf`

### Configuration

The configuration consists of several files.

#### configfile.conf

This file describes input and output.

```
templatePath=template.env
resultPath=result.env
configPath=conf1.txt
secureVaultPath=secrets.kdbx
secureVaultPass=password
```

`templatePath` is a path to a template which is taken as a source of output file.
`resultPath` is a path to an output file. This file will be created based on the template and configuration.
`configPath` is a path to a configuration file. The configuration file describes how to fill templates with values.
`secureVaultPath` is a path to a KeePass database file.
`secureVaultPass` is password to a KeePass database file.

#### template

The file is a source for the output file.

Template must be in form of `Key=Value`, where `Key` and `Value`are any valid strings. The `Values` will be replaced by specific values from config file.
`Value` can be also already fill with a real value or be `path to secret`.

E.g.:

```
KEY0=[KeePass](test_entry)
KEY1=VALUE1
KEY2=VALUE2
KEY3=VALUE3
KEY4=VALUE4
```

`[KeePass](test_entry)` means that output file will be filled with a password from KeePass for entry `test_entry`. Valid formulas are title of entry or username in entry.

#### config

The file has the same structure as `template`.
Values from this file are copied to template file and evaluated.

E.g.:

```
VALUE2=[KeePass](test_user)
VALUE4=[KeePass](test_entry)
VALUE3=some_value
```

Placeholders like `VALUE2` must have corresponding placeholder in the template file. If there are values in the template file which have no corresponding entries in the config file then will be left as they are.
