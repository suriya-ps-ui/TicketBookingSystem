using System;

[Serializable]
class User{
    private string userName;
    private string password;
    private Dictionary<Transport,int> ticketsBooked;

    public User(string userName,string password){
        this.userName=userName;
        this.password=password;
        ticketsBooked=new Dictionary<Transport, int>();
    }

    public string UserName{
        get{return userName;}
        set{userName=value;}
    }
    public string Password{
        get{return password;}
        set{password=value;}
    }
    public Dictionary<Transport,int> TicketsBooked(){
        return ticketsBooked;
    }
    public void addTickets(Transport transport,int seats){
        ticketsBooked.Add(transport,seats);
    }
    public void removeTickets(Transport transport){
        ticketsBooked.Remove(transport);
    }
}