using Enums;
using RaceTrackScene;
using UnityEngine;

namespace Messages
{
    public struct PlaneStateMessage
    {
        public FighterPlaneUI FighterPlaneUi;
        public FighterPlaneSounds FighterPlaneSounds;
        public FighterPlaneType FighterPlaneType;
        
        public string PlayerName;
        public string EnemyName;
        
        public string PlayerPlaneName;
        public string EnemyPlaneName;
        
        public Sprite PlayerSprite;
        public Sprite EnemySprite;
        
        public bool IsEnemy;
    }
}