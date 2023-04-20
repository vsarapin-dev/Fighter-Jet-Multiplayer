using System;
using Enums;
using Messages;

public static class Actions
{
    // Voice chat
    public static Action<string> OnStartVoiceChat;
    public static Action<string> OnStopVoiceChat;
    public static Action<string> OnStopRemoteVoiceChat;
    public static Action OnRemoveAllVoiceChatsIcons;
    
    //Fighter Plain Selection
    public static Action<string> OnFighterPlainSelected;
    public static Action OnAllPlayersSelectFighterPlain;
    
    // Game scene actions
    public static Action<float, float> OnChangeCurrentPlayerHealthOnUi;
    public static Action<float, float> OnChangeEnemyPlayerHealthOnUi;

    public static Action<bool> OnFighterPlaneRadarActivate;
    
    public static Action<PlaneStateMessage> OnSetDefaultPlaneBehaviour;
    public static Action<FighterPlaneType> OnSetDyingPlaneBehaviour;
    public static Action OnSetCriticalPlaneBehaviour;
    public static Action OnSetDisabledPlaneBehaviour;
}
