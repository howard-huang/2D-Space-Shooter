/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID BGM_PLAY = 1093876158U;
        static const AkUniqueID BGM_STOP = 2090192256U;
        static const AkUniqueID ENEMYLASER_PLAY = 1194956557U;
        static const AkUniqueID EXPLOSION_PLAY = 2090513163U;
        static const AkUniqueID HEALTHPOWERUP_PLAY = 314250078U;
        static const AkUniqueID LASER_PLAY = 3430907247U;
        static const AkUniqueID MISSILE_PLAY = 1189790860U;
        static const AkUniqueID MISSILE_STOP = 3159575446U;
        static const AkUniqueID NOAMMO_PLAY = 3842339375U;
        static const AkUniqueID PLAYERSFX_STOP = 1158098870U;
        static const AkUniqueID POWERUP_PLAY = 2538393448U;
        static const AkUniqueID SHIELD_PLAY = 3556904531U;
        static const AkUniqueID SHIELD_STOP = 2686923321U;
        static const AkUniqueID SPEEDBOOST_PLAY = 1600032724U;
        static const AkUniqueID SPEEDBOOST_STOP = 3262461614U;
        static const AkUniqueID SUPERLASER_PLAY = 4208992348U;
        static const AkUniqueID SUPERLASER_STOP = 2962696902U;
        static const AkUniqueID THRUSTER_START = 2051128187U;
        static const AkUniqueID THRUSTER_STOP = 2308976345U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace THRUSTERS
        {
            static const AkUniqueID GROUP = 1944582317U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID SPEEDACTIVE = 4248879684U;
                static const AkUniqueID SPEEDINACTIVE = 220333237U;
            } // namespace STATE
        } // namespace THRUSTERS

    } // namespace STATES

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MUSIC_SOUNDBANK = 3589812408U;
        static const AkUniqueID SFX_SOUNDBANK = 2641024368U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
