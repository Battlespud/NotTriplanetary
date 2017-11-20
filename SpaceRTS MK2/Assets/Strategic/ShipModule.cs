using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
public enum ShipModuleType
{
    //engine
    Conventional = 100,
    Nuclear,
    Fission,
    Ion,
    Magneto,
    Fusion,
    MagneticFusion,
    InertialFusion,
    Antimatter,
    Photonic,
    //capacity
    Fuel = 200,
    Cargo = 210,
    Hangar = 220,
    //personnel
    Maintenance = 300,
    Medical,
    Combat,
    //capabilities
    Infrared = 400,
    Electromagnetic,
    Optical,
    Active,
    Passive,
    DockingBay = 410,
    Communications = 420,
    //control
    Bridge = 500,
    Vectoring = 510,
    //Offensive
    Gauss = 600,
    Railgun,
    Missile,
    Laser = 610,
    Plasma,
    //Defensive
    LightArmour = 700,
    HeavyArmour,
    Shielding = 710
}

//these values represent that which are used by one, 2 or more but not all modules
public enum ShipModuleValue
{
    //since most of these values are float, the ones that arent will have to be configured slightly to mimic an int or bool (or w.e type)
    //general values that can be shared by 2 or more
    I_Available = 0, //int
    I_Maximum, //int
    Efficiency, //float
    Damage, //float
    PowerModifier = 100, //float
    ThrustForce, //float
    ExplosionChance = 200, //float
    ActiveUsagePerTurn, //float
    I_ActiveCrew = 300, //int
    Sensitivity = 400, //float
    Hardening, // float
    B_FleetCommander = 500, //bool (float -> int (0-1) -> bool)
    TurnSpeed = 510, //float
    FireRate = 600, //float
    PulseRate = 610, // float
    Integrity = 700, //float
    Frequency = 710//float
}
public enum ShipModuleInts : byte
{
    ID = 0,
    Mass = 50,
    HitsToKill = 5
}
public enum ShipModuleBools : byte
{
    IsDefault = 10,
    IsRequired = 20,
    IsObsolete = 30,
    IsDamaged = 40,
    IsEnabled = 51,
    IsToggleable = 60,
    IsInterior = 70
}
public enum ShipModuleFloats : byte
{
    PowerDrain,// = 15,
    PowerRequirement,// = 150,
    MaintenanceUpkeep,// = 4,
    MaintenanceRequirement,// = 40,
    InfraredSignature,// = 20,
    ElectromagneticSignature// = 10
}

[Serializable]
public class ShipModule
{
    //public static Dictionary<int, ShipModule> DesignedModules = new Dictionary<int, ShipModule>();
    public static List<ShipModule> DesignedModules = new List<ShipModule>();
    //static string ModulesFilePath = Path.Combine(Application.streamingAssetsPath, "Components/");
    static string ModulesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Components/");

    static ShipModule()
    {
        if (Directory.Exists(ModulesFilePath)) { return; }
        else { Directory.CreateDirectory(ModulesFilePath); }
    }

    public ShipModuleType Type { get; set; }
    public Dictionary<ShipModuleValue, float> Values { get; set; }
    //public Dictionary<RawResources, float> Cost { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    string moduleFileName;

    public Dictionary<ShipModuleInts, int> Ints { get; set; }
    public Dictionary<ShipModuleBools, bool> Bools { get; set; }
    public Dictionary<ShipModuleFloats, float> Floats { get; set; }

    public ShipModule(ShipModuleType type, string name = null, string description = null)
    {
        Type = type;
        Name = name ?? type.ToString();
        Description = description ?? "No description";
        moduleFileName = Path.Combine(ModulesFilePath, Name + "_Module.xml");
        Values = new Dictionary<ShipModuleValue, float>();
        //Cost = new Dictionary<RawResources, float>();
        PopulateGenericFields();
        PopulateTypeValuePairs();
        DesignedModules.Add(this);
    }

    //is it a deepclone or a reference clone
    public ShipModule Clone(bool IsDeepClone = true)
    {
        ShipModule DeepClone = new ShipModule(Type);

        if (IsDeepClone)
        {
            DeepClone = this;
            //copy full
        }
        else
        {
            //copy properties
        }

        return IsDeepClone ? DeepClone : null;
    }

