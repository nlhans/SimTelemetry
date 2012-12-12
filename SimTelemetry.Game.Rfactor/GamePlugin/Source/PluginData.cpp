
#include "rFactorPlugin.hpp"          // corresponding header file
#include "Telemetry.h"
#include <math.h>
#include <stdio.h>

rFactorTelemetry* telem;
bool telem_allocated;

void SimTelemetryPlugin::WriteToAllExampleOutputFiles( const char * const openStr, const char * const msg )
{
	FILE *fo;

	fo = fopen( "ExampleInternalsTelemetryOutput.txt", openStr );
	if( fo != NULL )
	{
		fprintf( fo, "%s\n", msg );
		fclose( fo );
	}

	fo = fopen( "ExampleInternalsGraphicsOutput.txt", openStr );
	if( fo != NULL )
	{
		fprintf( fo, "%s\n", msg );
		fclose( fo );
	}

	fo = fopen( "ExampleInternalsScoringOutput.txt", openStr );
	if( fo != NULL )
	{
		fprintf( fo, "%s\n", msg );
		fclose( fo );
	}
}


void SimTelemetryPlugin::Startup()
{
	mem = new SharedMemory(TEXT("Local\\SimTelemetryRfactor2"), sizeof(rFactorTelemetry));

	// We have got success.
	if (mem->Hooked())
	{
		telem = (rFactorTelemetry*) mem->GetBuffer();
		telem_allocated = true;

	}

	WriteToAllExampleOutputFiles( "w", "-STARTUP-" );
}


void SimTelemetryPlugin::Shutdown()
{
	telem_allocated = false;
	delete mem;
	mem = NULL;
	telem = NULL;

	WriteToAllExampleOutputFiles( "a", "-SHUTDOWN-" );
}


void SimTelemetryPlugin::StartSession()
{
	telem->SessionRunning=true;

	WriteToAllExampleOutputFiles( "a", "--STARTSESSION--" );
}


void SimTelemetryPlugin::EndSession()
{
	telem->SessionRunning=false;

	WriteToAllExampleOutputFiles( "a", "--ENDSESSION--" );
}


void SimTelemetryPlugin::EnterRealtime()
{
	telem->PlayerDriving=true;

	mET = 0.0f;
	WriteToAllExampleOutputFiles( "a", "---ENTERREALTIME---" );
}


void SimTelemetryPlugin::ExitRealtime()
{
	telem->PlayerDriving=false;

	WriteToAllExampleOutputFiles( "a", "---EXITREALTIME---" );
}

void SimTelemetryPlugin::UpdateWheelTelemetry (const TelemInfoV2& info, int i, rFactorWheel* wheel)
{
	wheel->BrakeTemp = info.mWheel[i].mBrakeTemp;
	wheel->Rideheight = info.mWheel[i].mRideHeight;
	wheel->Speed = info.mWheel[i].mRotation;
	wheel->TyreFlat = info.mWheel[i].mFlat;
	wheel->TyreLoad=info.mWheel[i].mTireLoad;
	wheel->TyreMissing = info.mWheel[i].mDetached;
	wheel->TyrePressure = info.mWheel[i].mPressure;
	wheel->TyreSlip = 0;
	wheel->TyreTemp_Inner = info.mWheel[i].mTemperature[0];
	wheel->TyreTemp_Middle = info.mWheel[i].mTemperature[1];
	wheel->TyreTemp_Outer = info.mWheel[i].mTemperature[2];
	wheel->TyreWear = info.mWheel[i].mWear;

}

