# OSRS Account Maker

Tool to create accounts for Oldschool Runescape:

    - Supports Proxy connections for individual accounts, or a Parent Proxy for all the creation process
    - Generate Accountnames from wordlists
    - Fill the submit form on the sign-up page per button click
    - Save the accounts in huge lists

This works halfautomatic, you still have to solve the captcha to create the account.

Launch the `OSRS-Account-Creator.exe` to open the account maker. When you start the application, it creates in the same directory the directories and files it needs to work.

### Data directory
- `.\Data\Generated Accounts`: The accounts you create are saved in this directory. "GeneratedAccounts.txt" contains all of them, while "Account_Session_N.txt" stores only the accounts you created while you run the tool. If you start it another time, it will store the tool now in another "Account_Session_N.txt" file (N is the session index, which increases every restart).
- `.\Data\Proxys`: This directory contains the "Proxys.txt" from which the application takes it's proxys for individual accounts. Put the proxys in the format host:port:username:password and make a new line for each one. username:password isn't required.
- `.\Data\Wordlists`: The wordlist directory is where the wordlists for name crafting are stored. You can type in words in the "Wordlist_A.txt" and "Wordlist_B.txt" and generate names from those words. The names are saved in the same directory in the "GeneratedNames.txt" file.

### Config directory
- `.Config\OptionalParameter.txt`: This file defines the three optional parameters in the tool. If you want to add some special logic to your account creation, just adjust the content inside the file. the `[insert name]` describes the parameter name, the following lines until the next `[]` are the values.
- `.Config\OSRS-Website.txt`: URL to the OSRS sign-up page. If this URL is outdated, just replace it with the current one.
- `.Config\SavePattern.txt`: With this file you can create custom account output. The lines you type in here are present for each account you create after creation. There is a little extra syntax, the `[]`-parameters represents the checkboxes (boolean trigger) and only shows the text after it when the specific checkbox is checked, the `{}`-parameters represents the text in the textboxes, basically what you types in can be gathered with this parameters:
```
    Checkboxes - [checkbox_pin], [checkbox_parameter_a], [checkbox_parameter_b], [checkbox_parameter_c], [checkbox_proxy]
    Textboxes - {textbox_email}, {textbox_name}, {textbox_password}, {textbox_pin}, {textbox_parameter_a}, {textbox_parameter_b}, {textbox_parameter_c}, {textbox_birthdate}, {textbox_proxy_host}, {textbox_proxy_port}, {textbox_proxy_username}, {textbox_proxy_password}
```
- `.Config\Settings.txt`: Here are the paths to the Data-Files stored, you can change them to what ever you like.

### How to use the tool in general:

- The Randomize-buttons fills your textboxes with random data, if you have name a namelist and/or proxylist.
- The Change-buttons allows you to change the directory structure of the Data-Directory
- The Generate-button generates the namelist, if the "Wordlist_A.txt" and "Wordlist_B.txt" provides content
- The mail-provider textbox contains the email/trashmail-provider you chose for your accounts, "mytrashmailer.com" is standard, but you can change it
- The Parent Proxy checkbox sets a proxy for the whole application, use it, if you want the same proxy for every account you create, you can check it's connection with the button on the right side of it. If this function is disabled, the individual prxy for each account is used, except the Host-checkbox is disabled or the textboxes are "none"
- With "Start OSRS Sign-Up" you call the website and create an account
- "Fill Form" will copy your account data into the submit for of the website
- "Save Account To File" will save your accounts into your files

![OSRS-Preview](https://user-images.githubusercontent.com/59608685/148834052-82708bdd-03f6-4cfb-865d-764f5eef51eb.JPG)
