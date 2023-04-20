using Interfaces;
using Messages;

namespace StateBehaviours.FighterPlane
{
    public class DefaultPlaneStateBehaviour : IPlaneBehaviour
    {
        private PlaneStateMessage _planeStateMessage;
        
        public void Enter(PlaneStateMessage planeStateMessage)
        {
            _planeStateMessage = planeStateMessage;

            SetPlanesHealthUiSpritesOnCanvas();
            SetPlanesHealthUiPlayerNamesOnCanvas();
            SetPlanesHealthUiPlaneName();
        }

        private void SetPlanesHealthUiSpritesOnCanvas()
        {
            if (_planeStateMessage.IsEnemy)
            {
                _planeStateMessage.FighterPlaneUi.SetEnemyPlayerHealthUiSprite(_planeStateMessage.EnemySprite);
            }
            else
            {
                _planeStateMessage.FighterPlaneUi.SetCurrentPlayerHealthUiSprite(_planeStateMessage.PlayerSprite);
            }
        }
    
        private void SetPlanesHealthUiPlayerNamesOnCanvas()
        {
            if (_planeStateMessage.IsEnemy)
            {
                _planeStateMessage.FighterPlaneUi.SetEnemyPlayerHealthUiPlayerName(_planeStateMessage.EnemyName);
            }
            else
            {
                _planeStateMessage.FighterPlaneUi.SetCurrentPlayerHealthUiPlayerName(_planeStateMessage.PlayerName);
            }
        }
        
        private void SetPlanesHealthUiPlaneName()
        {
            if (_planeStateMessage.IsEnemy)
            {
                _planeStateMessage.FighterPlaneUi.SetEnemyPlayerHealthUiPlaneName(_planeStateMessage.EnemyPlaneName);
            }
            else
            {
                _planeStateMessage.FighterPlaneUi.SetCurrentPlayerHealthUiPlaneName(_planeStateMessage.PlayerPlaneName);
            }
        }
        
    }
}