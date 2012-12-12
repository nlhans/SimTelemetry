//‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹‹
//›                                                                         ﬁ
//› Module: Internals Example Header File                                   ﬁ
//›                                                                         ﬁ
//› Description: Declarations for the Internals Example Plugin              ﬁ
//›                                                                         ﬁ
//›                                                                         ﬁ
//› This source code module, and all information, data, and algorithms      ﬁ
//› associated with it, are part of CUBE technology (tm).                   ﬁ
//›                 PROPRIETARY AND CONFIDENTIAL                            ﬁ
//› Copyright (c) 1996-2007 Image Space Incorporated.  All rights reserved. ﬁ
//›                                                                         ﬁ
//›                                                                         ﬁ
//› Change history:                                                         ﬁ
//›   tag.2005.11.30: created                                               ﬁ
//›                                                                         ﬁ
//ﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂﬂ

#ifndef _INTERNALS_EXAMPLE_H
#define _INTERNALS_EXAMPLE_H


#include "Telemetry.h"
#include "SharedMemory.h"

#include "InternalsPlugin.hpp"


class InternalsPluginInfo : public PluginObjectInfo
{
 public:

  // Constructor/destructor
  InternalsPluginInfo();
  ~InternalsPluginInfo() {}

  // Derived from base class PluginObjectInfo
  virtual const char*    GetName()     const;
  virtual const char*    GetFullName() const;
  virtual const char*    GetDesc()     const;
  virtual const unsigned GetType()     const;
  virtual const char*    GetSubType()  const;
  virtual const unsigned GetVersion()  const;
  virtual void*          Create() const;

 private:

  char m_szFullName[128];
};

class SimTelemetryPlugin : public InternalsPluginV3
{
 protected:


  SharedMemory* mem;
  const static char m_szName[];
  const static char m_szSubType[];
  const static unsigned m_uID;
  const static unsigned m_uVersion;

 public:

  // Constructor/destructor
  SimTelemetryPlugin() {}
  ~SimTelemetryPlugin() {}

  // Called from class InternalsPluginInfo to return specific information about plugin
  static const char *   GetName()                     { return m_szName; }
  static const unsigned GetType()                     { return PO_INTERNALS; }
  static const char *   GetSubType()                  { return m_szSubType; }
  static const unsigned GetVersion()                  { return m_uVersion; }

  // Derived from base class PluginObject
  void                  Destroy()                     { Shutdown(); }  // poorly named ... doesn't destroy anything
  PluginObjectInfo *    GetInfo();
  unsigned              GetPropertyCount() const      { return 0; }
  PluginObjectProperty *GetProperty( const char * )   { return 0; }
  PluginObjectProperty *GetProperty( const unsigned ) { return 0; }

  // These are the functions derived from base class InternalsPlugin
  // that can be implemented.
  void Startup();         // game startup
  void Shutdown();        // game shutdown

  void EnterRealtime();   // entering realtime
  void ExitRealtime();    // exiting realtime

  void StartSession();    // session has started
  void EndSession();      // session has ended

  // GAME OUTPUT
  bool WantsTelemetryUpdates() { return( true ); } // CHANGE TO TRUE TO ENABLE TELEMETRY EXAMPLE!
  void UpdateTelemetry( const TelemInfoV2 &info );
  void UpdateWheelTelemetry (const TelemInfoV2& info, int i, rFactorWheel* wheel);

  bool WantsGraphicsUpdates() { return( false ); } // CHANGE TO TRUE TO ENABLE GRAPHICS EXAMPLE!
  void UpdateGraphics( const GraphicsInfoV2 &info );

  // GAME INPUT
  bool HasHardwareInputs() { return( false ); } // CHANGE TO TRUE TO ENABLE HARDWARE EXAMPLE!
  void UpdateHardware( const float fDT ) { mET += fDT; } // update the hardware with the time between frames
  void EnableHardware() { mEnabled = true; }             // message from game to enable hardware
  void DisableHardware() { mEnabled = false; }           // message from game to disable hardware

  // See if the plugin wants to take over a hardware control.  If the plugin takes over the
  // control, this method returns true and sets the value of the float pointed to by the
  // second arg.  Otherwise, it returns false and leaves the float unmodified.
  bool CheckHWControl( const char * const controlName, float &fRetVal );

  bool ForceFeedback( float &forceValue );  // SEE FUNCTION BODY TO ENABLE FORCE EXAMPLE

  // SCORING OUTPUT
  bool WantsScoringUpdates() { return( true ); } // CHANGE TO TRUE TO ENABLE SCORING EXAMPLE!
  void UpdateScoring( const ScoringInfoV2 &info );

  // COMMENTARY INPUT
  bool RequestCommentary( CommentaryRequestInfo &info );  // SEE FUNCTION BODY TO ENABLE COMMENTARY EXAMPLE

 private:

  void WriteToAllExampleOutputFiles( const char * const openStr, const char * const msg );
  float mET;  // needed for the hardware example
  bool mEnabled; // needed for the hardware example
};


#endif // _INTERNALS_EXAMPLE_H

