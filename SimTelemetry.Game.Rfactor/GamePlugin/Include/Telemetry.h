#ifndef TELEMTRY_H
#define TELEMTRY_H

#include "rFactorPlugin.hpp"
#include <stdio.h>

enum rFactorSessionType
{
	TESTDAY,
	PRACTICE,
	QUALIFY,
	WARMUP,
	RACE
	
};

struct rFactorDriver
{
	char VehicleName[64];

	bool IsPlayer;
	bool AIControl;
	bool InPits;

	int Position;

	float SplitBehind;
	int LapsBehind;

	float SplitLeader;
	int LapsLeader;

	float LapStartTime;

	float X;
	float Y;
	float Z;

	float Speed;
};


struct rFactorWheel
{
	float Speed;
	float Rideheight;
	float BrakeTemp;

	float TyreSlip;

	float TyreLoad;

	float TyreTemp_Inner;
	float TyreTemp_Middle;
	float TyreTemp_Outer;

	float TyrePressure;
	float TyreWear;

	bool TyreFlat;
	bool TyreMissing;

};
struct rFactorPlayer
{
	char VehicleName[64];

	float LapStart;

	int Lap;
	int Gear;

	float EngineRPM;
	float EngineRPM_Max;

	float EngineTemp_Water;
	float EngineTemp_Oil;

	float Pedals_Throttle;
	float Pedals_Brake;
	float Pedals_Clutch;
	float Pedals_Steering;

	float Fuel;

	float Speed;

	rFactorWheel Wheel_LF;
	rFactorWheel Wheel_RF;
	rFactorWheel Wheel_LR;
	rFactorWheel Wheel_RR;

	bool EngineHot;
	
	// TODO: Add acceleration.
};

struct rFactorSession
{
	char TrackName[64];

	int RaceLaps;

	int MaximumLaps;
	int Cars;
	rFactorSessionType SessionType;
	
	bool IsRace;
	bool Flag_Green;
	bool Flag_FullCourseYellow;
	bool Flags_S1;
	bool Flags_S2;
	bool Flags_S3;

	int StartLight;
	int StartLightCount;

	float Time;
	float TimeClock;
	float TimeEnd;

	float TrackTemperature;
	float AmbientTemperature;
	float Wetness_OnPath;
	float Wetness_OffPath;

	

};

struct rFactorTelemetry
{
	bool SessionRunning;
	bool PlayerDriving;

	rFactorSession Session;
	rFactorPlayer  Player;
	rFactorDriver  Drivers[128];
};

#endif