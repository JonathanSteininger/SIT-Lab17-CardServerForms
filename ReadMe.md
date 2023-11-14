# Lab17-CardServerForms

This project both contains the making of the Server and the client application.

Both applications are WinForm GUI apps.

<h3>ServerForm</h3>
The server is ServerForm which when run will start a TCP listener on localhost (127.0.0.1:2048).

If a client forms a connection it will make a new worker thread and maintain all connections seperatly.

All users are displayed in a list including theire name and current card.

<h3>ClientForm</h3>
The ClientForm application is the client application, when run the user automatically connects to the server.

The user gets given a random name but can change it at any time. The user can also request for cards 

<h3>other</h3>
The "Testing" and "Lab17-server" applications are only console based versions to test functionality.
