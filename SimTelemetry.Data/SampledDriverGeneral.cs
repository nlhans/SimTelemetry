using SimTelemetry.Objects;

namespace SimTelemetry.Data
{
    public class SampledDriverGeneral : IDriverGeneral
    {
        public SampledDriverGeneral Duplicate()
        {
            return (SampledDriverGeneral)this.MemberwiseClone();
        }

        private bool _active;

        private bool _isPlayer;

        private string _name;

        private int _baseAddress;

        private double _coordinateX;

        private double _coordinateY;

        private double _coordinateZ;

        private double _throttle;

        private double _brake;

        private double _fuel;

        private string _carModel;

        private string _carClass;

        private bool _controlAiAid;

        private bool _pitLimiter;

        private bool _pits;

        private bool _headLights;

        private int _laps;

        private float _lapTimeBest;

        private float _lapTimeLast;

        private LevelIndicator _steeringHelp;

        private int _pitStopFrontWingSetting;

        private int _pitStopRearWingSetting;

        private int _pitStopFuelSetting;

        private double _fuelSettingOffset;

        private double _fuelSettingScale;

        private double _massEmpty;

        private double _mass;

        private double _rpmStationary;

        private double _rpmMaxOffset;

        private double _rpmMaxScale;

        private double _speed;

        private double _rpm;

        private int _position;

        private int _gear;

        private int _gears;

        private float _gearRatio1;

        private float _gearRatio2;

        private float _gearRatio3;

        private float _gearRatio4;

        private float _gearRatio5;

        private float _gearRatio6;

        private float _gearRatio7;

        private float _gearRatioR;

        private float _tyreWearLf;

        private float _tyreWearRf;

        private float _tyreWearLr;

        private float _tyreWearRr;

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool IsPlayer
        {
            get { return _isPlayer; }
            set { _isPlayer = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int BaseAddress
        {
            get { return _baseAddress; }
            set { _baseAddress = value; }
        }

        public double CoordinateX
        {
            get { return _coordinateX; }
            set { _coordinateX = value; }
        }

        public double CoordinateY
        {
            get { return _coordinateY; }
            set { _coordinateY = value; }
        }

        public double CoordinateZ
        {
            get { return _coordinateZ; }
            set { _coordinateZ = value; }
        }

        public double Throttle
        {
            get { return _throttle; }
            set { _throttle = value; }
        }

        public double Brake
        {
            get { return _brake; }
            set { _brake = value; }
        }

        public double Fuel
        {
            get { return _fuel; }
            set { _fuel = value; }
        }

        public string CarModel
        {
            get { return _carModel; }
            set { _carModel = value; }
        }

        public string CarClass
        {
            get { return _carClass; }
            set { _carClass = value; }
        }

        public bool Control_AI_Aid
        {
            get { return _controlAiAid; }
            set { _controlAiAid = value; }
        }

        public bool PitLimiter
        {
            get { return _pitLimiter; }
            set { _pitLimiter = value; }
        }

        public bool Pits
        {
            get { return _pits; }
            set { _pits = value; }
        }

        public bool HeadLights
        {
            get { return _headLights; }
            set { _headLights = value; }
        }

        public int Laps
        {
            get { return _laps; }
            set { _laps = value; }
        }

        public float LapTime_Best
        {
            get { return _lapTimeBest; }
            set { _lapTimeBest = value; }
        }

        public float LapTime_Last
        {
            get { return _lapTimeLast; }
            set { _lapTimeLast = value; }
        }

        public LevelIndicator SteeringHelp
        {
            get { return _steeringHelp; }
            set { _steeringHelp = value; }
        }

        public int PitStop_FrontWingSetting
        {
            get { return _pitStopFrontWingSetting; }
            set { _pitStopFrontWingSetting = value; }
        }

        public int PitStop_RearWingSetting
        {
            get { return _pitStopRearWingSetting; }
            set { _pitStopRearWingSetting = value; }
        }

        public int PitStop_FuelSetting
        {
            get { return _pitStopFuelSetting; }
            set { _pitStopFuelSetting = value; }
        }

        public double FuelSetting_Offset
        {
            get { return _fuelSettingOffset; }
            set { _fuelSettingOffset = value; }
        }

        public double FuelSetting_Scale
        {
            get { return _fuelSettingScale; }
            set { _fuelSettingScale = value; }
        }

        public double MassEmpty
        {
            get { return _massEmpty; }
            set { _massEmpty = value; }
        }

        public double Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }

        public double RPM_Stationary
        {
            get { return _rpmStationary; }
            set { _rpmStationary = value; }
        }

        public double RPM_Max_Offset
        {
            get { return _rpmMaxOffset; }
            set { _rpmMaxOffset = value; }
        }

        public double RPM_Max_Scale
        {
            get { return _rpmMaxScale; }
            set { _rpmMaxScale = value; }
        }

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public double RPM
        {
            get { return _rpm; }
            set { _rpm = value; }
        }

        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public int Gear
        {
            get { return _gear; }
            set { _gear = value; }
        }

        public int Gears
        {
            get { return _gears; }
            set { _gears = value; }
        }

        public float GearRatio1
        {
            get { return _gearRatio1; }
            set { _gearRatio1 = value; }
        }

        public float GearRatio2
        {
            get { return _gearRatio2; }
            set { _gearRatio2 = value; }
        }

        public float GearRatio3
        {
            get { return _gearRatio3; }
            set { _gearRatio3 = value; }
        }

        public float GearRatio4
        {
            get { return _gearRatio4; }
            set { _gearRatio4 = value; }
        }

        public float GearRatio5
        {
            get { return _gearRatio5; }
            set { _gearRatio5 = value; }
        }

        public float GearRatio6
        {
            get { return _gearRatio6; }
            set { _gearRatio6 = value; }
        }

        public float GearRatio7
        {
            get { return _gearRatio7; }
            set { _gearRatio7 = value; }
        }

        public float GearRatioR
        {
            get { return _gearRatioR; }
            set { _gearRatioR = value; }
        }

        public float TyreWear_LF
        {
            get { return _tyreWearLf; }
            set { _tyreWearLf = value; }
        }

        public float TyreWear_RF
        {
            get { return _tyreWearRf; }
            set { _tyreWearRf = value; }
        }

        public float TyreWear_LR
        {
            get { return _tyreWearLr; }
            set { _tyreWearLr = value; }
        }

        public float TyreWear_RR
        {
            get { return _tyreWearRr; }
            set { _tyreWearRr = value; }
        }
    }
}