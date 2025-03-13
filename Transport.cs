using System;
using System.Security.Cryptography;
class Transport{
    private string busId;
    private string from,to;
    private int seatsAvailable;

    public Transport(string busId,string from,string to,int seatsAvailable){
        this.busId=busId;
        this.from=from;
        this.to=to;
        this.seatsAvailable=seatsAvailable;
    }

    public string BusId{
        get{return busId;}
        set{busId=value;}
    }
    public string From{
        get{return from;}
        set{from=value;}
    }
    public string To{
        get{return to;}
        set{to=value;}
    }
    public int SeatsAvailable{
        get{return seatsAvailable;}
        set{seatsAvailable=value;}
    }
}