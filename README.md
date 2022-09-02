# Unity Json Localization
This package provides an interface to prepare JSON files as localization
resources and use them in the game. It is a simple tool and does not require
programming in most cases.

## How to integrate with your project
Use the git url to import the package using the Unity's built-in package
manager by its add package from git URL functionality.

Here is the git url if you did not find it:

https://github.com/kalkuz/unity-json-localization.git

## How to use Localization in Unity
Step by step:
1. Use the menu on top of the screen and click Kalkuz Systems =>
Json Localization => Initialize Localization Assets
2. After your project files created, check them in the 
StreamingAssets/Localization folder.
3. It created en.json by default, however you can create as many language as you want.
4. The system is dynamic, so you can create entries in the json file and it
is enough. 
5. Use the components provided by package, starting with 'Localized...'.
6. The components will listen and update the content automatically when you change language.
7. If you want to change language, use LocalizationProvider.ChangeLocale() function.
