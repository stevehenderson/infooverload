#define VBSPLUGIN_EXPORT __declspec(dllexport)

VBSPLUGIN_EXPORT void WINAPI RegisterCommandFnc(void *executeCommandFnc);
VBSPLUGIN_EXPORT void WINAPI OnSimulationStep(float deltaT);
VBSPLUGIN_EXPORT const char* WINAPI PluginFunction(const char *input);
VBSPLUGIN_EXPORT float WINAPI GetX();
VBSPLUGIN_EXPORT void WINAPI SetX(float i);
VBSPLUGIN_EXPORT float WINAPI GetY();
VBSPLUGIN_EXPORT void WINAPI SetY(float i);
VBSPLUGIN_EXPORT float WINAPI GetZ();
VBSPLUGIN_EXPORT void WINAPI SetZ(float i);
VBSPLUGIN_EXPORT float WINAPI GetPitch();
VBSPLUGIN_EXPORT void WINAPI SetPitch(float i);
VBSPLUGIN_EXPORT float WINAPI GetRoll();
VBSPLUGIN_EXPORT void WINAPI SetRoll(float i);
VBSPLUGIN_EXPORT float WINAPI GetYaw();
VBSPLUGIN_EXPORT void WINAPI SetYaw(float i);

#pragma data_seg("SHARED")
	float x=0;
	float y=0;
	float z=0;
	float pitch=0;
	float roll=0;
	float yaw=0;
#pragma data_seg()


#pragma comment(linker, "/section:SHARED,RWS")