    void PopulateGenericFields()
    {
        Ints = new Dictionary<ShipModuleInts, int>();
        foreach (ShipModuleInts field in Enum.GetValues(typeof(ShipModuleInts)))
        {
            Ints.Add(field, (int)field);
        }

        Bools = new Dictionary<ShipModuleBools, bool>();
        foreach (ShipModuleBools field in Enum.GetValues(typeof(ShipModuleBools)))
        {
            Bools.Add(field, (int)field % 10 == 0 ? false : true);
        }

        Floats = new Dictionary<ShipModuleFloats, float>();
        foreach (ShipModuleFloats field in Enum.GetValues(typeof(ShipModuleFloats)))
        {
            float value;
            switch (field)
            {
                case ShipModuleFloats.PowerDrain:
                    value = .15f;
                    break;
                case ShipModuleFloats.PowerRequirement:
                    value = 1.5f;
                    break;
                case ShipModuleFloats.MaintenanceUpkeep:
                    value = .00025f;
                    break;
                case ShipModuleFloats.MaintenanceRequirement:
                    value = .04f;
                    break;
                case ShipModuleFloats.InfraredSignature:
                    value = 25f;
                    break;
                case ShipModuleFloats.ElectromagneticSignature:
                    value = 10f;
                    break;
                default:
                    value = 0f;
                    break;
            }
            Floats.Add(field, value);
        }
    }

    public void PopulateTypeValuePairs()
    {
        int BaseHundredIndex = (int)Type / 100;
        foreach (ShipModuleValue upper in Enum.GetValues(typeof(ShipModuleValue)))
        {
            if ((int)upper < 100)
            {
                continue;
            }
            else
            {
                int UpperComparator = (int)upper / 100;
                if (BaseHundredIndex == UpperComparator)
                {
                    int BaseTenIndex = (int)BaseHundredIndex / 10;
                    foreach (ShipModuleValue lower in Enum.GetValues(typeof(ShipModuleValue)))
                    {
                        int LowerComparator = ((int)lower / 100) / 10;
                        bool UpperBaseEqual = BaseHundredIndex == UpperComparator ? true : false;
                        bool LowerBaseEqual = BaseTenIndex == LowerComparator ? true : false;

                        if ((UpperBaseEqual && LowerBaseEqual))
                        {
                            Values.Add(lower, (float)lower);
                        }
                        else if ((BaseHundredIndex < UpperComparator) || (BaseTenIndex < LowerComparator)) { return; }
                        else { continue; }
                    }
                }
            }
        }
    }

    public object GetModuleValue(ShipModuleValue valueType)
    {
        string temp = valueType.ToString();

        if (temp[1] != '_')
        {
            return Values[valueType];
        }
        else
        {
            if (temp[0] == 'I')
            {
                return (int)Values[valueType];
            }
            else
            {
                return (int)Values[valueType] == 0 ? false : true;
            }
        }
    }

    public void SetModuleValue(ShipModuleValue valueType, float value)
    {
        string temp = valueType.ToString();

        if (temp[1] != '_')
        {
            Values[valueType] += value;
        }
        else
        {
            int tempValue = (int)value;

            if (temp[0] == 'I')
            {
                Values[valueType] += (float)tempValue;
            }
            else
            {
                Values[valueType] = tempValue != 0 || tempValue != 1 ? 0f : value;
            }
        }
    }

    public override string ToString()
    {
        StringBuilder outerBuilder = new StringBuilder();
        outerBuilder.AppendLine(Name);
        outerBuilder.AppendLine(Description);
        List<object> fields = new List<object>() { Ints, Bools, Floats };
        for (int i = 0; i < 3; i++)
        {
            StringBuilder innerBuilder = new StringBuilder();
            switch (i)
            {
                //i dont like this redundancy
                case 0:
                    foreach (var kvp in (fields[i] as Dictionary<ShipModuleInts, int>))
                    {
                        innerBuilder.AppendFormat("{0} : {1}\n", kvp.Key, kvp.Value);
                    }
                    break;
                case 1:
                    foreach (var kvp in (fields[i] as Dictionary<ShipModuleBools, bool>))
                    {
                        innerBuilder.AppendFormat("{0} : {1}\n", kvp.Key, kvp.Value);
                    }
                    break;
                case 2:
                    foreach (var kvp in (fields[i] as Dictionary<ShipModuleFloats, float>))
                    {
                        innerBuilder.AppendFormat("{0} : {1}\n", kvp.Key, kvp.Value);
                    }
                    break;
                default:
                    break;
            }
            outerBuilder.Append(innerBuilder.ToString());
        }

        foreach (var kvp in Values)
        {
            outerBuilder.AppendFormat("{0} : {1}\n", ((ShipModuleValue)kvp.Value).ToString(), kvp.Value.ToString());
        }
        outerBuilder.AppendLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        return outerBuilder.ToString();
    }
}