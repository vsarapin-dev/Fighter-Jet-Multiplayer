using System;
using System.Collections.Generic;
using Enums;
using Interfaces;
using Messages;
using RaceTrackScene;
using UnityEngine;

namespace StateBehaviours.FighterPlane
{
    public class FighterPlaneStateBehaviour : MonoBehaviour
    {
        [SerializeField] private FighterPlaneUI fighterPlaneUi;
        [SerializeField] private FighterPlaneSounds fighterPlaneSounds;
        
        private Dictionary<Type, IPlaneBehaviour> _behavioursMap = new Dictionary<Type, IPlaneBehaviour>();
        private IPlaneBehaviour _behaviourCurrent;
        
        private PlaneStateMessage _planeStateMessage;

        private void OnEnable()
        {
            Actions.OnSetDefaultPlaneBehaviour += SetDefaultPlaneStateBehaviour;
            Actions.OnSetCriticalPlaneBehaviour += SetCriticalPlaneStateBehaviour;
            Actions.OnSetDyingPlaneBehaviour += SetDyingPlaneStateBehaviour;
            Actions.OnSetDisabledPlaneBehaviour += SetDisabledPlaneStateBehaviour;
        }
        
        private void OnDisable()
        {
            Actions.OnSetDefaultPlaneBehaviour -= SetDefaultPlaneStateBehaviour;
            Actions.OnSetCriticalPlaneBehaviour -= SetCriticalPlaneStateBehaviour;
            Actions.OnSetDyingPlaneBehaviour -= SetDyingPlaneStateBehaviour;
            Actions.OnSetDisabledPlaneBehaviour -= SetDisabledPlaneStateBehaviour;
        }
        
        private void Start()
        {
            InitBehaviours();
        }

        private void SetDefaultPlaneStateBehaviour(PlaneStateMessage planeStateMessage)
        {
            InitPlaneMessage(planeStateMessage);
            
            var behaviour = GetBehaviour<DefaultPlaneStateBehaviour>();
            SetBehaviour(behaviour);
        }
        
        private void SetCriticalPlaneStateBehaviour()
        {
            InitPlaneMessage(new PlaneStateMessage());
            
            var behaviour = GetBehaviour<CriticalPlaneStateBehaviour>();
            SetBehaviour(behaviour);
        }
        
        private void SetDyingPlaneStateBehaviour(FighterPlaneType fighterPlaneType)
        {
            PlaneStateMessage planeStateMessage = new PlaneStateMessage
            {
                FighterPlaneType = fighterPlaneType
            };
            
            InitPlaneMessage(planeStateMessage);
            
            var behaviour = GetBehaviour<DyingPlaneStateBehaviour>();
            SetBehaviour(behaviour);
        }
        
        private void SetDisabledPlaneStateBehaviour()
        {
            InitPlaneMessage(new PlaneStateMessage());
            
            var behaviour = GetBehaviour<DisabledPlaneStateBehaviour>();
            SetBehaviour(behaviour);
        }

        private void InitPlaneMessage(PlaneStateMessage planeStateMessage)
        {
            _planeStateMessage = planeStateMessage;
            _planeStateMessage.FighterPlaneUi = fighterPlaneUi;
            _planeStateMessage.FighterPlaneSounds = fighterPlaneSounds;
        }
        
        private void InitBehaviours()
        {
            _behavioursMap[typeof(DefaultPlaneStateBehaviour)] = new DefaultPlaneStateBehaviour();
            _behavioursMap[typeof(CriticalPlaneStateBehaviour)] = new CriticalPlaneStateBehaviour();
            _behavioursMap[typeof(DyingPlaneStateBehaviour)] = new DyingPlaneStateBehaviour();
            _behavioursMap[typeof(DisabledPlaneStateBehaviour)] = new DisabledPlaneStateBehaviour();
        }

        private void SetBehaviour(IPlaneBehaviour newBehaviour)
        {
            _behaviourCurrent = newBehaviour;
            _behaviourCurrent.Enter(_planeStateMessage);
        }
        
        private IPlaneBehaviour GetBehaviour<T>() where T : IPlaneBehaviour
        {
            var type = typeof(T);
            return _behavioursMap[type];
        }
        
    }
}