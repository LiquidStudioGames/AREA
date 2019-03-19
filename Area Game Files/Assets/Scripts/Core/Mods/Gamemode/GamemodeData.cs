public class GamemodeData
{

    string Type = "";               // The type of gamemode, ex: TDM, Payload, etc.
    int Rounds = 3;
    int RoundLength = 10;
    int Teams = 2;
    int TeamSize = 5;
    bool ZonyaEnabled = true;
    bool EyalEnabled = true;
    bool EqaEnabled = true;
    int HealthDifference = 100;     // 100 is the default (100%), 90 would be 90% of the default
    int DamageDifference = 100;     // ^^
    int AmmoDifference = 100;       // ^^

}
