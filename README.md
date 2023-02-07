# Spark
Spark is a distributed server messager, it is not like other P2P or distributed server communication software. \n
It do not need to deploy server. It uses the FTP server to transfer encryption TXT file to transfer messages.\n
It depends on how many FTP server account your group has, the message channel will be changed based on a algorithm, so it is safe.  

## How to install it?
 1. Before you download this app, you should download some other modules that the app depends on.
  - [AddressSelecter](https://github.com/JamesYanGit/AddressSelecter): It is used to choose message channel for the app.
  - [FileSystemModule](https://github.com/JamesYanGit/FileSystemModule): It is used to operate files which relate to the app.
  - [FTPHelper](https://github.com/JamesYanGit/FTPHelper): It is used to detect FTP server state.
  - [TimeAdjuster](https://github.com/JamesYanGit/TimeAdjuster): It is used to adjust clocks between different clients so that they can respect the same algorithm.

## How to use it?
Download all modules, and import them into one solution. \n
Add the modules you downloaded to dependencies of the Spark.\n
Run it\n
 - "My Profile" button. Spark will generate a code, you send this code to another user. They can use this code to add you as a contact.
 - "Add Contact" button. When you get any code from other users, fill it in to the text box, and Click "Add". 
 - Above these two button, there is a list for your contacts. 
 - At top of left, the chat history will be display over there.
 - At the bottom of left, the text box is used to input message and "Send" button is used to send message. 
