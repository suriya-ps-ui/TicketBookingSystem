using System;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
class Program{
    static Dictionary<string,User> users=new Dictionary<string, User>();
    static Dictionary<string,Transport> transports=new Dictionary<string, Transport>();
    static Dictionary<string,string> adminDetails=new Dictionary<string, string>();
    static string adminFile="admin.json";
    static string usersFile="users.json";
    static string transportsFile="transports.json";
    static void loadData(){
        try{
            if(File.Exists(adminFile)){
                adminDetails=JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(adminFile))??new Dictionary<string,string>();
            }
            if(File.Exists(usersFile)){
                users=JsonSerializer.Deserialize<Dictionary<string,User>>(File.ReadAllText(usersFile))??new Dictionary<string,User>();
            }
            if(File.Exists(transportsFile)){
                transports=JsonSerializer.Deserialize<Dictionary<string,Transport>>(File.ReadAllText(transportsFile))??new Dictionary<string,Transport>();
            }
        }catch(Exception ex){
            System.Console.WriteLine($"Error while Loading Files:{ex.Message}");
        }
    }
    static void saveData(){
        try{
            string usersJson=JsonSerializer.Serialize(users);
            File.WriteAllText(usersFile,usersJson);
            string transportJson=JsonSerializer.Serialize(transports);
            File.WriteAllText(transportsFile,transportJson);
        }catch(Exception ex){
            System.Console.WriteLine($"Error while writing Files:{ex.Message}");
        }
    }
    static bool adminLoginVerify(){
        string userName,password;
        System.Console.WriteLine("LOGIN");
        System.Console.WriteLine("Enter your Username:");
        userName=Console.ReadLine()??"";
        System.Console.WriteLine("Enter your Password:");
        password=Console.ReadLine()??"";
        if(!adminDetails.ContainsValue(userName)){
            return false;
        }
        if(adminDetails[userName]!=password){
            return false;
        }
        return true;
    }    
    static void addTransport(){
        string busId,from,to;
        int seatsAvailable;
        System.Console.WriteLine("Creating New Route");
        System.Console.WriteLine("Enter From:");
        from=Console.ReadLine()??"";
        System.Console.WriteLine("Enter To:");
        to=Console.ReadLine()??"";
        System.Console.WriteLine("Enter Seats Available:");
        seatsAvailable=Convert.ToInt32(Console.ReadLine());
        changeBusId:
        System.Console.WriteLine("Enter BusID:");
        busId=Console.ReadLine()??"";
        if(transports.ContainsKey(busId)){
            System.Console.WriteLine("Give Different BusID.");
            goto changeBusId;
        }
        Transport newTransport=new Transport(busId,from,to,seatsAvailable);
        transports.Add(busId,newTransport);
    }
    static void removeTransport(){
        string busId;
        System.Console.WriteLine("Removing Route");
        changeBusId:
        System.Console.WriteLine("Enter BusID:");
        busId=Console.ReadLine()??"";
        if(!transports.ContainsKey(busId)){
            System.Console.WriteLine("Give valid BusID.");
            goto changeBusId;
        }
        transports.Remove(busId);
    }
    static void adminPortal(){
        String addOrRemove,exitPortal;
        if(!adminLoginVerify()){
            System.Console.WriteLine("Wrong Username or Password.");
        }
        adminContinue:
        System.Console.WriteLine("Are you going to add or remove transport(A/R):");
        addOrRemove=Console.ReadLine()??"A";
        if(addOrRemove.ToLower()=="a"){
            addTransport();
        }
        if(addOrRemove.ToLower()=="r"){
            removeTransport();
        }
        System.Console.WriteLine("Do you Like to Continue or Exit admin portal(C/E):");
        exitPortal=Console.ReadLine()??"E";
        if(exitPortal.ToLower()=="c"){
            goto adminContinue;
        }
    }
    static User? userLoginVerify(){
        string userName,password;
        System.Console.WriteLine("LOGIN");
        System.Console.WriteLine("Enter your Username:");
        userName=Console.ReadLine()??"";
        System.Console.WriteLine("Enter your Password:");
        password=Console.ReadLine()??"";
        if(!users.ContainsKey(userName)){
            return null;
        }
        User user=(User)users[userName];
        if(user.Password!=password){
            return null;
        }
        return user;
    }
    static void createUser(){
        string userName,password;
        invalidUserName:
        System.Console.WriteLine("Creating New Account");
        System.Console.WriteLine("Enter your Username:");
        userName=Console.ReadLine()??"";
        System.Console.WriteLine("Enter your Password:");
        password=Console.ReadLine()??"";
        if(users.ContainsKey(userName)){
            System.Console.WriteLine("This Username is already taken.");
            goto invalidUserName;
        }
        User newUser=new User(userName,password);
        users.Add(userName,newUser);
    }
    static void bookTicket(Transport transport,User user,int seats){
        transport.SeatsAvailable-=seats;
        user.addTickets(transport,seats);
    }
    static void cancelTicket(Transport transport,User user){
        Dictionary<Transport,int> ticketsBooked=user.TicketsBooked();
        int seatsBooked=ticketsBooked[transport];
        ticketsBooked.Remove(transport);
        transport.SeatsAvailable+=seatsBooked;
    }
    static void userTickets(User user){
        Dictionary<Transport,int> ticketsBooked=user.TicketsBooked();
        System.Console.WriteLine($"Tickets Booked by {user.UserName}.");
        foreach(var iteam in ticketsBooked){
            System.Console.WriteLine($"BusID:{iteam.Key.BusId} Seats:{iteam.Value}");
        }
    }
    static bool checkBusBooked(User user,Transport transport){
        bool isBus=false;
        Dictionary<Transport,int> ticketsBooked=user.TicketsBooked();
        foreach(var iteam in ticketsBooked){
            if(iteam.Key==transport){
                isBus=true;
                break;
            }
        }
        return isBus;
    }
    static void userPortal(){
        string userType,busId;
        int userOptions,seatsNeeded;
        bool flag=true;
        Transport transport;
        System.Console.WriteLine("Are you Existing or New User(E/N):");
        userType=Console.ReadLine()??"N";
        if(userType.ToLower()=="n"){
            createUser();
        }
        User user=userLoginVerify();
        if(user==null){
            System.Console.WriteLine("Wrong Username or Password.");
            return;
        }
        while(flag){
            System.Console.WriteLine("\nWelcome to Ticket Booking:\n1.View All Travels\n2.Book Ticket\n3.Cancel Ticket\n4.View Booked Tickets\n5.Exit Portal.\n");
            userOptions=Convert.ToInt32(Console.ReadLine());
            switch(userOptions){
                case 1:
                    foreach(var tranport in transports){
                        Transport transportDetails=(Transport)tranport.Value;
                        System.Console.WriteLine($"BusID:{transportDetails.BusId}\nFrom {transportDetails.From} To {transportDetails.To}\nAvailable Seats:{transportDetails.SeatsAvailable}\n");
                    }
                    break;
                case 2:
                    System.Console.WriteLine("\nBook Tickets");
                    changeBusID:
                    System.Console.WriteLine("Enter Bus ID:");
                    busId=Console.ReadLine()??"";
                    if(!transports.ContainsKey(busId)){
                        System.Console.WriteLine("Wrong BusID.");
                        goto changeBusID;
                    }
                    System.Console.WriteLine("Enter number of seats you need:");
                    seatsNeeded=Convert.ToInt32(Console.ReadLine());
                    transport=(Transport)transports[busId];
                    if(seatsNeeded>transport.SeatsAvailable){
                        System.Console.WriteLine("Not Enough Seats.");
                        break;
                    }
                    bookTicket(transport,user,seatsNeeded);
                    System.Console.WriteLine("Your Ticket has been Booked.");
                    break;
                case 3:
                    userTickets(user);
                    System.Console.WriteLine("Which Bus would you like to cancel enter busID:");
                    changeBusId:
                    busId=Console.ReadLine()??"";
                    if(!transports.ContainsKey(busId)){
                        System.Console.WriteLine("Wrong BusID.");
                        goto changeBusId;
                    }
                    transport=(Transport)transports[busId];
                    if(!checkBusBooked(user,transport)){
                        System.Console.WriteLine("User doesn't have tickets in this bus.");
                        break;
                    }
                    cancelTicket(transport,user);
                    System.Console.WriteLine("Your ticket has been canceled.");
                    break;
                case 4:
                    userTickets(user);
                    break;
                case 5:
                    flag=false;
                    break;
                default:
                    System.Console.WriteLine("Wrong Choice.");
                    break;
            }
        }
    }
    static void Main(string[] args){
        loadData();
        string loginAs;
        System.Console.WriteLine("Admin or User:");
        loginAs=Console.ReadLine()??"User";
        if(loginAs.ToLower()=="admin"){
            adminPortal();
        }else{
            userPortal();
        }
        saveData();
    }
}