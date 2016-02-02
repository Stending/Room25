using UnityEngine;
using System.Collections;
 [System.Serializable]


public class UserInfos{

    private string ip;
    private string name;
    private CaracterType type;
    private Role role = Role.UNKNOWN;

    public UserInfos(string ip, string name)
    {
        this.ip = ip;
        this.name = name;
    }

    public string Ip
    {
        get { return ip; }
        set { ip = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public CaracterType Type
    {
        get { return type; }
        set { type = value; }
    }

    public Role Role
    {
        get { return role; }
        set { role = value; }
    }
}
