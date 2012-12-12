#ifndef SHAREDMEMORY_H
#define SHAREDMEMORY_H
#include <windows.h>

class SharedMemory
{
protected:

	char* name;
	int mapsize;

	HANDLE hMapFile;
	void* pBuffer;
	bool SharedMemoryHooked;

	void LogError(const char* const log);

public: 
	bool Hooked() { return SharedMemoryHooked; }
	void* GetBuffer() { return pBuffer; }

	SharedMemory(char* map, int mapsize);
	~SharedMemory();


};

#endif