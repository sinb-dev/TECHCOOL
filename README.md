# TECHCOOL
These are tools and libraries used by programming students of TECHCOLLEGE

# WebLet
A thin in-app webserver that can be bundle with a program to allow communication throw a webbrowser
## Basic example
`//Specify which URL is intended for the server. Don't forget a trailing / in the address
WebLet server = new WebLet("http://localhost:8080/"); 

//Add a page to the webserver
server.Route("index.html", request => "Privyet  comrade!");

//Start the server
server.Start();`
# SQLet
A shallow SQL client that can connect to SQL Server or SQLite and allow non-queries to be executed and queries to be returned in the form of an array
