namespace Project
{
    using DG.Tweening;
    using LudicWorlds;
    using UnityEngine;

    public class SingleAttackStateState : MonoBehaviour, IState, IAttackState
    { 
        [Header("Debug")]
        [SerializeField] 
        private string _id;
        
        private Unit _unit;
        private Registry<IDamageable> _targets;
        private float _attackDuration;
        private string _afterAttackStateId;
        private IStateMachine _stateMachine;
        private bool _isOnCooldown;

        public string ID => _id;
        public bool CanAttack => !_isOnCooldown;

        public void Init(string id, Unit unit, Registry<IDamageable> targets, float attackDuration, string afterAttackStateId, IStateMachine stateMachine)
        {
            _id = id;
            _unit = unit;
            _targets = targets;
            _attackDuration = attackDuration;
            _afterAttackStateId = afterAttackStateId;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _targets.Items[0].TakeDamage(_unit.Attack);

            DOTween.Sequence()
                .SetLink(_unit.gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_attackDuration)
                .AppendCallback(AfterAttack);

            _isOnCooldown = true;
            DOTween.Sequence()
                .SetLink(_unit.gameObject, LinkBehaviour.KillOnDisable)
                .AppendInterval(_unit.AttackCooldown)
                .AppendCallback(OnCooldownPass);
        }

        private void AfterAttack()
        {
            _stateMachine.SetState(_afterAttackStateId);
        }

        private void OnCooldownPass()
        {
            _isOnCooldown = false;
        }

        public void UpdateState() { }

        public void Exit() { }

        public void Dispose() { }
    }
}