void SimTelemetryPlugin::UpdateTelemetry( const TelemInfoV2 &info )
{
	telem->Player.EngineRPM = info.mEngineRPM;
	telem->Player.EngineRPM_Max = info.mEngineMaxRPM;
	telem->Player.EngineHot = info.mOverheating;
	telem->Player.EngineTemp_Oil = info.mEngineOilTemp;
	telem->Player.EngineTemp_Water = info.mEngineWaterTemp;

	telem->Player.Fuel = info.mFuel;
	telem->Player.Gear= info.mGear;
	telem->Player.Lap = info.mLapNumber;
	telem->Player.LapStart= info.mLapStartET;
	telem->Player.Pedals_Brake = info.mUnfilteredBrake;
	telem->Player.Pedals_Throttle=info.mUnfilteredThrottle;
	telem->Player.Pedals_Clutch = info.mUnfilteredClutch;
	telem->Player.Pedals_Steering=info.mUnfilteredSteering;
	strcpy(telem->Player.VehicleName, info.mVehicleName);

	float metersPerSec = sqrtf( ( info.mLocalVel.x * info.mLocalVel.x ) +
		( info.mLocalVel.y * info.mLocalVel.y ) +
		( info.mLocalVel.z * info.mLocalVel.z ) );
	telem->Player.Speed = metersPerSec;

	UpdateWheelTelemetry(info, 0, &telem->Player.Wheel_LF);
	UpdateWheelTelemetry(info, 1, &telem->Player.Wheel_RF);
	UpdateWheelTelemetry(info, 2, &telem->Player.Wheel_LR);
	UpdateWheelTelemetry(info, 3, &telem->Player.Wheel_RR);

	// IS THIS ALL?
	// DISAPPOINTED ISI.
	return;


	// Use the incoming data, for now I'll just write some of it to a file to a) make sure it
	// is working, and b) explain the coordinate system a little bit (see header for more info)
	FILE *fo = fopen( "ExampleInternalsTelemetryOutput.txt", "a" );
	if( fo != NULL )
	{
		// Delta time is variable, as we send out the info once per frame
		fprintf( fo, "DT=%.4f\n", info.mDeltaTime );
		fprintf( fo, "Lap=%d StartET=%.3f\n", info.mLapNumber, info.mLapStartET );
		fprintf( fo, "Vehicle=%s\n", info.mVehicleName );
		fprintf( fo, "Track=%s\n", info.mTrackName );
		fprintf( fo, "Pos=(%.3f,%.3f,%.3f)\n", info.mPos.x, info.mPos.y, info.mPos.z );

		// Forward is roughly in the -z direction (although current pitch of car may cause some y-direction velocity)
		fprintf( fo, "LocalVel=(%.2f,%.2f,%.2f)\n", info.mLocalVel.x, info.mLocalVel.y, info.mLocalVel.z );
		fprintf( fo, "LocalAccel=(%.1f,%.1f,%.1f)\n", info.mLocalAccel.x, info.mLocalAccel.y, info.mLocalAccel.z );

		// Orientation matrix is left-handed
		fprintf( fo, "[%6.3f,%6.3f,%6.3f]\n", info.mOriX.x, info.mOriX.y, info.mOriX.z );
		fprintf( fo, "[%6.3f,%6.3f,%6.3f]\n", info.mOriY.x, info.mOriY.y, info.mOriY.z );
		fprintf( fo, "[%6.3f,%6.3f,%6.3f]\n", info.mOriZ.x, info.mOriZ.y, info.mOriZ.z );
		fprintf( fo, "LocalRot=(%.3f,%.3f,%.3f)\n", info.mLocalRot.x, info.mLocalRot.y, info.mLocalRot.z );
		fprintf( fo, "LocalRotAccel=(%.2f,%.2f,%.2f)\n", info.mLocalRotAccel.x, info.mLocalRotAccel.y, info.mLocalRotAccel.z );

		// Vehicle status
		fprintf( fo, "Gear=%d RPM=%.1f RevLimit=%.1f\n", info.mGear, info.mEngineRPM, info.mEngineMaxRPM );
		fprintf( fo, "Water=%.1f Oil=%.1f\n", info.mEngineWaterTemp, info.mEngineOilTemp );
		fprintf( fo, "ClutchRPM=%.1f\n", info.mClutchRPM );

		// Driver input
		fprintf( fo, "UnfilteredThrottle=%.1f%%\n", 100.0f * info.mUnfilteredThrottle );
		fprintf( fo, "UnfilteredBrake=%.1f%%\n", 100.0f * info.mUnfilteredBrake );
		fprintf( fo, "UnfilteredSteering=%.1f%%\n", 100.0f * info.mUnfilteredSteering );
		fprintf( fo, "UnfilteredClutch=%.1f%%\n", 100.0f * info.mUnfilteredClutch );

		// Misc
		fprintf( fo, "SteeringArmForce=%.1f\n", info.mSteeringArmForce );
		fprintf( fo, "Fuel=%.1f ScheduledStops=%d Overheating=%d Detached=%d\n", info.mFuel, info.mScheduledStops, info.mOverheating, info.mDetached );
		fprintf( fo, "Dents=(%d,%d,%d,%d,%d,%d,%d,%d)\n", info.mDentSeverity[0], info.mDentSeverity[1], info.mDentSeverity[2], info.mDentSeverity[3],
			info.mDentSeverity[4], info.mDentSeverity[5], info.mDentSeverity[6], info.mDentSeverity[7] );
		fprintf( fo, "LastImpactET=%.1f Mag=%.1f, Pos=(%.1f,%.1f,%.1f)\n", info.mLastImpactET, info.mLastImpactMagnitude,
			info.mLastImpactPos.x, info.mLastImpactPos.y, info.mLastImpactPos.z );

		// Wheels
		for( long i = 0; i < 4; ++i )
		{
			const TelemWheelV2 &wheel = info.mWheel[i];
			fprintf( fo, "Wheel=%s\n", (i==0)?"FrontLeft":(i==1)?"FrontRight":(i==2)?"RearLeft":"RearRight" );
			fprintf( fo, " ForwardRotation=%.1f\n", -wheel.mRotation );
			fprintf( fo, " SuspensionDeflection=%.3f RideHeight=%.3f\n", wheel.mSuspensionDeflection, wheel.mRideHeight );
			fprintf( fo, " TireLoad=%.1f LateralForce=%.1f GripFract=%.3f\n", wheel.mTireLoad, wheel.mLateralForce, wheel.mGripFract );
			fprintf( fo, " BrakeTemp=%.1f TirePressure=%.1f\n", wheel.mBrakeTemp, wheel.mPressure );
			fprintf( fo, " TireTemp(l/c/r)=%.1f/%.1f/%.1f\n", wheel.mTemperature[0], wheel.mTemperature[1], wheel.mTemperature[2] );
			fprintf( fo, " Wear=%.3f TerrainName=%s SurfaceType=%d\n", wheel.mWear, wheel.mTerrainName, wheel.mSurfaceType );
			fprintf( fo, " Flat=%d Detached=%d\n", wheel.mFlat, wheel.mDetached );
		}

		// Compute some auxiliary info based on the above
		TelemVect3 forwardVector = { -info.mOriX.z, -info.mOriY.z, -info.mOriZ.z };
		TelemVect3    leftVector = {  info.mOriX.x,  info.mOriY.x,  info.mOriZ.x };

		// These are normalized vectors, and remember that our world Y coordinate is up.  So you can
		// determine the current pitch and roll (w.r.t. the world x-z plane) as follows:
		const float pitch = atan2f( forwardVector.y, sqrtf( ( forwardVector.x * forwardVector.x ) + ( forwardVector.z * forwardVector.z ) ) );
		const float  roll = atan2f(    leftVector.y, sqrtf( (    leftVector.x *    leftVector.x ) + (    leftVector.z *    leftVector.z ) ) );
		const float radsToDeg = 57.296f;
		fprintf( fo, "Pitch = %.1f deg, Roll = %.1f deg\n", pitch * radsToDeg, roll * radsToDeg );

		const float metersPerSec = sqrtf( ( info.mLocalVel.x * info.mLocalVel.x ) +
			( info.mLocalVel.y * info.mLocalVel.y ) +
			( info.mLocalVel.z * info.mLocalVel.z ) );
		fprintf( fo, "Speed = %.1f KPH, %.1f MPH\n\n", metersPerSec * 3.6f, metersPerSec * 2.237f );

		// Close file
		fclose( fo );
	}
}
void SimTelemetryPlugin::UpdateScoring( const ScoringInfoV2 &info )
{
	// Update session.
	telem->Session.AmbientTemperature = info.mAmbientTemp;
	telem->Session.Cars = info.mNumVehicles;
	telem->Session.Flag_FullCourseYellow = ((info.mGamePhase == 6)?true:false);
	telem->Session.Flag_Green =  ((info.mGamePhase == 5)?true:false);
	telem->Session.Flags_S1 = info.mSectorFlag[0];
	telem->Session.Flags_S2 = info.mSectorFlag[1];
	telem->Session.Flags_S3 = info.mSectorFlag[2];

	telem->Session.IsRace = true; // How to read?
	telem->Session.MaximumLaps = info.mMaxLaps;
	telem->Session.Time = info.mCurrentET;
	telem->Session.TimeClock = info.mSession;
	telem->Session.TimeEnd = info.mEndET;
	telem->Session.TrackTemperature = info.mTrackTemp;
	strcpy(telem->Session.TrackName, info.mTrackName);
	telem->Session.SessionType = RACE; // ???
	telem->Session.StartLight = info.mStartLight;
	telem->Session.StartLightCount = info.mNumRedLights;

	telem->Session.Wetness_OnPath = info.mOnPathWetness;
	telem->Session.Wetness_OffPath = info.mOffPathWetness;

	// Update each player
	for(int i = 0; i < info.mNumVehicles; i++)
	{
		VehicleScoringInfoV2 &veh = info.mVehicle[i];

		telem->Drivers[i].InPits = veh.mInPits;
		telem->Drivers[i].AIControl = ((veh.mControl != 0)?true : false);
		telem->Drivers[i].IsPlayer = veh.mIsPlayer;

		telem->Drivers[i].LapsBehind = veh.mLapsBehindNext;
		telem->Drivers[i].SplitBehind = veh.mTimeBehindNext;

		telem->Drivers[i].LapsLeader = veh.mLapsBehindLeader;
		telem->Drivers[i].SplitBehind = veh.mTimeBehindLeader;

		telem->Drivers[i].LapStartTime = veh.mLapStartET;

		telem->Drivers[i].Position = veh.mPlace;
		strcpy(telem->Drivers[i].VehicleName, veh.mVehicleClass);

		telem->Drivers[i].X = veh.mPos.x;
		telem->Drivers[i].Y = veh.mPos.y;
		telem->Drivers[i].Z = veh.mPos.z;


		float metersPerSec = sqrtf( ( veh.mLocalVel.x * veh.mLocalVel.x ) +
			( veh.mLocalVel.y * veh.mLocalVel.y ) +
			( veh.mLocalVel.z * veh.mLocalVel.z ) );
		telem->Drivers[i].Speed = metersPerSec;

	}
	return;
	// Note: function is called twice per second now (instead of once per second in previous versions)
	FILE *fo = fopen( "ExampleInternalsScoringOutput.txt", "a" );
	if( fo != NULL )
	{
		// Print general scoring info
		fprintf( fo, "TrackName=%s\n", info.mTrackName );
		fprintf( fo, "Session=%d NumVehicles=%d CurET=%.3f\n", info.mSession, info.mNumVehicles, info.mCurrentET );
		fprintf( fo, "EndET=%.3f MaxLaps=%d LapDist=%.1f\n", info.mEndET, info.mMaxLaps, info.mLapDist );

		// Note that only one plugin can use the stream (by enabling scoring updates) ... sorry if any clashes result
		fprintf( fo, "START STREAM\n" );
		const char *ptr = info.mResultsStream;
		while( *ptr != NULL )
			fputc( *ptr++, fo );
		fprintf( fo, "END STREAM\n" );

		// New version 2 stuff
		fprintf( fo, "GamePhase=%d YellowFlagState=%d SectorFlags=(%d,%d,%d)\n", info.mGamePhase, info.mYellowFlagState,
			info.mSectorFlag[0], info.mSectorFlag[1], info.mSectorFlag[2] );
		fprintf( fo, "InRealtime=%d StartLight=%d NumRedLights=%d\n", info.mInRealtime, info.mStartLight, info.mNumRedLights );
		fprintf( fo, "PlayerName=%s PlrFileName=%s\n", info.mPlayerName, info.mPlrFileName );
		fprintf( fo, "DarkCloud=%.2f Raining=%.2f AmbientTemp=%.1f TrackTemp=%.1f\n", info.mDarkCloud, info.mRaining, info.mAmbientTemp, info.mTrackTemp );
		fprintf( fo, "Wind=(%.1f,%.1f,%.1f) OnPathWetness=%.2f OffPathWetness=%.2f\n", info.mWind.x, info.mWind.y, info.mWind.z, info.mOnPathWetness, info.mOffPathWetness );

		// Print vehicle info
		for( long i = 0; i < info.mNumVehicles; ++i )
		{
			VehicleScoringInfoV2 &vinfo = info.mVehicle[ i ];
			fprintf( fo, "Driver %d: %s\n", i, vinfo.mDriverName );
			fprintf( fo, " Vehicle=%s\n", vinfo.mVehicleName );
			fprintf( fo, " Laps=%d Sector=%d FinishStatus=%d\n", vinfo.mTotalLaps, vinfo.mSector, vinfo.mFinishStatus );
			fprintf( fo, " LapDist=%.1f PathLat=%.2f RelevantTrackEdge=%.2f\n", vinfo.mLapDist, vinfo.mPathLateral, vinfo.mTrackEdge );
			fprintf( fo, " Best=(%.3f, %.3f, %.3f)\n", vinfo.mBestSector1, vinfo.mBestSector2, vinfo.mBestLapTime );
			fprintf( fo, " Last=(%.3f, %.3f, %.3f)\n", vinfo.mLastSector1, vinfo.mLastSector2, vinfo.mLastLapTime );
			fprintf( fo, " Current Sector 1 = %.3f, Current Sector 2 = %.3f\n", vinfo.mCurSector1, vinfo.mCurSector2 );
			fprintf( fo, " Pitstops=%d, Penalties=%d\n", vinfo.mNumPitstops, vinfo.mNumPenalties );

			// New version 2 stuff
			fprintf( fo, " IsPlayer=%d Control=%d InPits=%d LapStartET=%.3f\n", vinfo.mIsPlayer, vinfo.mControl, vinfo.mInPits, vinfo.mLapStartET );
			fprintf( fo, " Place=%d VehicleClass=%s\n", vinfo.mPlace, vinfo.mVehicleClass );
			fprintf( fo, " TimeBehindNext=%.3f LapsBehindNext=%d\n", vinfo.mTimeBehindNext, vinfo.mLapsBehindNext );
			fprintf( fo, " TimeBehindLeader=%.3f LapsBehindLeader=%d\n", vinfo.mTimeBehindLeader, vinfo.mLapsBehindLeader );
			fprintf( fo, " Pos=(%.3f,%.3f,%.3f)\n", vinfo.mPos.x, vinfo.mPos.y, vinfo.mPos.z );

			// Forward is roughly in the -z direction (although current pitch of car may cause some y-direction velocity)
			fprintf( fo, " LocalVel=(%.2f,%.2f,%.2f)\n", vinfo.mLocalVel.x, vinfo.mLocalVel.y, vinfo.mLocalVel.z );
			fprintf( fo, " LocalAccel=(%.1f,%.1f,%.1f)\n", vinfo.mLocalAccel.x, vinfo.mLocalAccel.y, vinfo.mLocalAccel.z );

			// Orientation matrix is left-handed
			fprintf( fo, " [%6.3f,%6.3f,%6.3f]\n", vinfo.mOriX.x, vinfo.mOriX.y, vinfo.mOriX.z );
			fprintf( fo, " [%6.3f,%6.3f,%6.3f]\n", vinfo.mOriY.x, vinfo.mOriY.y, vinfo.mOriY.z );
			fprintf( fo, " [%6.3f,%6.3f,%6.3f]\n", vinfo.mOriZ.x, vinfo.mOriZ.y, vinfo.mOriZ.z );
			fprintf( fo, " LocalRot=(%.3f,%.3f,%.3f)\n", vinfo.mLocalRot.x, vinfo.mLocalRot.y, vinfo.mLocalRot.z );
			fprintf( fo, " LocalRotAccel=(%.2f,%.2f,%.2f)\n", vinfo.mLocalRotAccel.x, vinfo.mLocalRotAccel.y, vinfo.mLocalRotAccel.z );
		}

		// Delimit sections
		fprintf( fo, "\n" );

		// Close file
		fclose( fo );
	}
}


void SimTelemetryPlugin::UpdateGraphics( const GraphicsInfoV2 &info ) { return; }
bool SimTelemetryPlugin::CheckHWControl( const char * const controlName, float &fRetVal ) { return false; }
bool SimTelemetryPlugin::ForceFeedback( float &forceValue ) { return false; }




bool SimTelemetryPlugin::RequestCommentary( CommentaryRequestInfo &info )
{
	return( false );
}

