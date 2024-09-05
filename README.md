# Custom TECHCOOL
A specialized fork of Techcool, tailored to implement custom features and enhancements, addressing specific requirements and optimizations.

# WebLet
A thin in-app webserver that can be bundle with a program to allow communication throw a webbrowser
## Basic example
`//Specify which URL is intended for the server. Don't forget a trailing / in the address`
`WebLet server = new WebLet("http://localhost:8080/");`

`//Add a page to the webserver`
`server.Route("index.html", request => "Privyet  comrade!");`

`//Start the server`
`server.Start();`

Open a webbrowser and visit [`http://localhost:8080`](http://localhost:8080)

# SQLet
A shallow SQL client that can connect to SQL Server or SQLite and allow non-queries to be executed and queries to be returned in the form of an array
