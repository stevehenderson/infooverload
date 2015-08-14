#include <stdio.h>
#include <windows.h>
#include "VBSPlugin.h"


#define WINDOWS_DEBUG true
/**
*     Debug handler
*   http://bytes.com/forum/thread212702.html
*/            
void debug(char *fmt, ...)
{   
	if(WINDOWS_DEBUG) {
        char buffer[255];
        va_list argp;                    
        va_start(argp, fmt);
        vsprintf_s(buffer, fmt, argp);
        va_end(argp);
        OutputDebugString(buffer);
    } 
}


// Command function declaration
typedef int (WINAPI * ExecuteCommandType)(const char *command, char *result, int resultLength);

// Command function definition
ExecuteCommandType ExecuteCommand = NULL;

// Function that will register the ExecuteCommand function of the engine
VBSPLUGIN_EXPORT void WINAPI RegisterCommandFnc(void *executeCommandFnc)
{
  ExecuteCommand = (ExecuteCommandType)executeCommandFnc;
  x=1;
}

// This function will be executed every simulation step (every frame) and took a part in the simulation procedure.
// We can be sure in this function the ExecuteCommand registering was already done.
// deltaT is time in seconds since the last simulation step
VBSPLUGIN_EXPORT void WINAPI OnSimulationStep(float deltaT)
{
  //{ Sample code:
  
  char * pch;

  //Get pitch and bank	
  char result[4096];    
  ExecuteCommand("player getVariable \"pitchBank\"", result, 4096-1);
  debug(result);
  
  //int result = sscanf(string, "%[^','],%[^','],%g, %g", string1, string2, d1, d2);
  //  [2556.32,240.6]
  //http://www.cplusplus.com/reference/clibrary/cstring/strtok/
  pch = strtok (result,"[,]");
  
  int i = 0;
  while (pch != NULL)
  {
    //debug("%s at token %d",pch,i);    
	if((i==0)&&(pch!=NULL)) pitch=atof(pch);
	if((i==1)&&(pch!=NULL)) roll=atof(pch);
	pch = strtok (NULL, "[,]");
	i++;
  }

  

  //Get roll
  char result_dir[4096];    
  ExecuteCommand("getDir apache", result_dir, 4096-1);
  //debug(result_dir);
  if(result_dir!=NULL) yaw=atof(result_dir);

  //Get x,y,z
  char result_pos[4096];    
  ExecuteCommand("getPos apache", result_pos, 4096-1);
  //debug(result_pos);
  pch = strtok (result_pos,"[,]");
  
  i = 0;
  while (pch != NULL)
  {
    //debug("%s at token %d",pch,i);    
	if((i==0)&&(pch!=NULL)) x=atof(pch);
	if((i==1)&&(pch!=NULL)) y=atof(pch);
	if((i==2)&&(pch!=NULL)) z=atof(pch);
	pch = strtok (NULL, "[,]");
	i++;
  }

  //debug("pitch=%f", pitch);
  //debug("roll=%f", roll);
  //debug("yaw=%f", yaw);
  //debug("x=%f", x);
  //debug("y=%f", y);
  //debug("z=%f", pitch);
  

  //sscanf(result, "%f", &x);
  
  //!}
}

// This function will be executed every time the script in the engine calls the script function "pluginFunction"
// We can be sure in this function the ExecuteCommand registering was already done.
// Note that the plugin takes responsibility for allocating and deleting the returned string
VBSPLUGIN_EXPORT const char* WINAPI PluginFunction(const char *input)
{
  //{ Sample code:
  static const char result[]="[1.0, 3.75]";
  return result;
  //!}
}

VBSPLUGIN_EXPORT float WINAPI GetX() { return x;}
VBSPLUGIN_EXPORT void WINAPI SetX(float i) { x=i;}
VBSPLUGIN_EXPORT float WINAPI GetY() {  return y;}
VBSPLUGIN_EXPORT void WINAPI SetY(float i) { x=i; }
VBSPLUGIN_EXPORT float WINAPI GetZ() {  return z;}
VBSPLUGIN_EXPORT void WINAPI SetZ(float i) { z=i; }
VBSPLUGIN_EXPORT float WINAPI GetPitch() {  return pitch;}
VBSPLUGIN_EXPORT void WINAPI SetPitch(float i) { pitch=i; }
VBSPLUGIN_EXPORT float WINAPI GetRoll() {  return roll;}
VBSPLUGIN_EXPORT void WINAPI SetRoll(float i) { roll=i; }
VBSPLUGIN_EXPORT float WINAPI GetYaw() {  return yaw;}
VBSPLUGIN_EXPORT void WINAPI SetYaw(float i) { yaw=i; }

// DllMain
BOOL WINAPI DllMain(HINSTANCE hDll, DWORD fdwReason, LPVOID lpvReserved)
{
   switch(fdwReason)
   {
      case DLL_PROCESS_ATTACH:
         OutputDebugString("Called DllMain with DLL_PROCESS_ATTACH\n");
         break;
      case DLL_PROCESS_DETACH:
         OutputDebugString("Called DllMain with DLL_PROCESS_DETACH\n");
         break;
      case DLL_THREAD_ATTACH:
         OutputDebugString("Called DllMain with DLL_THREAD_ATTACH\n");
         break;
      case DLL_THREAD_DETACH:
         OutputDebugString("Called DllMain with DLL_THREAD_DETACH\n");
         break;

		 x = 0;
   }
   return TRUE;
}
