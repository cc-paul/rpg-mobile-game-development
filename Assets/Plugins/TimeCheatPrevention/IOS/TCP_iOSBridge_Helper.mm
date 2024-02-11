#import "TCP_iOSBridge_Helper.h"

@implementation TCP_iOSBridge_Helper

//////////////////////////////////////////////////////////////////////////

#include <sys/sysctl.h>

static int64_t us_since_boot() 
{
		struct timeval boottime;
		int mib[2] = {CTL_KERN, KERN_BOOTTIME};
		size_t size = sizeof(boottime);
		int rc = sysctl(mib, 2, &boottime, &size, NULL, 0);
		if (rc != 0) {
			return 0;
		}
		return (int64_t)boottime.tv_sec * 1000000 + (int64_t)boottime.tv_usec;
}

- (int64_t)us_uptime
{
		int64_t before_now;
		int64_t after_now;
		struct timeval now;

		after_now = us_since_boot();
		do {
				before_now = after_now;
				gettimeofday(&now, NULL);
				after_now = us_since_boot();
		} while (after_now != before_now);

		int64_t val = (int64_t)now.tv_sec * 1000000 + (int64_t)now.tv_usec - before_now;

		// Conversion from micro secondes (1 millionth of a second)
		return (val / 1000000);
}

//////////////////////////////////////////////////////////////////////////

@end

//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////

extern "C" 
{
	long GetElapsedTimeSinceLastBoot();
}

//////////////////////////////////////////////////////////////////////////

static TCP_iOSBridge_Helper *TCP_Helper = nil;

//////////////////////////////////////////////////////////////////////////

long GetElapsedTimeSinceLastBoot()
{
	// Create the Helper Object instance if none exists yet
	if(TCP_Helper == nil)
	{
		TCP_Helper = [TCP_iOSBridge_Helper alloc];
	}

	return [TCP_Helper us_uptime];
}

//////////////////////////////////////////////////////////////////////////

